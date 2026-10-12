using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class SpriteValue : PolymorphicValue<Sprite> { }

    [System.Serializable]
    public sealed class SpriteConstantValue : ConstantValue<Sprite>
    {
        #region Cloning

        public override PolymorphicValue<Sprite> Clone()
        {
            return new SpriteConstantValue { _value = _value };
        }

        #endregion
    }
}
