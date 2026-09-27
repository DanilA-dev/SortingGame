using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.CustomEventManager
{
    [System.Serializable]
    public class StringVariableEventNameType : BaseEventNameType
    {
        #region Fields

        [SerializeField] private StringScriptableVariable _variable;

        #endregion
        
        #region Properties
        
        public override string EventName
        {
            get
            {
                if(_variable == null)
                    return "";
                
                return string.IsNullOrEmpty(_variable.Value) ? _variable.name : _variable.Value;
            }
        }
        
        #endregion
    }
}