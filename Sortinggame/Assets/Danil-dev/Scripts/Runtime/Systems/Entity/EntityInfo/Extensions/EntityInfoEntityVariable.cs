using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;

namespace D_Dev.Entity.Extensions
{
    [System.Serializable]
    public class EntityInfoEntityVariable : EntityVariable<EntityInfo>
    {
        #region Constructors

        public EntityInfoEntityVariable() {}
        
        public EntityInfoEntityVariable(StringScriptableVariable id, EntityInfo value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new EntityInfoEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}