using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ScriptableVariables.Listeners
{
    public abstract class BaseScriptableVariableListener<T,TVariable> : MonoBehaviour
    where TVariable : BaseScriptableVariable<T>
    {
        #region Fields

        [SerializeField] private TVariable _variable;
        
        [FoldoutGroup("Events")]
        public UnityEvent<T> OnValueChanged;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if(_variable == null)
                return;
            
            _variable.OnValueUpdate += OnVariableValueChanged;
        }

        private void OnDestroy()
        {
            if(_variable == null)
                return;
            
            _variable.OnValueUpdate -= OnVariableValueChanged;
        }

        #endregion

        #region Listeners

        protected virtual void OnVariableValueChanged(T value)
        {
            OnValueChanged?.Invoke(value);
        }

        #endregion
    }
}
