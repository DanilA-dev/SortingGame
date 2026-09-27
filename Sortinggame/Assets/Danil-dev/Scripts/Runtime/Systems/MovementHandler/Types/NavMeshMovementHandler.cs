using UnityEngine;
using UnityEngine.AI;

namespace D_Dev.MovementHandler
{
    [System.Serializable]
    public class NavMeshMovementHandler : BaseMovementHandler
    {
        #region Fields

        [SerializeField] private NavMeshAgent _navMeshAgent;

        private bool _isStopped;

        #endregion

        #region Overrides

        public override void OnUpdate()
        {
            if (_navMeshAgent == null || _isStopped)
                return;

            if (_navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.speed = MaxVelocity;
                _navMeshAgent.destination = Direction;
            }
        }

        public override void StopMovement()
        {
            _isStopped = true;
            Direction = Vector3.zero;

            if (_navMeshAgent == null)
                return;

            if (_navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
                _navMeshAgent.ResetPath();
        }

        public override void ResumeMovement() => _isStopped = false;

        public override float GetVelocity() => _navMeshAgent != null ? _navMeshAgent.velocity.magnitude : 0f;
        public override bool IsMoving() => _navMeshAgent != null && _navMeshAgent.velocity.magnitude > 0.1f;

        #endregion
    }
}