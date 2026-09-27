namespace  D_Dev.StateMachine
{
    public interface IState
    {
        public float ExitTime { get; }
        public void OnEnter();
        public void OnUpdate();
        public void OnFixedUpdate();
        public void OnExit();
    }
}