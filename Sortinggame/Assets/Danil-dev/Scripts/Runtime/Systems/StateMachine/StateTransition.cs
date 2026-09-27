using System;

namespace D_Dev.StateMachine
{
    public class StateTransition
    {
        public string ToState { get; set; }
        public IStateCondition Condition { get; }

        public StateTransition(string toState, IStateCondition condition)
        {
            ToState = toState;
            Condition = condition;
        }
    }
}
