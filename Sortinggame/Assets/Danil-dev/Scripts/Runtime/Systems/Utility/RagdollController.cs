using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.Utility
{
    public class RagdollController : MonoBehaviour
    {
        #region Fields

        [SerializeField, ReadOnly] private bool _isActive;
        [Space]
        [Title("Components")]
        [SerializeField] private Transform _musclesRoot;
        [SerializeField] private Collider _mainColl;
        [SerializeField] private Rigidbody _mainbody;

        [Title("Animation")]
        [SerializeField] private bool _deactivateAnimatorOnActive;
        [ShowIf(nameof(_deactivateAnimatorOnActive))]
        [SerializeField] private Animator _animator;

        [Title("Root body alignment")] 
        [SerializeField] private bool _keepMainBodyVelocity = true;
        [SerializeField] private Transform _ragdollRootBone;
        [SerializeField] private bool _alignRootOnDeactivate = true;
        [SerializeField] private bool _alignRotation = true;
        [ShowIf("@_alignRootOnDeactivate")]
        [SerializeField] private LayerMask _groundMask = ~0;
        [ShowIf("@_alignRootOnDeactivate")]
        [SerializeField, Min(0f)] private float _groundCheckDistance = 2f;

        [FoldoutGroup("Events")]
        public UnityEvent OnRagdollActivate;
        [FoldoutGroup("Events")]
        public UnityEvent OnRagdollDeactivate;

        
        private const float FACING_THRESHOLD = 0.0001f;

        private Vector3 _mainBodyVelocity;
        
        private Dictionary<Collider, Rigidbody> _ragdollMuscles;
        
        #endregion

        #region Properties

        public bool IsActive => _isActive;

        public bool DeactivateAnimatorOnActive
        {
            get => _deactivateAnimatorOnActive;
            set => _deactivateAnimatorOnActive = value;
        }

        public bool AlignRootOnDeactivate
        {
            get => _alignRootOnDeactivate;
            set => _alignRootOnDeactivate = value;
        }

        public bool AlignRotation
        {
            get => _alignRotation;
            set => _alignRotation = value;
        }

        public LayerMask GroundMask
        {
            get => _groundMask;
            set => _groundMask = value;
        }

        public float GroundCheckDistance
        {
            get => _groundCheckDistance;
            set => _groundCheckDistance = value;
        }

        #endregion
        
        #region Monobehaviour

        private void Awake()
        {
            GetMuscles();
            DeactivateRagdoll();
        }

        #endregion

        #region Public

        public void ActivateRagdoll()
        {
            if (_keepMainBodyVelocity)
                _mainBodyVelocity = _mainbody.linearVelocity;
            
            _mainColl.enabled = false;
            _mainbody.isKinematic = true;
            
            if (_deactivateAnimatorOnActive && _animator != null)
                _animator.enabled = false;
            
            SetMusclesKinematic(false);
            if(_keepMainBodyVelocity)
                SetMusclesLinearVelocity(_mainBodyVelocity);
            SetMusclesColliderActive(true);

            if(_isActive)
                return;
            
            _isActive = true;
            OnRagdollActivate?.Invoke();
        }

        public void DeactivateRagdoll()
        {
            if (_isActive && _alignRootOnDeactivate)
                AlignRootToRagdollBone();

            _mainColl.enabled = true;
            _mainbody.isKinematic = false;
            if (_deactivateAnimatorOnActive && _animator != null)
                _animator.enabled = true;

            SetMusclesKinematic(true);
            SetMusclesColliderActive(false);

            if(!_isActive)
                return;

            _isActive = false;
            OnRagdollDeactivate?.Invoke();
        }

        public void AlignRootToRagdollBone()
        {
            if (_ragdollRootBone == null || _mainbody == null)
                return;

            var root = _mainbody.transform;

            Vector3 targetPosition = _ragdollRootBone.position;
            if (Physics.Raycast(targetPosition, Vector3.down, out var hit, _groundCheckDistance, _groundMask))
                targetPosition = hit.point;

            Quaternion targetRotation = root.rotation;
            if (_alignRotation)
            {
                Vector3 flatForward = Vector3.ProjectOnPlane(GetBoneFacing(), Vector3.up);
                if (flatForward.sqrMagnitude > FACING_THRESHOLD)
                    targetRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
            }

            root.SetPositionAndRotation(targetPosition, targetRotation);
        }

        #endregion
        
        #region Private

        private void GetMuscles()
        {
            var colls = _musclesRoot.GetComponentsInChildren<Collider>();
            if(colls == null)
                return;

            _ragdollMuscles = new();
            
            foreach (var coll in colls)
                _ragdollMuscles.TryAdd(coll, coll.attachedRigidbody);
        }

        private void SetMusclesKinematic(bool value)
        {
            foreach (var (coll, body) in _ragdollMuscles)
                body.isKinematic = value;
        }

        private void SetMusclesLinearVelocity(Vector3 velocity)
        {
            foreach (var (coll, body) in _ragdollMuscles)
                body.linearVelocity = velocity;
        }
        
        private void SetMusclesColliderActive(bool value)
        {
            foreach (var (coll, body) in _ragdollMuscles)
                coll.enabled = value;
        }

        private Vector3 GetBoneFacing()
        {
            Vector3 boneUp = _ragdollRootBone.up;
            return boneUp.y >= 0f ? -boneUp : boneUp;
        }

        #endregion

        #region Debug

        [FoldoutGroup("Debug")]
        [Button]
        private void ToggleRagdoll(bool value)
        {
            if(value)
                ActivateRagdoll();
            else
                DeactivateRagdoll();
        }

        #endregion
    }
}