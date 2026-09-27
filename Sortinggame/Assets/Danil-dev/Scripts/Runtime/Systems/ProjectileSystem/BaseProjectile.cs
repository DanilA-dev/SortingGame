using System.Linq;
using D_Dev.ColliderEvents;
using D_Dev.DamageableSystem;
using D_Dev.EntityPool;
using D_Dev.PolymorphicValueSystem;
using D_Dev.TagSystem;
using D_Dev.TagSystem.Extensions;
using D_Dev.TimerSystem;
using D_Dev.UpdateManagerSystem;
using D_Dev.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ProjectileSystem
{
    [RequireComponent(typeof(PoolableObject))]
    public abstract class BaseProjectile : MonoBehaviour, ITickable
    {
        #region Fields

        [Title("Variables")]
        [SerializeReference] protected PolymorphicValue<float> _speed = new FloatConstantValue();
        [SerializeReference] protected PolymorphicValue<float> _lifeTime = new FloatConstantValue();
        [SerializeReference] protected PolymorphicValue<float> _releaseDelayOnHit = new FloatConstantValue();
        [SerializeReference] protected PolymorphicValue<bool>  _lockToTarget = new BoolConstantValue();
        [SerializeReference] protected PolymorphicValue<int>   _pierceCount = new IntConstantValue { Value = 1 };

        [Title("Offsets")]
        [SerializeField] protected Vector3 _targetOffset;

        [Title("Components")]
        [SerializeField] protected TriggerColliderObservable _trigger;
        [SerializeField] protected Collider[] _colliders;
        [SerializeField] protected bool _ignoreOwnerTags = true;

        [FoldoutGroup("Events"), PropertyOrder(999)]
        [SerializeField] protected UnityEvent _onHit;
        [FoldoutGroup("Events"), PropertyOrder(999)]
        [SerializeField] protected UnityEvent _onReleased;

        protected PoolableObject _poolableObject;
        protected RotationHandler _rotationHandler;

        protected CountdownTimer _lifeTimer;
        protected CountdownTimer _releaseDelayTimer;

        protected Transform _target;
        protected GameObject _owner;
        protected TagComponent _ownerTagComponent;
        protected DamageData _damageData;

        protected Vector3 _startPosition;
        protected Vector3 _endTargetPosition;

        protected int _remainingPierce;
        protected bool _isReleasing;
        protected bool _isInitialized;

        #endregion

        #region Properties

        public bool IsInitialized => _isInitialized;
        public bool IsReleasing => _isReleasing;
        public Transform Target => _target;
        public GameObject Owner => _owner;
        public UnityEvent OnHit => _onHit;
        public UnityEvent OnReleased => _onReleased;

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            _poolableObject = GetComponent<PoolableObject>();

            _rotationHandler = new RotationHandler();
            _rotationHandler.Initialize(transform);

            if (_trigger != null)
                _trigger.OnEnter.AddListener(OnTriggerHit);

            UpdateManager.Add(this);
        }

        protected virtual void OnDestroy()
        {
            if (_trigger != null)
                _trigger.OnEnter.RemoveListener(OnTriggerHit);

            if (_lifeTimer != null)
                _lifeTimer.OnTimerEnd -= ReleaseToPool;

            if (_releaseDelayTimer != null)
                _releaseDelayTimer.OnTimerEnd -= ReleaseToPool;

            UpdateManager.Remove(this);
        }

        #endregion

        #region ITickable

        public void Tick()
        {
            if (!_isInitialized)
                return;

            float dt = Time.deltaTime;

            if (_lifeTimer != null && _lifeTimer.IsRunning)
                _lifeTimer.Tick(dt);

            if (_releaseDelayTimer != null && _releaseDelayTimer.IsRunning)
                _releaseDelayTimer.Tick(dt);

            if (!_isInitialized || _isReleasing)
                return;

            Move(dt);
        }

        #endregion

        #region Public

        public virtual void Init(Transform target, GameObject owner, DamageData damageData)
        {
            if (target == null)
            {
                Debug.LogError($"[{nameof(BaseProjectile)}] Init(Transform) called with null target on '{name}'. Use Init(Vector3) for fire-forward.");
                return;
            }

            InitInternal(target.position + _targetOffset, target, owner, damageData);
        }

        public virtual void Init(Vector3 endPosition, GameObject owner, DamageData damageData)
        {
            InitInternal(endPosition, null, owner, damageData);
        }

        public virtual void InitInDirection(Vector3 direction, GameObject owner, DamageData damageData)
        {
            if (direction.sqrMagnitude < 0.0001f)
            {
                Debug.LogError($"[{nameof(BaseProjectile)}] InitInDirection called with zero direction on '{name}'.");
                return;
            }

            float maxDistance = Mathf.Max(0.01f, _speed.Value * _lifeTime.Value);
            Vector3 endPos = transform.position + direction.normalized * maxDistance;
            InitInternal(endPos, null, owner, damageData);
        }

        #endregion

        #region Init core

        protected void InitInternal(Vector3 endPosition, Transform target, GameObject owner, DamageData damageData)
        {
            _target = target;
            _owner = owner;
            _ownerTagComponent = _owner != null ? _owner.GetTagComponent() : null;
            _damageData = damageData;

            _startPosition = transform.position;
            _endTargetPosition = endPosition;

            _remainingPierce = Mathf.Max(1, _pierceCount.Value);
            _isReleasing = false;

            EnsureTimers();
            ToggleColliders(true);
            ApplyOwnerTagIgnore();

            _lifeTimer.Start();

            OnInitialized();
            _isInitialized = true;
        }

        #endregion

        #region Hit handling

        protected virtual void OnTriggerHit(Collider other)
        {
            if (!_isInitialized || _isReleasing)
                return;

            if (other.TryGetComponent(out IDamageable damageable))
            {
                _damageData.DamageSource = gameObject;
                damageable.TakeDamage(_damageData);
            }

            _onHit?.Invoke();

            _remainingPierce--;
            if (_remainingPierce > 0)
                return;

            BeginRelease();
        }

        protected void BeginRelease()
        {
            if (_isReleasing)
                return;

            _isReleasing = true;
            ToggleColliders(false);

            if (_releaseDelayOnHit.Value > 0f)
                _releaseDelayTimer.Start();
            else
                ReleaseToPool();
        }

        protected void ReleaseToPool()
        {
            if (!_isInitialized)
                return;

            _isInitialized = false;
            _onReleased?.Invoke();

            if (_poolableObject != null)
                _poolableObject.Release();
        }

        #endregion

        #region Helpers

        protected Vector3 GetCurrentTargetPosition()
        {
            if (_lockToTarget.Value && _target != null && _target.gameObject.activeInHierarchy)
                return _target.position + _targetOffset;

            return _endTargetPosition;
        }

        #endregion

        #region Override hooks

        protected virtual void OnInitialized() { }
        protected abstract void Move(float deltaTime);

        #endregion

        #region Private

        private void EnsureTimers()
        {
            if (_lifeTimer != null)
                _lifeTimer.OnTimerEnd -= ReleaseToPool;

            _lifeTimer = new CountdownTimer(_lifeTime.Value);
            _lifeTimer.OnTimerEnd += ReleaseToPool;

            if (_releaseDelayTimer != null)
                _releaseDelayTimer.OnTimerEnd -= ReleaseToPool;

            _releaseDelayTimer = new CountdownTimer(_releaseDelayOnHit.Value);
            _releaseDelayTimer.OnTimerEnd += ReleaseToPool;
        }

        private void ApplyOwnerTagIgnore()
        {
            if (!_ignoreOwnerTags || _ownerTagComponent == null || _trigger == null)
                return;

            var ownerTags = _ownerTagComponent.Tags;
            if (ownerTags == null || ownerTags.Count == 0)
                return;

            var checker = _trigger.CollisionChecker;
            if (checker == null)
                return;

            checker.IgnoreTag = true;
            checker.IgnoreTags = ownerTags as Tag[] ?? ownerTags.ToArray();
        }

        private void ToggleColliders(bool isEnabled)
        {
            if (_colliders == null)
                return;

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null)
                    _colliders[i].enabled = isEnabled;
            }
        }

        #endregion
    }
}
