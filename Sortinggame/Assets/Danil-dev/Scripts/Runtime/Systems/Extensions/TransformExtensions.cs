using UnityEngine;

namespace D_Dev.Extensions
{
    public static class TransformExtensions
    {
        public static void RotateTowardsDirection(this Transform owner, Vector3 target, Vector3 axis, float speed,
            bool constrainX = false, bool constrainY = false, bool constrainZ = false)
        {
            var dir = (target - owner.position).normalized;
            Quaternion look = Quaternion.LookRotation(dir, axis);
            look.x = constrainX ? 0 : look.x;
            look.y = constrainY ? 0 : look.y;
            look.z = constrainZ ? 0 : look.z;
            owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, look, speed);
            
        }
        
        public static void RotateTowards(this Transform owner, Vector3 target, Vector3 axis, float speed,
            bool constrainX = false, bool constrainY = false, bool constrainZ = false)
        {
            Quaternion look = Quaternion.LookRotation(target, axis);
            look.x = constrainX ? 0 : look.x;
            look.y = constrainY ? 0 : look.y;
            look.z = constrainZ ? 0 : look.z;
            owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, look, speed);
            
        }
    }
}
