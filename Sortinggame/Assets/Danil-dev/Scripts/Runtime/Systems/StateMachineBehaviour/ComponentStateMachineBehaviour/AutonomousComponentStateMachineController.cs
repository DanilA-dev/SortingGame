using UnityEngine;

namespace D_Dev.StateMachineBehaviour
{
    public class AutonomousComponentStateMachineController : ComponentStateMachineController
    {
        #region Monobehaviour

        private void Update() => ManagedUpdate(Time.time);

        private void FixedUpdate() => ManagedFixedUpdate();

        #endregion
    }
}