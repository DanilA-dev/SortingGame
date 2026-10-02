using System.Collections.Generic;
using D_Dev.Singleton;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Scripts
{
    public class InstancedItemsRenderer : BaseSingleton<InstancedItemsRenderer>
    {
        #region Classes

        private class Batch
        {
            public readonly List<Transform> Transforms = new();
            public Matrix4x4[] Matrices = new Matrix4x4[64];
            public RenderParams Params;
            public Mesh Mesh;
            public bool IsDirty;
        }

        #endregion

        #region Fields

        private const int MaxInstancesPerDraw = 1023;

        [SerializeField] private Bounds _drawBounds = new(Vector3.zero, Vector3.one * 200f);

        private readonly Dictionary<(Mesh, Material, uint), Batch> _batches = new();
        private readonly Dictionary<Transform, Batch> _registered = new();

        #endregion

        #region Monobehaviour

        private void LateUpdate()
        {
            foreach (var batch in _batches.Values)
            {
                if (batch.Transforms.Count == 0)
                    continue;

                RefreshMatrices(batch);

                for (int start = 0; start < batch.Transforms.Count; start += MaxInstancesPerDraw)
                {
                    int count = Mathf.Min(MaxInstancesPerDraw, batch.Transforms.Count - start);
                    Graphics.RenderMeshInstanced(batch.Params, batch.Mesh, 0, batch.Matrices, count, start);
                }
            }
        }

        #endregion

        #region Public

        public bool Register(MeshRenderer meshRenderer, MeshFilter meshFilter)
        {
            var target = meshRenderer.transform;
            if (_registered.ContainsKey(target))
                return true;

            var mesh = meshFilter.sharedMesh;
            var material = meshRenderer.sharedMaterial;
            if (mesh == null || material == null || !material.enableInstancing)
                return false;

            var renderingLayerMask = meshRenderer.renderingLayerMask;
            var key = (mesh, material, renderingLayerMask);
            if (!_batches.TryGetValue(key, out var batch))
            {
                batch = new Batch
                {
                    Mesh = mesh,
                    Params = new RenderParams(material)
                    {
                        layer = target.gameObject.layer,
                        renderingLayerMask = renderingLayerMask,
                        shadowCastingMode = ShadowCastingMode.Off,
                        receiveShadows = false,
                        worldBounds = _drawBounds
                    }
                };
                _batches.Add(key, batch);
            }

            batch.Transforms.Add(target);
            batch.IsDirty = true;
            _registered.Add(target, batch);
            meshRenderer.enabled = false;
            return true;
        }

        public void Unregister(MeshRenderer meshRenderer)
        {
            var target = meshRenderer.transform;
            if (!_registered.Remove(target, out var batch))
                return;

            int index = batch.Transforms.IndexOf(target);
            int last = batch.Transforms.Count - 1;
            batch.Transforms[index] = batch.Transforms[last];
            batch.Transforms.RemoveAt(last);
            batch.IsDirty = true;
            meshRenderer.enabled = true;
        }

        #endregion

        #region Private

        private void RefreshMatrices(Batch batch)
        {
            int count = batch.Transforms.Count;
            if (batch.Matrices.Length < count)
            {
                System.Array.Resize(ref batch.Matrices, Mathf.NextPowerOfTwo(count));
                batch.IsDirty = true;
            }

            for (int i = 0; i < count; i++)
            {
                var t = batch.Transforms[i];
                if (!batch.IsDirty && !t.hasChanged)
                    continue;

                batch.Matrices[i] = t.localToWorldMatrix;
                t.hasChanged = false;
            }

            batch.IsDirty = false;
        }

        #endregion
    }
}
