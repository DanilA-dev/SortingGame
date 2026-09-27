using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D_Dev.MeshPathCreator
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshPathCreator : MonoBehaviour
    {
        #region Structs

        [System.Serializable]
        public struct PathPoint
        {
            public Vector3 position;
            public float radius;
            public float cornerSmoothing;

            public PathPoint(Vector3 position, float radius = 1f, float cornerSmoothing = 0.5f)
            {
                this.position = position;
                this.radius = radius;
                this.cornerSmoothing = Mathf.Clamp01(cornerSmoothing);
            }
        }

        #endregion

        #region Fields

        [SerializeField] private List<PathPoint> _points = new();
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private float _segmentsPerUnit = 1f;
        [SerializeField] private bool _addMeshCollider;
        [SerializeField] private bool _closed;

        private Mesh _mesh;
        private MeshCollider _meshCollider;

        #endregion

        #region Properties

        public List<PathPoint> Points => _points;
        
        public float SegmentsPerUnit
        {
            get => _segmentsPerUnit;
            set
            {
                _segmentsPerUnit = Mathf.Max(0.1f, value);
                UpdateMesh();
            }
        }
        public bool AddMeshCollider
        {
            get => _addMeshCollider;
            set
            {
                _addMeshCollider = value;
                UpdateCollider();
            }
        }
        public bool Closed
        {
            get => _closed;
            set
            {
                _closed = value;
                UpdateMesh();
            }
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            UpdateMesh();
        }

        private void Reset()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void OnValidate()
        {
            UpdateMesh();
        }

        #endregion

        #region Public

        public void AddPointToEnd(Vector3 position, float radius = 1f, float cornerSmoothing = 0.5f)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            _points.Add(new PathPoint(localPosition, radius, cornerSmoothing));
            UpdateMesh();
        }

        public void AddPointToStart(Vector3 position, float radius = 1f, float cornerSmoothing = 0.5f)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            _points.Insert(0, new PathPoint(localPosition, radius, cornerSmoothing));
            UpdateMesh();
        }

        public void UpdatePoint(int index, PathPoint point)
        {
            if (index >= 0 && index < _points.Count)
            {
                _points[index] = point;
                UpdateMesh();
            }
        }

        public void RemovePoint(int index)
        {
            if (index >= 0 && index < _points.Count)
            {
                _points.RemoveAt(index);
                UpdateMesh();
            }
        }

        public void ClearPoints()
        {
            _points.Clear();
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            _mesh ??= new Mesh();
            
            if (_mesh == null)
                _mesh = new Mesh();
            
            if (_points.Count < 2)
            {
                ClearMesh();
                return;
            }

            GenerateMesh();
            UpdateCollider();
        }

        #endregion

        #region Private

        private void ClearMesh()
        {
            if (_mesh != null)
                _mesh.Clear();
        }

        private void GenerateMesh()
        {
            if (_mesh == null)
                _mesh = new Mesh();
            
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            List<(Vector3 position, float radius)> pathPoints = GetSplinePoints();
            
            for (int i = 0; i < pathPoints.Count; i++)
            {
                var point = pathPoints[i];
                point.position = transform.InverseTransformPoint(point.position);
                pathPoints[i] = point;
            }
            
            if (pathPoints.Count < 2)
                return;

            Vector3 up = Vector3.up;
            float totalLength = 0;
            List<float> segmentLengths = new List<float>();

            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                float dist = Vector3.Distance(pathPoints[i].position, pathPoints[i + 1].position);
                segmentLengths.Add(dist);
                totalLength += dist;
            }

            for (int i = 0; i < pathPoints.Count; i++)
            {
                Vector3 currentPos = pathPoints[i].position;
                float currentRadius = pathPoints[i].radius;

                Vector3 forward;
                if (i < pathPoints.Count - 1)
                {
                    forward = (pathPoints[i + 1].position - currentPos).normalized;
                }
                else if (i > 0)
                {
                    forward = (currentPos - pathPoints[i - 1].position).normalized;
                }
                else
                {
                    forward = Vector3.forward;
                }

                Vector3 right = Vector3.Cross(up, forward).normalized;
                if (right.sqrMagnitude < 0.001f)
                {
                    right = Vector3.right;
                }

                vertices.Add(currentPos - right * currentRadius);
                vertices.Add(currentPos + right * currentRadius);

                float t = (i > 0) ? (segmentLengths.GetRange(0, i).Sum() / totalLength) : 0;
                uvs.Add(new Vector2(0, t));
                uvs.Add(new Vector2(1, t));
            }

            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                int baseIndex = i * 2;
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);

                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 3);
            }

            _mesh.Clear();
            _mesh.SetVertices(vertices);
            _mesh.SetTriangles(triangles, 0);
            _mesh.SetUVs(0, uvs);
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            
            _meshFilter.mesh = _mesh;
        }

        private List<(Vector3 position, float radius)> GetSplinePoints()
        {
            List<(Vector3 position, float radius)> result = new List<(Vector3, float)>();

            if (_points.Count < 2)
                return result;

            int segments = _closed ? _points.Count : _points.Count - 1;
            
            for (int i = 0; i < segments; i++)
            {
                Vector3 p0 = transform.TransformPoint(_points[GetIndex(i - 1)].position);
                Vector3 p1 = transform.TransformPoint(_points[GetIndex(i)].position);
                Vector3 p2 = transform.TransformPoint(_points[GetIndex(i + 1)].position);
                Vector3 p3 = transform.TransformPoint(_points[GetIndex(i + 2)].position);

                float r1 = _points[GetIndex(i)].radius;
                float r2 = _points[GetIndex(i + 1)].radius;

                float smooth1 = _points[GetIndex(i)].cornerSmoothing;
                float smooth2 = _points[GetIndex(i + 1)].cornerSmoothing;

                int numSubSegments = Mathf.Max(1, Mathf.CeilToInt(Vector3.Distance(p1, p2) * _segmentsPerUnit));

                for (int j = 0; j < numSubSegments; j++)
                {
                    float t = (float)j / numSubSegments;
                    float inverseT = 1f - t;

                    Vector3 position = CatmullRom(p0, p1, p2, p3, t);
                    float radius = Mathf.Lerp(r1, r2, t);

                    if (j == 0 && i == 0)
                    {
                        result.Add((position, radius));
                    }
                    else if (j == numSubSegments - 1 && i == segments - 1 && !_closed)
                    {
                        result.Add((position, radius));
                    }
                    else
                    {
                        result.Add((position, radius));
                    }
                }
            }

            return result;
        }

        private int GetIndex(int i)
        {
            if (_closed)
            {
                return (i % _points.Count + _points.Count) % _points.Count;
            }
            return Mathf.Clamp(i, 0, _points.Count - 1);
        }

        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3
            );
        }

        private void UpdateCollider()
        {
            if (_addMeshCollider && _meshCollider != null && _mesh != null && _mesh.vertexCount > 0)
                if (_meshCollider.sharedMesh != _mesh)
                    _meshCollider.sharedMesh = _mesh;
        }

        #endregion
    }
}
