using System.Collections.Generic;

namespace D_Dev.Extensions
{
    public static class ListExtensions
    {
        public static bool TryRemove<T>(this IList<T> list, T type)
        {
            if (!list.Contains(type))
                return false;
            
            list.Remove(type);
            return true;
        }
    }
}
