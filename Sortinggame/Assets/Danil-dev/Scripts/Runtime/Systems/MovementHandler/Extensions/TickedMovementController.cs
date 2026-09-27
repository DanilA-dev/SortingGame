using UnityEngine;
using D_Dev.UpdateManagerSystem;

namespace D_Dev.MovementHandler
{
    public class TickedMovementController : BaseMovementController, ITickable, IFixedTickable
    {
        #region Fields

        [SerializeField] private int _priority;

        #endregion

        #region Monobehaviour

        protected override void Start()
        {
            base.Start();
            UpdateManager.AddWithPriority(this, _priority);
            FixedUpdateManager.AddWithPriority(this, _priority);
        }

        private void OnDestroy()
        {
            UpdateManager.Remove(this);
            FixedUpdateManager.Remove(this);
        }

        #endregion

        #region ITickable / IFixedTickable

        public void Tick() => PerformUpdate();
        public void FixedTick() => PerformFixedUpdate();

        #endregion

        #region Public

        public int GetPriority() => _priority;
        public void SetPriority(int priority) => _priority = priority;

        #endregion
    }
}
