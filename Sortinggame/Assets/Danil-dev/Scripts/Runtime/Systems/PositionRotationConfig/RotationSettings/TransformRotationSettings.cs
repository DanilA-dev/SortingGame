using System;
using UnityEngine;

namespace D_Dev.PositionRotationConfig.RotationSettings
{
    [Serializable]
    public class TransformRotationSettings : BaseRotationSettings
    {
        #region Fields

        [SerializeField] private Transform _value;
        [SerializeField] private bool _isLocal;

        #endregion

        #region Overrides

        public override Quaternion GetRotation() => _isLocal 
            ? _value.localRotation 
            : _value.rotation;

        #endregion
    }
}