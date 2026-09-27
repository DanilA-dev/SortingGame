using UnityEngine;

namespace D_Dev.Utility
{
    public class RigidbodyPush : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
        [Space]
        [SerializeField] private ForceMode _forceMode;
        [SerializeField] private float _strength;
        [SerializeField] private Vector3 _forceDirection;
        [SerializeField] private bool _addForceOnStart;

        #endregion

        #region Monobehaviour

        private void Reset()
        {
            if(_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (_addForceOnStart)
                AddForce();
        }

        #endregion

        #region Public

        public void AddForce()
        {
            _rigidbody?.AddForce(_forceDirection.normalized * _strength, _forceMode);
        }

        public void AddForce(Vector3 direction)
        {
            _rigidbody?.AddForce(direction.normalized * _strength, _forceMode);
        }
        
        public void AddForce(Vector3 direction, float strength)
        {
            _rigidbody?.AddForce(direction.normalized * strength, _forceMode);
        }

        public void AddForce(Vector3 direction, float strength, ForceMode forceMode)
        {
            _rigidbody?.AddForce(direction.normalized * strength, forceMode);
        }

        #endregion
    }
}
