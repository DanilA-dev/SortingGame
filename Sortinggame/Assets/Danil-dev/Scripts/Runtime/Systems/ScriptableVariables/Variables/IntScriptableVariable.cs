using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/IntVariable")]
    public class IntScriptableVariable : BaseScriptableVariable<int>
    {
        public override void ResetValue() => Value = 0;
    }
   
}