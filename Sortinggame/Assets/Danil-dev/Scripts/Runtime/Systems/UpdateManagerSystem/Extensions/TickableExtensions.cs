using System.Collections.Generic;

namespace D_Dev.UpdateManagerSystem
{
    public static class TickableExtensions
    {
        private static readonly Dictionary<ITickable, int> _priorities = new();
        
        public static int GetPriority(this ITickable tickable)
        {
            return _priorities.TryGetValue(tickable, out var priority) ? priority : 0;
        }
        
        public static void SetPriority(this ITickable tickable, int priority)
        {
            if (tickable != null)
                _priorities[tickable] = priority;
        }
    }
}