using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/GameObjectVariable")]
    public class GameObjectScriptableVariable : BaseScriptableVariable<GameObject>
    {
        public override void ResetValue() => Value = null;
    }
    
}