using UnityEngine;

namespace D_Dev.Extensions
{
    public static class ObjectExtensions
    {
        public static TComponent GetOrAdd<TComponent>(this GameObject go) where TComponent : Component
        {
            if (go.TryGetComponent(out TComponent comp))
                return comp;

            return go.AddComponent<TComponent>();
        }
        
        public static bool TryGetComponentInAny<TComponent>(this GameObject go, out TComponent outComponent)
            where TComponent : Component
        {
            if (go.TryGetComponent(out TComponent comp))
            {
                outComponent = comp;
                return true;
            }
            foreach (Transform child in go.transform)
            {
                if (child.TryGetComponent(out TComponent childComp))
                {
                    outComponent = childComp;
                    return true;
                }
            }

            if (go.transform.parent != null && go.transform.parent.TryGetComponent(out TComponent parentComp))
            {
                outComponent = parentComp;
                return true;
            }
            outComponent = null;
            return false;
        }
    }
}
