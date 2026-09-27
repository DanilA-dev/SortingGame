using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/FloatVariable")]
    public class FloatScriptableVariable : BaseScriptableVariable<float>
    {
        public override void ResetValue() => Value = 0f;
    }
   
}