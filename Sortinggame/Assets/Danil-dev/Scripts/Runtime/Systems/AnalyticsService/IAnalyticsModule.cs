using System.Collections.Generic;

namespace D_Dev.AnalyticsService
{
    public interface IAnalyticsModule
    {
        public bool IsInitialized { get; }
        public void Initialize();
        public void SendEvent(string eventName);
        public void SendEvent(string eventName, Dictionary<string, object> parameters);
    }
}
