using System.Collections;
using D_Dev.CoroutineManagerSystem;
using D_Dev.CustomEventManager;
using D_Dev.InteractableSystem;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    public class ItemInteractable : BaseInteractable
    {
        #region Fields

        [Title("Base Settings")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeReference] private PolymorphicValue<float> _sleepDelay = new FloatConstantValue();
        [Title("Events Variables")] 
        [SerializeField] private StringScriptableVariable _onInteractStartEventName;
        [SerializeField] private StringScriptableVariable _onInteractStopEventName;
        
        #endregion

        #region Properties

        public bool IsSorted { get; private set; }

        #endregion

        #region Monobehaviour

        private void Start()
        {
            TryStartSleepLogic();
        }

        private void TryStartSleepLogic()
        {
            if (IsSorted)
            {
                _collider.isTrigger = true;
                _rigidbody.isKinematic = true;
                return;
            }

            CoroutineManager.Run(SleepRoutine());
        }

        #endregion
        
        #region Public

        public void SetIsSorted(bool value) => IsSorted = value;

        #endregion

        #region Coroutines

        private IEnumerator SleepRoutine()
        {
            yield return CoroutineManager.Wait(_sleepDelay.Value);
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        #endregion
        
        #region Overrides

        protected override void OnInteract(GameObject interactor)
        {
            EventManager.Invoke(_onInteractStartEventName.ToString(), this);
        }

        protected override void OnStopInteract(GameObject interactor)
        {
            EventManager.Invoke(_onInteractStopEventName.ToString(), this);
        }

        #endregion
    }
}