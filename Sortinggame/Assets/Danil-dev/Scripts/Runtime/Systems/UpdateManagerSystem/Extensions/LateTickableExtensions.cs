using System.Collections.Generic;

namespace D_Dev.UpdateManagerSystem
{
    public static class LateTickableExtensions
    {
        private static readonly Dictionary<ILateTickable, int> _priorities = new();
        
        public static int GetPriority(this ILateTickable tickable)
        {
            return _priorities.TryGetValue(tickable, out var priority) ? priority : 0;
        }
        
        public static void SetPriority(this ILateTickable tickable, int priority)
        {
            if (tickable != null)
                _priorities[tickable] = priority;
        }
    }
}