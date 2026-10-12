using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;

namespace D_Dev.Entity.Extensions
{
    [System.Serializable]
    public class EntityInfoArrayEntityVariable : EntityVariable<EntityInfo[]>
    {
        #region Constructors

        public EntityInfoArrayEntityVariable() {}

        public EntityInfoArrayEntityVariable(StringScriptableVariable id, EntityInfo[] value) : base(id, value) {}

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new EntityInfoArrayEntityVariable(_variableID, (EntityInfo[])_value?.Clone());
        }

        #endregion
    }
}
