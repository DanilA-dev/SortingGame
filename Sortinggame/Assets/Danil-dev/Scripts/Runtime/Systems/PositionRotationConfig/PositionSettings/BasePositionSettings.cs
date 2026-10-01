using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace D_Dev.PositionRotationConfig
{
    #region Enum

    [Flags]
    public enum AxisUpdate
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        All = X | Y | Z
    }

    public enum RandomPositionMode
    {
        None = 0,
        Sphere = 1,
        Box = 2
    }

    #endregion

    [Serializable]
    public class BasePositionSettings
    {
        #region Fields

        [Title("Random")]
        [SerializeField] private RandomPositionMode _randomMode;
        [ShowIf(nameof(_randomMode), RandomPositionMode.Sphere)]
        [SerializeField] private float _radius;
        [ShowIf(nameof(_randomMode), RandomPositionMode.Box)]
        [SerializeField] private Vector3 _boxSize = Vector3.one;
        [ShowIf(nameof(IsRandom))]
        [SerializeField] private AxisUpdate _axis = AxisUpdate.All;

        [ShowIf(nameof(IsRandom))]
        [SerializeField] private bool _drawGizmos = true;
        [ShowIf(nameof(ShowGizmoColor))]
        [SerializeField] private Color _gizmoColor = new(0f, 1f, 0.5f, 0.8f);

        #endregion

        #region Properties

        public RandomPositionMode RandomMode
        {
            get => _randomMode;
            set => _randomMode = value;
        }

        public float RandomRadius
        {
            get => _radius;
            set => _radius = value;
        }

        public Vector3 RandomBoxSize
        {
            get => _boxSize;
            set => _boxSize = value;
        }

        public AxisUpdate Axis
        {
            get => _axis;
            set => _axis = value;
        }

        public bool DrawGizmos
        {
            get => _drawGizmos;
            set => _drawGizmos = value;
        }

        public Color GizmoColor
        {
            get => _gizmoColor;
            set => _gizmoColor = value;
        }

        public bool IsRandom => _randomMode != RandomPositionMode.None;
        private bool ShowGizmoColor => IsRandom && _drawGizmos;

        #endregion

        #region Public

        public Vector3 GetPosition()
        {
            var position = OnGetPosition();

            return _randomMode switch
            {
                RandomPositionMode.Sphere => position + MaskAxis(Random.insideUnitSphere * _radius),
                RandomPositionMode.Box => position + MaskAxis(RandomInsideBox(_boxSize)),
                _ => position
            };
        }

        public void GetGizmoPositions(List<Vector3> positions) => OnGetGizmoPositions(positions);

        #endregion

        #region Private

        private static Vector3 RandomInsideBox(Vector3 size)
        {
            var extents = size * 0.5f;
            return new Vector3(
                Random.Range(-extents.x, extents.x),
                Random.Range(-extents.y, extents.y),
                Random.Range(-extents.z, extents.z));
        }

        private Vector3 MaskAxis(Vector3 offset)
        {
            offset.x = _axis.HasFlag(AxisUpdate.X) ? offset.x : 0;
            offset.y = _axis.HasFlag(AxisUpdate.Y) ? offset.y : 0;
            offset.z = _axis.HasFlag(AxisUpdate.Z) ? offset.z : 0;
            return offset;
        }

        #endregion

        #region Virtual

        public virtual Vector3 OnGetPosition() => Vector3.zero;

        protected virtual void OnGetGizmoPositions(List<Vector3> positions) => positions.Add(OnGetPosition());

        #endregion
    }
}
