using System.Collections;
using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InteractableSystem.Interactor
{
    public class Interactor : MonoBehaviour
    {
        #region Enums

        public enum InputMode
        {
            Press = 0,
            Hold = 1,
            Toggle = 2
        }

        #endregion

        #region Fields

        [SerializeField] private InteractableDetector.InteractableDetector _detector;
        [SerializeReference] private PolymorphicValue<bool> _interactInput = new BoolConstantValue();
        [ShowIf("@this._interactInput != null")]
        [SerializeField] private InputMode _inputMode = InputMode.Hold;
        [ShowIf("@this._interactInput != null && this._inputMode == InputMode.Hold")]
        [SerializeReference] private PolymorphicValue<float> _holdTime = new FloatConstantValue();
        [ShowIf("@this._interactInput != null && this._inputMode == InputMode.Hold")]
        [SerializeReference] private PolymorphicValue<float> _holdProgress = new FloatConstantValue();
        [FoldoutGroup("Debug")]
        [SerializeField] private bool _debug;

        private IInteractable _activeInteractable;
        private bool _isActiveFromFocus;
        private IInteractable _holdTarget;
        private Coroutine _holdRoutine;

        #endregion

        #region Properties

        public IInteractable ActiveInteractable => _activeInteractable;
        public bool IsInteracting => IsAlive(_activeInteractable);
        public bool IsHolding => _holdRoutine != null;

        public InputMode Mode
        {
            get => _inputMode;
            set => _inputMode = value;
        }

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            if (_detector != null)
                _detector.OnFocusChanged += OnDetectorFocusChanged;

            if (_interactInput != null)
                _interactInput.OnValueChanged += OnInteractInputChanged;
        }

        private void OnDisable()
        {
            if (_detector != null)
                _detector.OnFocusChanged -= OnDetectorFocusChanged;

            if (_interactInput != null)
                _interactInput.OnValueChanged -= OnInteractInputChanged;

            CancelHold();
            StopInteract();
        }

        #endregion

        #region Public

        public bool TryInteract()
        {
            if (_detector == null)
                return false;

            var started = TryInteract(_detector.CurrentInteractable);
            if (started && _activeInteractable != null)
                _isActiveFromFocus = true;

            return started;
        }

        public bool TryInteract(IInteractable target)
        {
            if (!IsAlive(target))
                return false;

            if (IsInteracting)
                StopInteract();

            if (!target.CanInteract(gameObject))
                return false;

            target.StartInteract(gameObject);

            if (target.CanBeStopped)
            {
                _activeInteractable = target;
                _isActiveFromFocus = false;
            }

            if (_debug)
                Debug.Log($"[Interactor : {gameObject.name}] <color=green> Interact with </color> {target.GameObject.name}");

            return true;
        }

        public void StopInteract()
        {
            var interactable = _activeInteractable;
            _activeInteractable = null;
            _isActiveFromFocus = false;

            if (!IsAlive(interactable))
                return;

            interactable.StopInteract(gameObject);

            if (_debug)
                Debug.Log($"[Interactor : {gameObject.name}] <color=red> Stop interact with </color> {interactable.GameObject.name}");
        }

        #endregion

        #region Listeners

        private void OnDetectorFocusChanged(IInteractable focused)
        {
            if (IsHolding && !ReferenceEquals(_holdTarget, focused))
                CancelHold();

            if (_activeInteractable == null || !_isActiveFromFocus)
                return;

            if (!ReferenceEquals(_activeInteractable, focused))
                StopInteract();
        }

        private void OnInteractInputChanged(bool isPressed)
        {
            switch (_inputMode)
            {
                case InputMode.Press:
                    if (isPressed)
                        TryInteract();
                    break;

                case InputMode.Hold:
                    if (isPressed)
                    {
                        StartHold();
                    }
                    else
                    {
                        CancelHold();
                        StopInteract();
                    }
                    break;

                case InputMode.Toggle:
                    if (!isPressed)
                        break;

                    if (IsInteracting)
                        StopInteract();
                    else
                        TryInteract();
                    break;
            }
        }

        #endregion

        #region Coroutines

        private IEnumerator HoldRoutine(float holdTime)
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
            _holdTarget = null;
            TryInteract();
        }

        #endregion

        #region Private

        private void StartHold()
        {
            CancelHold();

            if (_detector == null)
                return;

            var target = _detector.CurrentInteractable;
            if (!IsAlive(target) || !target.CanInteract(gameObject))
                return;

            var holdTime = _holdTime != null ? _holdTime.Value : 0f;
            if (holdTime <= 0f)
            {
                TryInteract();
                return;
            }

            _holdTarget = target;
            _holdRoutine = StartCoroutine(HoldRoutine(holdTime));
        }

        private void CancelHold()
        {
            if (_holdRoutine != null)
            {
                StopCoroutine(_holdRoutine);
                _holdRoutine = null;
            }

            _holdTarget = null;
            SetHoldProgress(0f);
        }

        private void SetHoldProgress(float progress)
        {
            if (_holdProgress != null)
                _holdProgress.Value = progress;
        }

        private static bool IsAlive(IInteractable interactable)
        {
            if (interactable == null)
                return false;

            if (interactable is Object unityObject)
                return unityObject != null;

            return interactable.GameObject != null;
        }

        #endregion
    }
}
