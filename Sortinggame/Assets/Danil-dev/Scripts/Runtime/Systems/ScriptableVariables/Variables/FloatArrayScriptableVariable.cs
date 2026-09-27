using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/FloatArrayVariable")]
    public class FloatArrayScriptableVariable : BaseScriptableVariable<float[]>
    {
        public override void ResetValue() => Value = null;
    }

}