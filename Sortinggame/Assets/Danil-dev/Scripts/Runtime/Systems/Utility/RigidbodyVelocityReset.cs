using UnityEngine;

namespace D_Dev.Utility
{
    public class RigidbodyVelocityReset : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private bool _resetOnEnable;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_resetOnEnable)
            {
                ResetVelocity();
            }
        }

        #endregion

        #region Public

        public void ResetVelocity()
        {
            if(_rigidbody.isKinematic)
                return;
            
            _rigidbody.linearVelocity = Vector3.one;
            _rigidbody.angularVelocity = Vector3.one;
        }

        #endregion
    }
}