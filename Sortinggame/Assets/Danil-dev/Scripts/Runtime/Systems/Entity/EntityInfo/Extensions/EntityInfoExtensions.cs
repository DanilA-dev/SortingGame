using System.Linq;
using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;

namespace D_Dev.Entity.Extensions
{
    public static class EntityInfoExtensions
    {
        public static TVariable GetVariable<TVariable>(this EntityInfo entityInfo,
            StringScriptableVariable id)
            where TVariable : BaseEntityVariable
        {
            foreach (var variable in entityInfo.Variables)
            {
                if (variable.VariableID == id)
                    return variable as TVariable;
            }

            return null;
        }
        
        public static TVariable GetVariableFirst<TVariable>(this EntityInfo entityInfo)
            where TVariable : BaseEntityVariable
        {
            return entityInfo.Variables.           
                OfType<TVariable>().FirstOrDefault();
        }
        
    }
}