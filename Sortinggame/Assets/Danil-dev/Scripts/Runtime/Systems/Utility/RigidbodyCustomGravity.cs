using UnityEngine;

namespace D_Dev.Utility
{
    public class RigidbodyCustomGravity : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private Rigidbody _body;
        [SerializeField] private Vector3 _gravityDirection = new(0f, -9.81f, 0f);
        [SerializeField] private float _gravityMultiplier = 1;
        [SerializeField] private ForceMode _forceMode;

        #endregion
        
        #region Properties
        
        public Rigidbody Body
        {
            get => _body;
            set => _body = value;
        }

        public Vector3 GravityDirection
        {
            get => _gravityDirection;
            set => _gravityDirection = value;
        }

        public float GravityMultiplier
        {
            get => _gravityMultiplier;
            set => _gravityMultiplier = value;
        }

        public ForceMode Mode
        {
            get => _forceMode;
            set => _forceMode = value;
        }

        #endregion

        #region Monobehaviour

        private void Reset() => TryGetComponent(out _body);

        private void Awake()
        {
            if(_body != null)
                _body.useGravity = false;
        }

        private void FixedUpdate()
        {
            UpdateGravity();
        }
        
        #endregion

        #region Private
        private void UpdateGravity()
        {
            _body?.AddForce(_gravityDirection * _gravityMultiplier, _forceMode);
        }
        #endregion
    }
}
