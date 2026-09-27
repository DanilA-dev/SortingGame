#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace D_Dev.MeshPathCreator.Editor
{
    [CustomEditor(typeof(MeshPathCreator))]
    public class MeshPathCreatorEditor : UnityEditor.Editor
    {
        private MeshPathCreator _meshPathCreator;
        private int _selectedPointIndex = -1;
        private bool _isDragging;

        private void OnEnable()
        {
            _meshPathCreator = (MeshPathCreator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Point To End"))
            {
                AddPointToEnd();
            }
            if (GUILayout.Button("Add Point To Start"))
            {
                AddPointToStart();
            }
            if (GUILayout.Button("Remove Selected"))
            {
                RemoveSelectedPoint();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All"))
            {
                _meshPathCreator.ClearPoints();
                _selectedPointIndex = -1;
            }
            if (GUILayout.Button("Update Mesh"))
            {
                _meshPathCreator.UpdateMesh();
            }
            EditorGUILayout.EndHorizontal();

            if (_meshPathCreator.Points.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Selected Point:", EditorStyles.boldLabel);

                if (_selectedPointIndex >= 0 && _selectedPointIndex < _meshPathCreator.Points.Count)
                {
                    var point = _meshPathCreator.Points[_selectedPointIndex];
                    
                    EditorGUI.BeginChangeCheck();
                    Vector3 worldPos = _meshPathCreator.transform.TransformPoint(point.position);
                    worldPos = EditorGUILayout.Vector3Field("Position (World)", worldPos);
                    point.radius = EditorGUILayout.FloatField("Radius", point.radius);
                    point.cornerSmoothing = EditorGUILayout.Slider("Corner Smoothing", point.cornerSmoothing, 0f, 1f);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        point.position = _meshPathCreator.transform.InverseTransformPoint(worldPos);
                        _meshPathCreator.UpdatePoint(_selectedPointIndex, point);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Click a point in the scene to select it");
                }
            }
        }

        private void AddPointToEnd()
        {
            Vector3 newPosition;
            
            if (_meshPathCreator.Points.Count == 0)
            {
                newPosition = _meshPathCreator.transform.position;
            }
            else
            {
                var lastPoint = _meshPathCreator.Points[_meshPathCreator.Points.Count - 1];
                Vector3 lastWorldPos = _meshPathCreator.transform.TransformPoint(lastPoint.position);
                newPosition = lastWorldPos + Vector3.forward * 2f;
            }

            _meshPathCreator.AddPointToEnd(newPosition);
            _selectedPointIndex = _meshPathCreator.Points.Count - 1;
        }

        private void AddPointToStart()
        {
            Vector3 newPosition;
            
            if (_meshPathCreator.Points.Count == 0)
            {
                newPosition = _meshPathCreator.transform.position;
            }
            else
            {
                var firstPoint = _meshPathCreator.Points[0];
                Vector3 firstWorldPos = _meshPathCreator.transform.TransformPoint(firstPoint.position);
                newPosition = firstWorldPos - Vector3.forward * 2f;
            }

            _meshPathCreator.AddPointToStart(newPosition);
            _selectedPointIndex = 0;
        }

        private void RemoveSelectedPoint()
        {
            if (_selectedPointIndex >= 0)
            {
                _meshPathCreator.RemovePoint(_selectedPointIndex);
                _selectedPointIndex = -1;
            }
        }

        private void OnSceneGUI()
        {
            DrawSceneGUI();
        }

        private void DrawSceneGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                TrySelectPoint(e.mousePosition);
                
                if (_selectedPointIndex >= 0)
                {
                    _isDragging = true;
                    e.Use();
                }
            }

            if (e.type == EventType.MouseUp)
            {
                _isDragging = false;
            }

            DrawPoints();

            if (_selectedPointIndex >= 0 && _selectedPointIndex < _meshPathCreator.Points.Count)
            {
                HandleSelectedPoint();
            }
        }

        private void TrySelectPoint(Vector2 mousePosition)
        {
            float closestDist = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < _meshPathCreator.Points.Count; i++)
            {
                Vector3 worldPos = _meshPathCreator.transform.TransformPoint(_meshPathCreator.Points[i].position);
                float dist = HandleUtility.DistanceToCircle(worldPos, 0.5f);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }

            if (closestDist < 1f)
            {
                _selectedPointIndex = closestIndex;
            }
            else
            {
                _selectedPointIndex = -1;
                _isDragging = false;
            }
        }

        private void DrawPoints()
        {
            for (int i = 0; i < _meshPathCreator.Points.Count; i++)
            {
                Vector3 worldPos = _meshPathCreator.transform.TransformPoint(_meshPathCreator.Points[i].position);
                float radius = _meshPathCreator.Points[i].radius;

                bool isSelected = (i == _selectedPointIndex);

                if (isSelected)
                {
                    Handles.color = Color.green;
                }
                else
                {
                    Handles.color = Color.yellow;
                }

                float handleSize = HandleUtility.GetHandleSize(worldPos) * 0.3f;
                float drawRadius = Mathf.Min(radius, handleSize);
                drawRadius = Mathf.Max(drawRadius, 0.2f);

                Handles.DrawWireDisc(worldPos, Vector3.up, drawRadius);
                
                if (i > 0)
                {
                    Vector3 prevWorldPos = _meshPathCreator.transform.TransformPoint(_meshPathCreator.Points[i - 1].position);
                    Handles.color = isSelected ? Color.green : new Color(1f, 1f, 0f, 0.5f);
                    Handles.DrawLine(prevWorldPos, worldPos);
                }
            }

            if (_meshPathCreator.Closed && _meshPathCreator.Points.Count > 1)
            {
                Handles.color = new Color(1f, 1f, 0f, 0.5f);
                Vector3 lastWorldPos = _meshPathCreator.transform.TransformPoint(_meshPathCreator.Points[_meshPathCreator.Points.Count - 1].position);
                Vector3 firstWorldPos = _meshPathCreator.transform.TransformPoint(_meshPathCreator.Points[0].position);
                Handles.DrawLine(lastWorldPos, firstWorldPos);
            }

            if (_meshPathCreator.Points.Count > 0)
            {
                string label = $"Points: {_meshPathCreator.Points.Count}";
                if (_selectedPointIndex >= 0)
                {
                    label += $" | Selected: {_selectedPointIndex}";
                }
                Vector3 firstWorldPos = _meshPathCreator.transform.TransformPoint(_meshPathCreator.Points[0].position);
                Handles.Label(firstWorldPos + Vector3.up * 2f, label);
            }
        }

        private void HandleSelectedPoint()
        {
            if (_selectedPointIndex < 0 || _selectedPointIndex >= _meshPathCreator.Points.Count)
                return;

            var point = _meshPathCreator.Points[_selectedPointIndex];
            Vector3 worldPos = _meshPathCreator.transform.TransformPoint(point.position);
            Event e = Event.current;

            if (_isDragging && e.type == EventType.MouseDrag)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(Vector3.up, worldPos);
                
                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    hitPoint.y = worldPos.y;
                    point.position = _meshPathCreator.transform.InverseTransformPoint(hitPoint);
                    _meshPathCreator.UpdatePoint(_selectedPointIndex, point);
                    e.Use();
                }
            }

            worldPos = _meshPathCreator.transform.TransformPoint(point.position);
            Handles.color = _isDragging ? Color.green : Color.cyan;
            Handles.DrawWireDisc(worldPos, Vector3.up, point.radius);
        }
    }
}
#endif
