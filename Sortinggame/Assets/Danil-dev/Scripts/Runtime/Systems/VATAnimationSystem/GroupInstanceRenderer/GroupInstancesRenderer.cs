using System.Collections.Generic;
using D_Dev.Singleton;
using UnityEngine;
using UnityEngine.Rendering;

namespace D_Dev.VATAnimationSystem.GroupInstanceRenderer
{
    public class GroupInstancesRenderer : BaseSingleton<GroupInstancesRenderer>
    {
        #region Classes

        public class VATInstance
        {
            public Matrix4x4 Matrix;
            public VertexAnimationClipInfo Clip;
            public float Time;
            public bool Playing;
        }

        public class SimpleMeshInstance
        {
            public Matrix4x4 Matrix;
        }

        public class FillBarInstance
        {
            public Matrix4x4 Matrix;
            public float Fill;
            public float DelayedFill;
        }
        
        private class VATGroup
        {
            public VertexAnimationData Data;
            public Material Material;
            public List<VATInstance> Instances = new();
            public Matrix4x4[] MatrixBuffer = new Matrix4x4[1023];
            public float[] FrameBuffer = new float[1023];
        }

        private class SimpleMeshGroup
        {
            public Mesh Mesh;
            public Material Material;
            public List<SimpleMeshInstance> Instances = new();
            public Matrix4x4[] MatrixBuffer = new Matrix4x4[1023];
        }

        private class FillBarGroup
        {
            public Mesh Mesh;
            public Material Material;
            public List<FillBarInstance> Instances = new();
            public Matrix4x4[] MatrixBuffer = new Matrix4x4[1023];
            public float[] FillBuffer = new float[1023];
            public float[] DelayedFillBuffer = new float[1023];
        }
        
        #endregion

        #region Fields

        [SerializeField] private int _layer;
        [SerializeField] private ShadowCastingMode _shadowCasting = ShadowCastingMode.Off;
        [SerializeField] private bool _receiveShadows;

        private readonly Dictionary<VertexAnimationData, VATGroup> _groups = new();
        private readonly Dictionary<(Mesh, Material), SimpleMeshGroup> _staticGroups = new();
        private readonly Dictionary<(Mesh, Material), FillBarGroup> _fillBarGroups = new();
        
        private MaterialPropertyBlock _props;
        private MaterialPropertyBlock _staticProps;
        private MaterialPropertyBlock _fillBarProps;

        private static readonly int CurrentFrameId = Shader.PropertyToID("_CurrentFrame");
        private static readonly int TotalFramesId = Shader.PropertyToID("_TotalFrames");
        
        private static readonly int FillId = Shader.PropertyToID("_Fill");
        private static readonly int DelayedFillId = Shader.PropertyToID("_DelayedFill");

        private const int BatchSize = 1023;

        #endregion

        #region Monobehaviour

        protected override void Awake()
        {
            base.Awake();
            _props = new MaterialPropertyBlock();
            _staticProps = new MaterialPropertyBlock();
            _fillBarProps = new MaterialPropertyBlock();
        }

        private void Update()
        {
            foreach (var group in _groups.Values)
            {
                if (group.Instances.Count > 0)
                    DrawGroup(group);
            }

            foreach (var group in _staticGroups.Values)
            {
                if (group.Instances.Count > 0)
                    DrawStaticGroup(group);
            }

            foreach (var group in _fillBarGroups.Values)
            {
                if (group.Instances.Count > 0)
                    DrawFillBarGroup(group);
            }
        }

        #endregion

        #region Public VAT

        public VATInstance AddUnit(VertexAnimationData data, Material material, Matrix4x4 matrix)
        {
            if (!_groups.TryGetValue(data, out var group))
            {
                group = new VATGroup { Data = data, Material = material };
                material.SetFloat(TotalFramesId, data.TotalFrames);
                _groups[data] = group;
            }

            var instance = new VATInstance { Matrix = matrix, Playing = true };
            group.Instances.Add(instance);
            return instance;
        }

        public void AddExistingUnit(VertexAnimationData data, VATInstance instance)
        {
            if (!_groups.TryGetValue(data, out var group))
                return;
            
            if (!group.Instances.Contains(instance))
                group.Instances.Add(instance);
        }

        public void RemoveUnit(VertexAnimationData data, VATInstance instance)
        {
            if (_groups.TryGetValue(data, out var group))
                group.Instances.Remove(instance);
        }

        #endregion

        #region Public Static

        public SimpleMeshInstance AddStaticInstance(Mesh mesh, Material material, Matrix4x4 matrix)
        {
            var key = (mesh, material);
            if (!_staticGroups.TryGetValue(key, out var group))
            {
                group = new SimpleMeshGroup { Mesh = mesh, Material = material };
                _staticGroups[key] = group;
            }

            var instance = new SimpleMeshInstance { Matrix = matrix };
            group.Instances.Add(instance);
            return instance;
        }

