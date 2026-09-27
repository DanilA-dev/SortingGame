using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/BoolArrayVariable")]
    public class BoolArrayScriptableVariable : BaseScriptableVariable<bool[]>
    {
        public override void ResetValue() => Value = null;
    }

}