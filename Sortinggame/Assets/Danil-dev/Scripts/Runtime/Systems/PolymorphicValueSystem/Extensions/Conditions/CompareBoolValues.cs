using D_Dev.Base;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions
{
    [System.Serializable]
    public class CompareBoolValues : ICondition
    {
        #region Fields

        [SerializeField] private bool _invert;
        [SerializeReference] private PolymorphicValue<bool> _value = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<bool> _compareTo = new BoolConstantValue();
        
        #endregion

        #region IConditions

        public bool IsConditionMet()
        {
            return _invert? !_value.Value.Equals(_compareTo.Value) : _value.Value.Equals(_compareTo.Value);
        }

        public void Reset() {}

        #endregion
    }
}