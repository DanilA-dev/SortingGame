using D_Dev.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Actions
{
    public abstract class WaitUntilValueAction<TCondition> : BaseAction where TCondition : ICondition, new()
    {
        #region Fields

        [SerializeField, InlineProperty, HideLabel] protected TCondition _condition = new();

        #endregion

        #region Overrides

        public override void Execute()
        {
            if (_condition != null && _condition.IsConditionMet())
                IsFinished = true;
        }

        public override void Undo()
        {
            _condition?.Reset();
            base.Undo();
        }

        #endregion
    }
}
