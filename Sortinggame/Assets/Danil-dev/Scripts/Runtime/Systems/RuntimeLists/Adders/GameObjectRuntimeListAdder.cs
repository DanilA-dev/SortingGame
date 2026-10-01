using UnityEngine;

namespace D_Dev.RuntimeLists.Adders
{
    public class GameObjectRuntimeListAdder : BaseRuntimeListAdder<GameObject, GameObjectRuntimeList>
    {
        protected override GameObject GetDefaultItem() => gameObject;
    }
}
