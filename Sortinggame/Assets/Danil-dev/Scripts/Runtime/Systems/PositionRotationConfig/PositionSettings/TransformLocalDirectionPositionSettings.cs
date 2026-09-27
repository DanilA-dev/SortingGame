using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PositionRotationConfig
{
    [Serializable]
    public class TransformLocalDirectionPositionSettings : BasePositionSettings
    {
        #region Enums

        public enum LocalDirection
        {
            Up = 0,
            Down = 1,
            Right = 2,
            Left = 3,
            Forward = 4,
            Back = 5
        }

        #endregion
        
        #region Fields

        [SerializeReference] private PolymorphicValue<Transform> _value = new TransformConstantValue();
        [SerializeField] private LocalDirection _direction;

        

        #endregion

        #region Properties

        public PolymorphicValue<Transform> Value
        {
            get => _value;
            set => _value = value;
        }

        public LocalDirection Direction
        {
            get => _direction;
            set => _direction = value;
        }

        #endregion
        
        #region Overrides

        public override Vector3 OnGetPosition()
        {
            if (_value?.Value == null)
            {
                Debug.Log($"[PositionSettings] Value is null, reset to Vector.zero");
                return Vector3.zero;
            }
            
            return _direction switch
            {
                LocalDirection.Up => _value.Value.up,
                LocalDirection.Down => -_value.Value.up,
                LocalDirection.Right => _value.Value.right,
                LocalDirection.Left => -_value.Value.right,
                LocalDirection.Forward => _value.Value.forward,
                LocalDirection.Back => -_value.Value.forward,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion
    }
}