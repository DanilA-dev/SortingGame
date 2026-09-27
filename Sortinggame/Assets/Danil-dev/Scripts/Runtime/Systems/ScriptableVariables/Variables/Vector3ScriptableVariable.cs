using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/Vector3Variable")]
    public class Vector3ScriptableVariable : BaseScriptableVariable<Vector3>
    {
        public override void ResetValue() => Value = Vector3.zero;
    }
    
}
