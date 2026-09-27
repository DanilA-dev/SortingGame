using D_Dev.UpdateManagerSystem;
using UnityEngine;

namespace D_Dev.StateMachineBehaviour.Extensions
{
    public class TickedComponentStateMachineController : ComponentStateMachineController, ITickable, IFixedTickable
    {
        #region Fields

        [SerializeField] private int _tickPriority = 0;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            UpdateManager.AddWithPriority(this, _tickPriority);
            FixedUpdateManager.AddWithPriority(this, _tickPriority);
        }

        private void OnDisable()
        {
            UpdateManager.Remove(this);
            FixedUpdateManager.Remove(this);
        }

        #endregion
        
        #region ITickable

        public void Tick()
        {
            ManagedUpdate(Time.time);
        }

        #endregion
        
        #region IFixedTickable
        public void FixedTick()
        {
            ManagedFixedUpdate();
        }
        #endregion
    }
}