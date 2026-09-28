using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InteractableSystem.Interactor
{
    public class Interactor : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InteractableDetector.InteractableDetector _detector;
        [SerializeReference] private PolymorphicValue<bool> _interactInput = new BoolConstantValue();
        [FoldoutGroup("Debug")]
        [SerializeField] private bool _debug;

        private IInteractable _pressedInteractable;
        private IInteractable _activeInteractable;

        #endregion

        #region Properties

        public IInteractable ActiveInteractable => _activeInteractable;
        public bool IsInteracting => IsAlive(_activeInteractable);

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            if (_interactInput != null)
                _interactInput.OnValueChanged += OnInteractInputChanged;
        }

        private void OnDisable()
        {
            if (_interactInput != null)
                _interactInput.OnValueChanged -= OnInteractInputChanged;

            Release();
            StopInteract();
        }

        #endregion

        #region Public

        public void Press()
        {
            Release();

            if (_detector == null)
                return;

            var target = _detector.CurrentInteractable;
            if (!IsAlive(target))
                return;

            _pressedInteractable = target;
            target.PressInteract(gameObject);

            if (_debug)
                Debug.Log($"[Interactor : {gameObject.name}] <color=green> Press </color> {target.GameObject.name}");
        }

        public void Release()
        {
            var target = _pressedInteractable;
            _pressedInteractable = null;

            if (!IsAlive(target))
                return;

            target.ReleaseInteract(gameObject);

            if (_debug)
                Debug.Log($"[Interactor : {gameObject.name}] <color=red> Release </color> {target.GameObject.name}");
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
                _activeInteractable = target;

            if (_debug)
                Debug.Log($"[Interactor : {gameObject.name}] <color=green> Interact with </color> {target.GameObject.name}");

            return true;
        }

        public void StopInteract()
        {
            var interactable = _activeInteractable;
            _activeInteractable = null;

            if (!IsAlive(interactable))
                return;

            interactable.StopInteract(gameObject);

            if (_debug)
                Debug.Log($"[Interactor : {gameObject.name}] <color=red> Stop interact with </color> {interactable.GameObject.name}");
        }

        #endregion

        #region Listeners

        private void OnInteractInputChanged(bool isPressed)
        {
            if (isPressed)
                Press();
            else
                Release();
        }

        #endregion

        #region Private

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
