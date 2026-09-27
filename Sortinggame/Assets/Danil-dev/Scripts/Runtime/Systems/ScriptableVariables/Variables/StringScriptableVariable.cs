using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/StringVariable")]
    public class StringScriptableVariable : BaseScriptableVariable<string>
    {
        public override void ResetValue() => Value = null;
    }
    
}
