using D_Dev.Base;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions
{
    [System.Serializable]
    public class RuntimeValueExists<T> : ICondition
    {
        #region Fields

        [SerializeReference] protected PolymorphicValue<T> _value;
        [SerializeField] protected bool _isExists;

        #endregion
        
        #region ICondition

        public virtual bool IsConditionMet()
        {
            var result = _value.Value!= null;
            return result == _isExists;
        }

        public virtual void Reset()
        {
        }

        #endregion
    }
}