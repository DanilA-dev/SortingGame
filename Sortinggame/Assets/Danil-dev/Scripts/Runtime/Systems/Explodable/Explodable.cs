using System.Linq;
using D_Dev.DamageableSystem;
using D_Dev.PolymorphicValueSystem;
using D_Dev.TagSystem;
using D_Dev.TagSystem.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ExplodableTrigger
{
    public class Explodable : MonoBehaviour
    {
        #region Fields

        [Title("Damage Settings")]
        [SerializeField] private DamageData _damageData;

        [Title("Owner Settings")] 
        [SerializeField] private bool _addOwnerToIgnore;
        [ShowIf(nameof(_addOwnerToIgnore))]
        [SerializeField] private bool _ignoreByOwnerTag;
        [ShowIf(nameof(_addOwnerToIgnore))]
        [SerializeField] private bool _ignoreByOwnerCollider;
        [ShowIf(nameof(_addOwnerToIgnore))]
        [SerializeField] private Transform _rootOwner;

        [Title("Values")]
        [SerializeReference] private PolymorphicValue<float> _explosionRadius = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _explodeForce = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<Vector3> _explodeDirectionModifier = new Vector3ConstantValue();

        [SerializeField] private ForceMode _forceMode;

        [Title("Colliders Settings")]
        [SerializeField] private int _colliderBuffer;
        [SerializeField] private ColliderChecker.ColliderChecker _colliderChecker;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent OnExplode;

        private Collider[] _colliders;

        private Vector3 _center;
        private TagComponent _rootOwnerTagComponent; 

        private Vector3 _directionModifier; 
        
        #endregion

        #region Monobehaviour

        private void Start()
        {
            _colliders = new Collider[_colliderBuffer];
            _center = transform.position;
            _directionModifier = _explodeDirectionModifier.Value; 

            SetOwnerToIgnore();
        }

        #endregion
        
        #region Public

        public void Explode()
        {
            ApplyExplosionForce(transform.position, _explosionRadius.Value, _explodeForce.Value, _forceMode);
        }

        public void Explode(Vector3 center)
        {
            ApplyExplosionForce(center, _explosionRadius.Value, _explodeForce.Value, _forceMode);
        }

        public void Explode(Vector3 center, float radius)
        {
            ApplyExplosionForce(center, radius, _explodeForce.Value, _forceMode);
        }

        public void Explode(Vector3 center, float radius, float force)
        {
            ApplyExplosionForce(center, radius, force, _forceMode);
        }

        public void Explode(Vector3 center, float radius, float force, ForceMode forceMode)
        {
            ApplyExplosionForce(center, radius, force, forceMode);
        }

        #endregion

        #region Private

        private void ApplyExplosionForce(Vector3 center, float radius, float power, ForceMode forceMode)
        {
            _center = center;

            int hitCount = Physics.OverlapSphereNonAlloc(_center, radius, _colliders);
            if (hitCount <= 0)
                return;

            for (int i = 0; i < hitCount; i++)
            {
                var collider = _colliders[i];

                if (!_colliderChecker.IsColliderPassed(collider))
                    continue;

                var attachedRigidbody = collider.attachedRigidbody;
                var damageable = collider.GetComponentInParent<IDamageable>();

                Vector3 pos = collider.transform.position;
                Vector3 baseDirection = (pos - _center).normalized;
                Vector3 direction = (baseDirection + _directionModifier).normalized;
               
                if (attachedRigidbody != null)
                    attachedRigidbody.AddForce(direction * power, forceMode);
               
                if (damageable != null)
                {
                    var damageData = _damageData;

                    damageData.DamageSource = gameObject;
                    damageData.Force = power;
                    damageData.ForceDirection = direction;
                    damageData.ForceMode = forceMode;
                   
                    damageable.TakeDamage(damageData);
                }
            }
            OnExplode?.Invoke(); 
        }
        
        private void SetOwnerToIgnore()
        {
            if (!_addOwnerToIgnore || _rootOwner == null)
                return;
         
            var root = _rootOwner.root;
            var ownerRootTags = _rootOwnerTagComponent != null ? _rootOwnerTagComponent.Tags : null;

            _rootOwnerTagComponent = root.gameObject.GetTagComponent();
            if (_ignoreByOwnerTag && ownerRootTags != null)
            {
                _colliderChecker.IgnoreTag = true;
                _colliderChecker.IgnoreTags = ownerRootTags as Tag[] ?? ownerRootTags.ToArray();
            }

            if (_ignoreByOwnerCollider && root.TryGetComponent(out Collider collider))
            {
                _colliderChecker.IgnoreColliders = true;
                _colliderChecker.IgnoredColliders = new[] { collider };
            }
        }

        #endregion
        
        #region Gizmos

        private void OnDrawGizmos()
        {
            if (_explosionRadius == null)
                return;

            var center = _center;

            if (!Application.isPlaying)
                center = transform.position;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, _explosionRadius.Value);
        }

        #endregion
    }
}