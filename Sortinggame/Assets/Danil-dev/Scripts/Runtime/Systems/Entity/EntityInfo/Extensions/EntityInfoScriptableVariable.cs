using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.Entity.Extensions
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/EntityInfoVariable")]
    public class EntityInfoScriptableVariable : BaseScriptableVariable<EntityInfo>
    {
        public override void ResetValue() => Value = null;
    }
}
