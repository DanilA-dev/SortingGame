using Cinemachine;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.Utility
{
    public class CinemachineTargetAutoAssigner : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private TransformScriptableVariable _target;
        [Space]
        [SerializeField] private bool _isFollow;
        [SerializeField] private bool _isLookAt;

        #endregion

        #region MonoBehaviour

        private void Awake() => _target.OnValueUpdate += SetTarget;
        private void OnDestroy() => _target.OnValueUpdate -= SetTarget;

        #endregion

        #region Listeners

        private void SetTarget(Transform target)
        {
            _camera.Follow = _isFollow ? target : null;
            _camera.LookAt = _isLookAt ? target : null;
        }

        #endregion
    }
}
