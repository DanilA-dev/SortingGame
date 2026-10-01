using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeLists.Extensions
{
    [System.Serializable]
    public sealed class TransformRuntimeListValue : RuntimeListArrayValue<TransformRuntimeList, Transform>
    {
        #region Cloning

        public override PolymorphicValue<Transform[]> Clone()
        {
            return new TransformRuntimeListValue { _list = _list };
        }

        #endregion
    }
}
