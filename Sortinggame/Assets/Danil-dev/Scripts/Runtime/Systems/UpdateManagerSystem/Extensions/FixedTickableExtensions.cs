using System.Collections.Generic;

namespace D_Dev.UpdateManagerSystem
{
    public static class FixedTickableExtensions
    {
        private static readonly Dictionary<IFixedTickable, int> _priorities = new();
        
        public static int GetPriority(this IFixedTickable tickable)
        {
            return _priorities.TryGetValue(tickable, out var priority) ? priority : 0;
        }
        
        public static void SetPriority(this IFixedTickable tickable, int priority)
        {
            if (tickable != null)
                _priorities[tickable] = priority;
        }
    }
}