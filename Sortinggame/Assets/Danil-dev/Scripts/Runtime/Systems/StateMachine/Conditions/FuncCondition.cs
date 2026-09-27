using System;

namespace D_Dev.StateMachine
{
    public class FuncCondition : IStateCondition
    {
        private Func<bool> _func;

        public FuncCondition(Func<bool> func)
        {
            _func = func;
        }
        public bool IsMatched() => _func.Invoke();
    }
}
