using UnityEngine;

namespace D_Dev.RuntimeLists.Adders
{
    public class TransformRuntimeListAdder : BaseRuntimeListAdder<Transform, TransformRuntimeList>
    {
        protected override Transform GetDefaultItem() => transform;
    }
}
