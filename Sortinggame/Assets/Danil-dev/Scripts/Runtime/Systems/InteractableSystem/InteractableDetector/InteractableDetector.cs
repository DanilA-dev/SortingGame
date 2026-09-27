using System.Collections;
using D_Dev.Base;
using D_Dev.ColliderEvents;
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

        [FoldoutGroup("Events")]
        public UnityEvent<IInteractable> OnInteractableFound;
        [FoldoutGroup("Events")]
        public UnityEvent OnInteractableLost;

        private IInteractable _currentInteractable;
        private WaitForSeconds _interval;

        #endregion

        #region Properties

        public IInteractable CurrentInteractable => _currentInteractable;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _interval = new WaitForSeconds(_updateRate);
            _triggerColliderObservable?.OnEnter.AddListener(OnTriggerInteractableEnter);
            _triggerColliderObservable?.OnExit.AddListener(OnTriggerInteractableExit);
        }

        private void Start()
        {
            if(_interactableDetectType == InteractableDetectType.Raycaster)
                StartCoroutine(DetectInteractableRoutine());
        }

        private void OnDisable()
        {
            _triggerColliderObservable?.OnEnter.RemoveListener(OnTriggerInteractableEnter);
            _triggerColliderObservable?.OnExit.RemoveListener(OnTriggerInteractableExit);
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
            SetInteractable(collider);
        }

        private void OnTriggerInteractableExit(Collider collider)
        {
            ResetInteractable();
        }

        #endregion
        
        #region Private

        private void DetectInteractable()
        {
            if (_raycaster.IsHit(out RaycastHit hit))
                SetInteractable(hit.collider);
            else
                ResetInteractable();
        }

        private void SetInteractable(Collider collider)
        {
            if (collider.TryGetComponent(out IInteractable interactable) &&
                interactable.CanInteract(gameObject))
            {
                _currentInteractable = interactable;
                OnInteractableFound?.Invoke(_currentInteractable);
            }
        }
        
        private void ResetInteractable()
        {
            _currentInteractable = null;
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
