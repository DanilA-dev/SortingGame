using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/BoolVariable")]
    public class BoolScriptableVariable : BaseScriptableVariable<bool>
    {
        public override void ResetValue() => Value = false;
    }
}
