using UnityEngine;

namespace D_Dev.MovementHandler
{
    public class AutonomousMovementController : BaseMovementController
    {
        #region Monobehaviour

        protected virtual void Update() => PerformUpdate();
        protected virtual void FixedUpdate() => PerformFixedUpdate();

        #endregion
    }
}
