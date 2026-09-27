using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class ColliderEntityVariable : EntityVariable<Collider>
    {
        #region Constructors

        public ColliderEntityVariable() {}
        
        public ColliderEntityVariable(StringScriptableVariable id, Collider value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new ColliderEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}