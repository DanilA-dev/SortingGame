using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class ColliderValue : PolymorphicValue<Collider> {}

    [System.Serializable]
    public class ColliderConstantValue : ConstantValue<Collider>
    {
        #region Clone

        public override PolymorphicValue<Collider> Clone()
        {
            return new ColliderConstantValue() { _value = _value };
        }

        #endregion
    }
}