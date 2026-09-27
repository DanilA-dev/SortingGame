using System;
using D_Dev.Base;
using D_Dev.Conditions;
using D_Dev.StateMachine;

namespace D_Dev.StateMachineBehaviour
{
    public class CompositeTransitionCondition : IStateCondition, IFixedStateCondition
    {
        #region Fields

        private readonly ICondition[] _emptyConditions = Array.Empty<ICondition>();
        private readonly IFixedCondition[] _emptyFixedConditions = Array.Empty<IFixedCondition>();

        private readonly ICondition[] _conditions;
        private readonly IFixedCondition[] _fixedConditions;
        private readonly ConditionMatchMode _matchMode;
        private readonly Func<bool> _gate;

        #endregion

        #region Properties

        public ConditionMatchMode MatchMode => _matchMode;
        public bool HasConditions => _conditions.Length > 0;
        public bool HasFixedConditions => _fixedConditions.Length > 0;
        public bool IsEmpty => !HasConditions && !HasFixedConditions;

        #endregion

        #region Constructors

        public CompositeTransitionCondition(
            ICondition[] conditions,
            IFixedCondition[] fixedConditions,
            ConditionMatchMode matchMode,
            Func<bool> gate = null)
        {
            _conditions = conditions ?? _emptyConditions;
            _fixedConditions = fixedConditions ?? _emptyFixedConditions;
            _matchMode = matchMode;
            _gate = gate;
        }

        #endregion

        #region Public

        public void Reset()
        {
            for (int i = 0; i < _conditions.Length; i++)
                _conditions[i]?.Reset();

            for (int i = 0; i < _fixedConditions.Length; i++)
                _fixedConditions[i]?.Reset();
        }

        #endregion

        #region IStateCondition

        bool IStateCondition.IsMatched() => Evaluate();

        #endregion

        #region IFixedStateCondition

        bool IFixedStateCondition.IsMatched() => Evaluate();

        #endregion

        #region Private

        private bool Evaluate()
        {
            if (_gate != null && !_gate.Invoke())
                return false;

            if (IsEmpty)
                return false;

            return _matchMode == ConditionMatchMode.All ? MatchAll() : MatchAny();
        }

        private bool MatchAll()
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                var condition = _conditions[i];

                if (condition == null || !condition.IsConditionMet())
                    return false;
            }

            for (int i = 0; i < _fixedConditions.Length; i++)
            {
                var condition = _fixedConditions[i];

                if (condition == null || !condition.IsConditionMet())
                    return false;
            }

            return true;
        }

        private bool MatchAny()
        {
            for (int i = 0; i < _conditions.Length; i++)
            {
                var condition = _conditions[i];

                if (condition != null && condition.IsConditionMet())
                    return true;
            }

            for (int i = 0; i < _fixedConditions.Length; i++)
            {
                var condition = _fixedConditions[i];

                if (condition != null && condition.IsConditionMet())
                    return true;
            }

            return false;
        }

        #endregion
    }
}
