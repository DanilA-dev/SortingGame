using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    public class BaseScriptableVariable<T> : ScriptableObject
    {
        #region Fields

        [SerializeField] private T _value;
        [SerializeField] private bool _resetOnEnterRuntime;

        public event Action<T> OnValueUpdate;

        #endregion
        
        #region Properties

        public T Value
        {
            get => _value;
            set
            {
                if(EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                OnValueUpdate?.Invoke(_value);
            }
        }

        public bool ResetOnEnterRuntime => _resetOnEnterRuntime;

        #endregion
        
        #region Virtual
        
        [Button]
        public virtual void ResetValue() => Value = default;
        
        #endregion
    }
}
