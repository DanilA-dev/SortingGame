namespace D_Dev.DebugConsole
{
    public interface IDebugCommand
    {
        public string Command { get; }
        public void Register();
        public void Unregister();
    }
}
