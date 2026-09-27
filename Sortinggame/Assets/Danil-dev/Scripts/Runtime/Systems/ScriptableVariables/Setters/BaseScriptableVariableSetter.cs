using UnityEngine;

namespace D_Dev.ScriptableVariables.Setters
{
    public class BaseScriptableVariableSetter<T, TVariable> : MonoBehaviour where TVariable : BaseScriptableVariable<T>
    {
        #region Fields

        [SerializeField] protected TVariable _variable;
        [SerializeField] protected T _value;
        [Space]
        [SerializeField] protected bool _setOnAwake;
        [SerializeField] protected bool _setOnStart;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_setOnAwake)
                SetValue(_value);
        }

        private void Start()
        {
            if (_setOnStart)
                SetValue(_value);
        }

        #endregion
        
        #region Public

        public void SetValue(T value)
        {
            if(_variable == null)
                return;

            if(value == null)
                return;

            _variable.Value = value;
        }

        #endregion
    }
}