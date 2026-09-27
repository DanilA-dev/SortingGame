using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/StringArrayVariable")]
    public class StringArrayScriptableVariable : BaseScriptableVariable<string[]>
    {
        public override void ResetValue() => Value = null;
    }

}