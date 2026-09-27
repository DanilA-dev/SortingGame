using UnityEngine;

namespace D_Dev.ScriptableVariables
{
    [CreateAssetMenu(menuName = "D-Dev/Variables/GameObjectArrayVariable")]
    public class GameObjectArrayScriptableVariable : BaseScriptableVariable<GameObject[]>
    {
        public override void ResetValue() => Value = null;
    }

}