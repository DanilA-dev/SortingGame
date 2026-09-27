using System;
using UnityEngine;

namespace D_Dev.StateMachineBehaviour
{
    public class UIStateDebugger : MonoBehaviour
    {
        #region Fields

        [SerializeField] private StateMachineController _stateMachineController;
        [SerializeField] private TextMesh _text;

        private Camera _camera;
        
        #endregion
        
        #region Monobehaviour

        private void OnEnable()
        {
            _camera = Camera.main;
            _stateMachineController.OnAnyStateEnter.AddListener(OnStateEnter);
        }
        private void OnDisable() => _stateMachineController.OnAnyStateEnter.RemoveListener(OnStateEnter);

        private void LateUpdate() => transform.LookAt(_camera.transform.position);

        #endregion

        #region Listeners
        private void OnStateEnter(string state) => _text.text = state;

        #endregion
    }
}
