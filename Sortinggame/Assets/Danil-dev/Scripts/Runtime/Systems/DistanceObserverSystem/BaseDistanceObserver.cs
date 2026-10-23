using D_Dev.Conditions;
using D_Dev.PolymorphicValueSystem;
using D_Dev.PositionRotationConfig;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.DistanceObserverSystem
{
    public abstract class BaseDistanceObserver : MonoBehaviour
    {
        #region Fields

        [Title("Targets")]
        [SerializeReference] protected BasePositionSettings _from = new();
        [SerializeReference] protected BasePositionSettings _to = new();
        [Title("Condition")]
        [SerializeField] protected ValueCompareType _compareType = ValueCompareType.EqualOrLess;
        [SerializeReference] protected PolymorphicValue<float> _distance = new FloatConstantValue();
        [Title("Settings")]
        [SerializeField, Min(0f)] protected float _checkInterval;
        [SerializeField] protected bool _raiseOnce;
        [Space]
        [FoldoutGroup("Events")]
        public UnityEvent OnConditionEnter;
        [FoldoutGroup("Events")]
        public UnityEvent OnConditionExit;
        [FoldoutGroup("Events")]
        public UnityEvent<float> OnDistanceChanged;

        private float _elapsed;
        private bool _hasState;
        private bool _completed;

        #endregion

        #region Properties

        public bool IsConditionMet { get; private set; }
        public float CurrentDistance { get; private set; }

        #endregion

        #region Monobehaviour

        protected virtual void OnEnable()
        {
            _elapsed = 0f;
            _hasState = false;
            _completed = false;
            IsConditionMet = false;
        }

        #endregion

        #region Public

        public void ForceCheck() => Evaluate();

        #endregion

        #region Protected

        protected void Check(float deltaTime)
        {
            if (_completed)
                return;

            if (_checkInterval > 0f)
            {
                _elapsed += deltaTime;
                if (_elapsed < _checkInterval)
                    return;

                _elapsed = 0f;
            }

            Evaluate();
        }

        #endregion

        #region Private

        private void Evaluate()
        {
            if (_completed || _from == null || _to == null || _distance == null)
                return;

            var distance = Vector3.Distance(_from.GetPosition(), _to.GetPosition());

            if (!_hasState || !Mathf.Approximately(distance, CurrentDistance))
            {
                CurrentDistance = distance;
                OnDistanceChanged?.Invoke(distance);
            }

            var isMet = Compare(distance, _distance.Value);

            if (_hasState && isMet == IsConditionMet)
                return;

            var wasMet = IsConditionMet;
            var hadState = _hasState;
            _hasState = true;
            IsConditionMet = isMet;

            if (isMet)
            {
                OnConditionEnter?.Invoke();
                _completed = _raiseOnce;
            }
            else if (hadState && wasMet)
            {
                OnConditionExit?.Invoke();
            }
        }

        private bool Compare(float distance, float target)
        {
            return _compareType switch
            {
                ValueCompareType.Less => distance < target,
                ValueCompareType.Equal => Mathf.Approximately(distance, target),
                ValueCompareType.Bigger => distance > target,
                ValueCompareType.EqualOrLess => distance <= target,
                ValueCompareType.EqualOrBigger => distance >= target,
                _ => false
            };
        }

        #endregion
    }
}
