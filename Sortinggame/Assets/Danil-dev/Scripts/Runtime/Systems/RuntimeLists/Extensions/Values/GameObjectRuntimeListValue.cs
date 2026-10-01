using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeLists.Extensions
{
    [System.Serializable]
    public sealed class GameObjectRuntimeListValue : RuntimeListArrayValue<GameObjectRuntimeList, GameObject>
    {
        #region Cloning

        public override PolymorphicValue<GameObject[]> Clone()
        {
            return new GameObjectRuntimeListValue { _list = _list };
        }

        #endregion
    }
}
