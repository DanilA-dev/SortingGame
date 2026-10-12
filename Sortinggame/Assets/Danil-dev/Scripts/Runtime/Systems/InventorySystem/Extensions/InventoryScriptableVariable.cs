using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.InventorySystem.Extensions
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/InventoryVariable")]
    public class InventoryScriptableVariable : BaseScriptableVariable<Inventory>
    {
        public override void ResetValue() => Value = null;
    }
}
