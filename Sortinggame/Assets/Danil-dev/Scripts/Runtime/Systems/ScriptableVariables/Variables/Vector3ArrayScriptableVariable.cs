using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/Vector3ArrayVariable")]
    public class Vector3ArrayScriptableVariable : BaseScriptableVariable<Vector3[]>
    {
        public override void ResetValue() => Value = null;
    }

}