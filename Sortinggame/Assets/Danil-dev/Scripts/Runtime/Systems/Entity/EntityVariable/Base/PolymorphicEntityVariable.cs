using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public abstract class PolymorphicEntityVariable<T> : BaseEntityVariable 
    {
        #region Fields

        [SerializeReference] protected T _value;

        #endregion

        #region Properties

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        #endregion

        #region Constructor

        protected PolymorphicEntityVariable() {}
        
        protected PolymorphicEntityVariable(T value) { _value = value; }

        protected PolymorphicEntityVariable(StringScriptableVariable variableID, T value)
        {
            _variableID = variableID;
            _value = value;
        }

        #endregion
        
        #region Overrides

        public override object GetValueRaw()
        {
            return Value;
        }

        public override void SetValueRaw(object value)
        {
            Value = (T)value;
        }

        #endregion
    }
}
