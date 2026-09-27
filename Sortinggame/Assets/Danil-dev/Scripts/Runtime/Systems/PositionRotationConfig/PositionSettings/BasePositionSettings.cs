using System;
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

    #endregion
    
    [Serializable]
    public class BasePositionSettings
    {
        #region Fields

        [Title("Random")]
        [SerializeField] private bool _useRandomSphere;
        [ShowIf(nameof(_useRandomSphere))]
        [SerializeField] private float _radius;
        [ShowIf(nameof(_useRandomSphere))] 
        [SerializeField] private AxisUpdate _axis = AxisUpdate.All;

        #endregion

        #region Properties

        public bool UseRandomSphere
        {
            get => _useRandomSphere;
            set => _useRandomSphere = value;
        }

        public float RandomRadius
        {
            get => _radius;
            set => _radius = value;
        }

        public AxisUpdate Axis
        {
            get => _axis;
            set => _axis = value;
        }

        #endregion
        
        #region Public

        public Vector3 GetPosition()
        {
            Vector3 randomOffset = Random.insideUnitSphere * _radius;
            randomOffset.y = _axis.HasFlag(AxisUpdate.Y) ? randomOffset.y : 0;
            randomOffset.x = _axis.HasFlag(AxisUpdate.X) ? randomOffset.x : 0;
            randomOffset.z = _axis.HasFlag(AxisUpdate.Z) ? randomOffset.z : 0;

            if (_useRandomSphere)
                return OnGetPosition() + randomOffset;
                
            return OnGetPosition();
        }

        #endregion

        #region Virtual

        public virtual Vector3 OnGetPosition() => Vector3.zero;

        #endregion
    }
}