namespace D_Dev.StateMachine
{
    public class FixedStateTransition
    {
        public string ToState { get; set; }
        public IFixedStateCondition Condition { get; }

        public FixedStateTransition(string toState, IFixedStateCondition condition)
        {
            ToState = toState;
            Condition = condition;
        }
    }
}
