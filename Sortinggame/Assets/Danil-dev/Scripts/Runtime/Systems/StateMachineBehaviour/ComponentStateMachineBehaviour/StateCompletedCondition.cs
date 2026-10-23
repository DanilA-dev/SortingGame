using D_Dev.Base;
using UnityEngine;

namespace D_Dev.StateMachineBehaviour
{
    [System.Serializable]
    public class StateCompletedCondition : ICondition
    {
        #region Fields

        [SerializeField] private BaseComponentState _state;

        #endregion

        #region ICondition

        public bool IsConditionMet() => _state is ICompletableState completableState && completableState.IsCompleted;

        public void Reset() {}

        #endregion
    }
}
