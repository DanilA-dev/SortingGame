using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace D_Dev.PositionRotationConfig.RotationSettings
{
    [Serializable]
    public class RandomRotationSettings : BaseRotationSettings
    {
        #region Fields

        [SerializeField] private AxisUpdate _axis = AxisUpdate.All;

        #endregion

        #region Properties

        public AxisUpdate Axis
        {
            get => _axis;
            set => _axis = value;
        }

        #endregion

        #region Overrides

        public override Quaternion GetRotation() => Quaternion.Euler(
            _axis.HasFlag(AxisUpdate.X) ? Random.Range(0f, 360f) : 0f,
            _axis.HasFlag(AxisUpdate.Y) ? Random.Range(0f, 360f) : 0f,
            _axis.HasFlag(AxisUpdate.Z) ? Random.Range(0f, 360f) : 0f);

        #endregion
    }
}