        public void RemoveStaticInstance(Mesh mesh, Material material, SimpleMeshInstance instance)
        {
            var key = (mesh, material);
            if (_staticGroups.TryGetValue(key, out var group))
                group.Instances.Remove(instance);
        }

        public void AddExistingStaticInstance(Mesh mesh, Material material, SimpleMeshInstance instance)
        {
            var key = (mesh, material);
            if (!_staticGroups.TryGetValue(key, out var group))
            {
                group = new SimpleMeshGroup { Mesh = mesh, Material = material };
                _staticGroups[key] = group;
            }

            if (!group.Instances.Contains(instance))
                group.Instances.Add(instance);
        }

        #endregion

        #region Public Fill Bar

        public FillBarInstance AddFillBarInstance(Mesh mesh, Material material, Matrix4x4 matrix)
        {
            var key = (mesh, material);
            if (!_fillBarGroups.TryGetValue(key, out var group))
            {
                group = new FillBarGroup { Mesh = mesh, Material = material };
                _fillBarGroups[key] = group;
            }

            var instance = new FillBarInstance { Matrix = matrix, Fill = 1f, DelayedFill = 1f };
            group.Instances.Add(instance);
            return instance;
        }

        public void RemoveFillBarInstance(Mesh mesh, Material material, FillBarInstance instance)
        {
            var key = (mesh, material);
            if (_fillBarGroups.TryGetValue(key, out var group))
                group.Instances.Remove(instance);
        }

        public void AddExistingFillBarInstance(Mesh mesh, Material material, FillBarInstance instance)
        {
            var key = (mesh, material);
            if (!_fillBarGroups.TryGetValue(key, out var group))
            {
                group = new FillBarGroup { Mesh = mesh, Material = material };
                _fillBarGroups[key] = group;
            }

            if (!group.Instances.Contains(instance))
                group.Instances.Add(instance);
        }

        #endregion

        #region Public General

        public void Clear()
        {
            _groups.Clear();
            _staticGroups.Clear();
            _fillBarGroups.Clear();
        }

        #endregion

        #region Private Draw Methods

        private void DrawGroup(VATGroup group)
        {
            int total = group.Instances.Count;
            int offset = 0;

            while (offset < total)
            {
                int count = Mathf.Min(BatchSize, total - offset);

                for (int i = 0; i < count; i++)
                {
                    var unit = group.Instances[offset + i];
                    group.MatrixBuffer[i] = unit.Matrix;
                    group.FrameBuffer[i] = CalculateFrame(unit);
                }

                _props.SetFloat(TotalFramesId, group.Data.TotalFrames);
                _props.SetFloatArray(CurrentFrameId, group.FrameBuffer);

                Graphics.DrawMeshInstanced(
                    group.Data.BaseMesh, 0, group.Material,
                    group.MatrixBuffer, count, _props,
                    _shadowCasting, _receiveShadows, _layer);

                offset += count;
            }
        }

        private void DrawStaticGroup(SimpleMeshGroup group)
        {
            int total = group.Instances.Count;
            int offset = 0;

            while (offset < total)
            {
                int count = Mathf.Min(BatchSize, total - offset);

                for (int i = 0; i < count; i++)
                    group.MatrixBuffer[i] = group.Instances[offset + i].Matrix;

                Graphics.DrawMeshInstanced(
                    group.Mesh, 0, group.Material,
                    group.MatrixBuffer, count, _staticProps,
                    _shadowCasting, _receiveShadows, _layer);

                offset += count;
            }
        }

        private void DrawFillBarGroup(FillBarGroup group)
        {
            int total = group.Instances.Count;
            int offset = 0;

            while (offset < total)
            {
                int count = Mathf.Min(BatchSize, total - offset);

                for (int i = 0; i < count; i++)
                {
                    var inst = group.Instances[offset + i];
                    group.MatrixBuffer[i] = inst.Matrix;
                    group.FillBuffer[i] = inst.Fill;
                    group.DelayedFillBuffer[i] = inst.DelayedFill;
                }

                _fillBarProps.SetFloatArray(FillId, group.FillBuffer);
                _fillBarProps.SetFloatArray(DelayedFillId, group.DelayedFillBuffer);

                Graphics.DrawMeshInstanced(
                    group.Mesh, 0, group.Material,
                    group.MatrixBuffer, count, _fillBarProps,
                    _shadowCasting, _receiveShadows, _layer);

                offset += count;
            }
        }

        private float CalculateFrame(VATInstance unit)
        {
            if (unit.Clip == null)
                return 0f;
            
            float t = unit.Clip.Duration > 0f ? unit.Time / unit.Clip.Duration : 0f;
            return unit.Clip.StartFrame + t * (unit.Clip.FrameCount - 1);
        }

        #endregion
    }
}