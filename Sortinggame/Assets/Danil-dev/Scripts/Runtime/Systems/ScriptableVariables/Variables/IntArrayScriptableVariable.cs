using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/IntArrayVariable")]
    public class IntArrayScriptableVariable : BaseScriptableVariable<int[]>
    {
        public override void ResetValue() => Value = null;
    }

}