using D_Dev.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.InteractableSystem
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        #region Fields

        [Title("Interaction Settings")]
        [SerializeField] protected bool _isInteractable = true;
        [SerializeField] protected bool _isDistanceBased;
        [ShowIf(nameof(_isDistanceBased))]
        [SerializeField] protected float _interactionDistance = 2f;
        [SerializeField] protected bool _canBeStopped;
        [FoldoutGroup("Events")]
        public UnityEvent<GameObject> OnInteractStart;
        [FoldoutGroup("Events")]
        [ShowIf(nameof(_canBeStopped))]
        public UnityEvent OnInteractStop;
        [FoldoutGroup("Debug")]
        [SerializeField] protected bool _debug;
        
        #endregion

        #region Properties

        public bool IsInteracting { get; protected set; }
        public bool CanBeStopped => _canBeStopped;
        public bool IsDistanceBased => _isDistanceBased;

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

        protected virtual void OnInteract(GameObject interactor) {}
        protected virtual void OnStopInteract(GameObject interactor) {}

        #endregion

        #region Gizmos

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _interactionDistance);
        }

        #endregion
    }
}
