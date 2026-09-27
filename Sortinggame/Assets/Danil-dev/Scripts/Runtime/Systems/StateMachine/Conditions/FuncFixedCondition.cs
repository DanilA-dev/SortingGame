namespace D_Dev.StateMachine
{
    public class FuncFixedCondition : IFixedStateCondition
    {
        private readonly System.Func<bool> _condition;

        public FuncFixedCondition(System.Func<bool> condition)
        {
            _condition = condition;
        }

        public bool IsMatched() => _condition?.Invoke() ?? false;

    }
}
