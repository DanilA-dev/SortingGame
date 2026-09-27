using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.Utility
{
    public class RigidbodyPusherFromPoint : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform _rootPusher;
        [SerializeField] private float _pushForce;
        [SerializeField] private ForceMode _pushForceMode;

        private HashSet<Rigidbody> _rigidbodiesToPush = new();

        #endregion

        #region Monobehaviour

        private void Reset()
        {
            if(_rootPusher == null)
                _rootPusher = transform;
        }
        
        private void FixedUpdate()
        {
            AddForceToBodies();
        }

        #endregion

        #region Public

        public void AddBodyToPush(Collider col)
        {
            _rigidbodiesToPush.Add(col.attachedRigidbody);
        }
        
        public void RemoveBodyFromPush(Collider col)
        {
            if(_rigidbodiesToPush.Contains(col.attachedRigidbody))
                _rigidbodiesToPush.Remove(col.attachedRigidbody);
        }

        #endregion
        
        #region Private

        private void AddForceToBodies()
        {
            if (_rigidbodiesToPush.Count <= 0)
                return;

            foreach (var rb in _rigidbodiesToPush)
            {
                if(rb == null)
                    continue;
                
                var direction = (_rootPusher.position - rb.position).normalized;
                rb.AddForce(-direction * _pushForce, _pushForceMode);
            }
        }

        #endregion
    }
}
