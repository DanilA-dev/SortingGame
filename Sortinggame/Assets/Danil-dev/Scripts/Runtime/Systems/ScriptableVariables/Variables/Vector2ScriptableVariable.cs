using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/Vector2Variable")]
    public class Vector2ScriptableVariable : BaseScriptableVariable<Vector2>
    {
        public override void ResetValue() => Value = Vector2.zero;
    }
}