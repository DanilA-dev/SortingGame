using System.Linq;
using D_Dev.EntityVariable;
using D_Dev.PolymorphicValueSystem;
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

        public static bool TryGetVariableValue<T>(this EntityInfo entityInfo,
            StringScriptableVariable id, out T value)
        {
            value = default;
            if (entityInfo == null || id == null)
                return false;

            foreach (var variable in entityInfo.Variables)
            {
                if (variable == null || variable.VariableID != id)
                    continue;

                switch (variable.GetValueRaw())
                {
                    case T rawValue:
                        value = rawValue;
                        return true;
                    case PolymorphicValue<T> polymorphicValue:
                        value = polymorphicValue.Value;
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

    }
}