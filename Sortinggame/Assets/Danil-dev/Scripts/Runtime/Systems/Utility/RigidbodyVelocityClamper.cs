using UnityEngine;

namespace D_Dev.Utility
{
    public class RigidbodyVelocityClamper : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Rigidbody _body;
        [SerializeField, Min(0f)] private float _maxVelocity = 0f;

        private Vector3 _clampedVelocity;

        #endregion
        
        #region Properties
        
        public Rigidbody Body
        {
            get => _body;
            set => _body = value;
        }

        public float MaxVelocity
        {
            get => _maxVelocity;
            set => _maxVelocity = value;
        }   
        
        public Vector3 ClampedVelocity
        {
            get => _clampedVelocity;
            set => _clampedVelocity = value;
        }
        
        #endregion
        
        #region Monobehaviour

        private void Reset() => TryGetComponent(out _body);
        private void FixedUpdate() => ClampVelocity();

        #endregion

        #region Private

        private void ClampVelocity()
        {
            if (_body != null)
            {
                _clampedVelocity = Vector3.ClampMagnitude(_body.linearVelocity, _maxVelocity);
                _body.linearVelocity = _clampedVelocity;
            }
        }

        #endregion
    }
}
