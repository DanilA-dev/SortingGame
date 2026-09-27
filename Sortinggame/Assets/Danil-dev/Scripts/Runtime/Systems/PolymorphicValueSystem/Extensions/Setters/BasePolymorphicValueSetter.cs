using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Setters
{
    public abstract class BasePolymorphicValueSetter<T> : MonoBehaviour
    {
        #region Fields

        [SerializeField] protected bool _setOnStart;
        [SerializeReference] protected PolymorphicValue<T> _valueToSet;
        [SerializeReference] protected PolymorphicValue<T> _resultValue;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if(_setOnStart)
                SetValue();
        }

        #endregion
        
        #region Pulic

        public void SetValue()
        {
            if(_valueToSet.Value == null ||
               _resultValue.Value == null)
                return;
            
            _valueToSet.Value = _resultValue.Value;
        }

        public void SetValue(T newValue)
        {
            if(_valueToSet == null)
                return;

            _valueToSet.Value = newValue;
        }

        #endregion
    }
}