using System;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable
{
    [System.Serializable]
    public abstract class BaseEntityVariable
    {
        #region Fields

        [SerializeField] protected StringScriptableVariable _variableID;
        
        #endregion

        #region Properties

        public StringScriptableVariable VariableID => _variableID;

        #endregion

        #region Abstract Methods

        public abstract object GetValueRaw();
        public abstract void SetValueRaw(object value);
        public abstract BaseEntityVariable Clone();

        #endregion
    }
}
