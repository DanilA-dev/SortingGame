using System.Collections;
using D_Dev.Base;
using D_Dev.ColliderEvents;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.InteractableSystem.InteractableDetector
{
    public class InteractableDetector : MonoBehaviour
    {
        #region Enums

        private enum InteractableDetectType
        {
            Raycaster = 0,
            Trigger = 1,
        }

        #endregion

        #region Fields

        [SerializeField] private InteractableDetectType _interactableDetectType;
        [ShowIf(nameof(_interactableDetectType), InteractableDetectType.Trigger)]
        [SerializeField] private TriggerColliderObservable _triggerColliderObservable;
        [ShowIf(nameof(_interactableDetectType), InteractableDetectType.Raycaster)]
        [SerializeField] private float _updateRate = 0.1f;
        [ShowIf(nameof(_interactableDetectType), InteractableDetectType.Raycaster)]
        [HideLabel]
        [SerializeField] private Raycaster.Raycaster _raycaster;
        [SerializeReference] private PolymorphicValue<GameObject> _currentInteractableOutput = new GameObjectConstantValue();

        [FoldoutGroup("Events")]
        public UnityEvent<GameObject> OnInteractableFound;
        [FoldoutGroup("Events")]
        public UnityEvent OnInteractableLost;

        private IInteractable _currentInteractable;
        private WaitForSeconds _interval;
        private Coroutine _detectRoutine;

        #endregion

        #region Properties

        public IInteractable CurrentInteractable => _currentInteractable;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _interval = new WaitForSeconds(_updateRate);
        }

        private void OnEnable()
        {
            _triggerColliderObservable?.OnEnter.AddListener(OnTriggerInteractableEnter);
            _triggerColliderObservable?.OnExit.AddListener(OnTriggerInteractableExit);

            if(_interactableDetectType == InteractableDetectType.Raycaster)
                _detectRoutine = StartCoroutine(DetectInteractableRoutine());
        }

        private void OnDisable()
        {
            _triggerColliderObservable?.OnEnter.RemoveListener(OnTriggerInteractableEnter);
            _triggerColliderObservable?.OnExit.RemoveListener(OnTriggerInteractableExit);

            if (_detectRoutine != null)
            {
                StopCoroutine(_detectRoutine);
                _detectRoutine = null;
            }

            SetCurrent(null);
        }

        #endregion

        #region Coroutines

        private IEnumerator DetectInteractableRoutine()
        {
            while (true)
            {
                DetectInteractable();
                yield return _interval;
            }
        }

        #endregion

        #region Listeners

        private void OnTriggerInteractableEnter(Collider collider)
        {
            var interactable = GetInteractable(collider);
            if (interactable != null)
                SetCurrent(interactable);
        }

        private void OnTriggerInteractableExit(Collider collider)
        {
            if (_currentInteractable == null)
                return;

            if (collider.TryGetComponent(out IInteractable interactable) &&
                ReferenceEquals(interactable, _currentInteractable))
                SetCurrent(null);
        }

        #endregion

        #region Private

        private void DetectInteractable()
        {
            var interactable = _raycaster.IsHit(out RaycastHit hit) ? GetInteractable(hit.collider) : null;
            SetCurrent(interactable);
        }

        private IInteractable GetInteractable(Collider collider)
        {
            if (collider.TryGetComponent(out IInteractable interactable) &&
                interactable.CanInteract(gameObject))
                return interactable;

            return null;
        }

        private void SetCurrent(IInteractable interactable)
        {
            if (ReferenceEquals(_currentInteractable, interactable))
                return;

            _currentInteractable = interactable;
            var target = interactable?.GameObject;

            if (_currentInteractableOutput != null)
                _currentInteractableOutput.Value = target;

            if (target != null)
                OnInteractableFound?.Invoke(target);
            else
                OnInteractableLost?.Invoke();
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if(_interactableDetectType == InteractableDetectType.Raycaster)
                _raycaster.OnGizmos();
        }

        #endregion
    }
}
