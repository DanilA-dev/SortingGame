using UnityEngine;

namespace D_Dev.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 With(this Vector3 currentVector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? currentVector.x, y ?? currentVector.y, z ?? currentVector.z);
        }
    }
}
