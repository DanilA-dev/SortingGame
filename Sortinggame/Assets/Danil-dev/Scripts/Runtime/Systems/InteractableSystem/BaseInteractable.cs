using System.Collections;
using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.InteractableSystem
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        #region Enums

        public enum InteractMode
        {
            Press = 0,
            Hold = 1,
            Toggle = 2
        }

        #endregion

        #region Fields

        [Title("Interaction Settings")]
        [SerializeField] protected bool _isInteractable = true;
        [SerializeField] protected InteractMode _interactMode = InteractMode.Press;
        [ShowIf(nameof(_interactMode), InteractMode.Hold)]
        [SerializeReference] protected PolymorphicValue<float> _holdTime = new FloatConstantValue();
        [ShowIf(nameof(_interactMode), InteractMode.Hold)]
        [SerializeReference] protected PolymorphicValue<float> _holdProgress = new FloatConstantValue();
        [SerializeField] protected bool _isDistanceBased;
        [ShowIf(nameof(_isDistanceBased))]
        [SerializeField] protected float _interactionDistance = 2f;
        [SerializeField] protected bool _canBeStopped;
        [ShowIf(nameof(_canBeStopped))]
        [SerializeField] protected bool _stopOnFocusLost = true;
        [FoldoutGroup("Events")]
        public UnityEvent<GameObject> OnInteractStart;
        [FoldoutGroup("Events")]
        [ShowIf(nameof(_canBeStopped))]
        public UnityEvent OnInteractStop;
        [FoldoutGroup("Events")]
        public UnityEvent<GameObject> OnFocused;
        [FoldoutGroup("Events")]
        public UnityEvent<GameObject> OnUnfocused;
        [FoldoutGroup("Debug")]
        [SerializeField] protected bool _debug;

        private Coroutine _holdRoutine;

        #endregion

        #region Properties

        public GameObject GameObject => gameObject;
        public bool IsInteracting { get; protected set; }
        public bool IsFocused { get; protected set; }
        public bool IsHolding => _holdRoutine != null;

        public bool IsInteractable
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }

        public InteractMode Mode
        {
            get => _interactMode;
            set => _interactMode = value;
        }

        public bool CanBeStopped
        {
            get => _canBeStopped;
            set => _canBeStopped = value;
        }

        public bool StopOnFocusLost
        {
            get => _stopOnFocusLost;
            set => _stopOnFocusLost = value;
        }

        public bool IsDistanceBased
        {
            get => _isDistanceBased;
            set => _isDistanceBased = value;
        }

        #endregion

        #region MonoBehaviour

        protected virtual void OnDisable() => CancelHold();

        #endregion

        #region Virtual

        public virtual bool CanInteract(GameObject interactor)
        {
            if (!_isInteractable)
            {
                Debug.Log($"<color=yellow> [Interactable : {gameObject.name}] Can't interact with {interactor.name} because it's not interactable.</color>");
                return false;
            }

            if (interactor == null)
                return false;

            var distance = Vector3.Distance(transform.position, interactor.transform.position);
            var isWithinDistance = distance <= _interactionDistance;
            if (_isDistanceBased)
            {
                if (isWithinDistance)
                    return true;

                Debug.Log($"<color=yellow> [Interactable : {gameObject.name}] Can't interact with {interactor.name} because it's too far away.</color>");
                return false;
            }
            return true;
        }


        public void StartInteract(GameObject interactor)
        {
            if (!CanInteract(interactor))
                return;

            IsInteracting = true;
            OnInteract(interactor);
            OnInteractStart?.Invoke(interactor);

            if (!_canBeStopped)
                IsInteracting = false;

            if(_debug)
                Debug.Log($"[Interactable : {gameObject.name}] <color=green> Start interacting with </color> {interactor.name}");
        }

        public void StopInteract(GameObject interactor)
        {
            if (!IsInteracting)
                return;

            if(!_canBeStopped)
                return;

            IsInteracting = false;
            OnStopInteract(interactor);
            OnInteractStop?.Invoke();

            if(_debug)
                Debug.Log($"[Interactable : {gameObject.name}] <color=red> Stop interacting with </color> {interactor.name}");
        }

        public void PressInteract(GameObject interactor)
        {
            switch (_interactMode)
            {
                case InteractMode.Press:
                    StartInteract(interactor);
                    break;

                case InteractMode.Hold:
                    StartHold(interactor);
                    break;

                case InteractMode.Toggle:
                    if (IsInteracting)
                        StopInteract(interactor);
                    else
                        StartInteract(interactor);
                    break;
            }
        }

        public void ReleaseInteract(GameObject interactor)
        {
            if (_interactMode != InteractMode.Hold)
                return;

            CancelHold();
            StopInteract(interactor);
        }

        public void Focus(GameObject interactor)
        {
            if (IsFocused)
                return;

            IsFocused = true;
            OnFocus(interactor);
            OnFocused?.Invoke(interactor);

            if(_debug)
                Debug.Log($"[Interactable : {gameObject.name}] <color=cyan> Focused by </color> {interactor.name}");
        }

        public void Unfocus(GameObject interactor)
        {
            if (!IsFocused)
                return;

            IsFocused = false;
            CancelHold();

            if (_stopOnFocusLost)
                StopInteract(interactor);

            OnUnfocus(interactor);
            OnUnfocused?.Invoke(interactor);

            if(_debug)
                Debug.Log($"[Interactable : {gameObject.name}] <color=cyan> Unfocused by </color> {interactor.name}");
        }

        protected virtual void OnInteract(GameObject interactor) {}
        protected virtual void OnStopInteract(GameObject interactor) {}
        protected virtual void OnFocus(GameObject interactor) {}
        protected virtual void OnUnfocus(GameObject interactor) {}

        #endregion

        #region Coroutines

        private IEnumerator HoldRoutine(GameObject interactor, float holdTime)
        {
            var elapsed = 0f;
            SetHoldProgress(0f);

            while (elapsed < holdTime)
            {
                yield return null;
                elapsed += Time.deltaTime;
                SetHoldProgress(Mathf.Clamp01(elapsed / holdTime));
            }

            _holdRoutine = null;
            StartInteract(interactor);
        }

        #endregion

        #region Private

        private void StartHold(GameObject interactor)
        {
            CancelHold();

            if (!CanInteract(interactor))
                return;

            var holdTime = _holdTime != null ? _holdTime.Value : 0f;
            if (holdTime <= 0f)
            {
                StartInteract(interactor);
                return;
            }

            _holdRoutine = StartCoroutine(HoldRoutine(interactor, holdTime));
        }

        private void CancelHold()
        {
            if (_holdRoutine != null)
            {
                StopCoroutine(_holdRoutine);
                _holdRoutine = null;
            }

            SetHoldProgress(0f);
        }

        private void SetHoldProgress(float progress)
        {
            if (_holdProgress != null)
                _holdProgress.Value = progress;
        }

        #endregion

        #region Gizmos

        protected virtual void OnDrawGizmosSelected()
        {
            if (_isDistanceBased)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, _interactionDistance);
            }
        }

        #endregion
    }
}
