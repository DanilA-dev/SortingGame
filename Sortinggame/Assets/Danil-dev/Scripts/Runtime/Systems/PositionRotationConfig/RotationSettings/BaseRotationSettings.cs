using UnityEngine;

namespace D_Dev.PositionRotationConfig.RotationSettings
{
    public class BaseRotationSettings
    {
        public virtual Quaternion GetRotation() => Quaternion.identity;
    }
}