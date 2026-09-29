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
        [SerializeReference] private PolymorphicValue<bool> _isSorted = new BoolConstantValue();
        [Space]
        [Title("Stop Interact Settings")] 
        [SerializeReference] private PolymorphicValue<float> _pushForce = new FloatConstantValue();
        [SerializeField] private ForceMode _forceMode;

        [Space]
        [Title("Variables")]
        [SerializeField] private IntScriptableVariable _maxSortedItemsVariable;
        [SerializeField] private IntScriptableVariable _currentSortedItemsVariable;
        [Space]
        [SerializeField] private StringScriptableVariable _onInteractStartEventName;
        [SerializeField] private StringScriptableVariable _onInteractStopEventName;

        private Coroutine _sleepRoutine;
        
        #endregion

        #region Properties

        public bool IsPicked { get; private set; }

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if (_isSorted.Value)
            {
                SetSorted();
                return;
            }

            _maxSortedItemsVariable.Value++;
            TryStartSleepLogic();
        }

        private void TryStartSleepLogic()
        {
            RestartSleepRoutine();
        }

        #endregion

        #region Public

        public void SetSorted()
        {
            _isSorted.Value = true;
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _currentSortedItemsVariable.Value++;
        }

        public void CancelPick()
        {
            IsPicked = false;
            IsInteracting = false;
            _rigidbody.isKinematic = false;
            RestartSleepRoutine();
        }

        #endregion

        #region Private

        private void RestartSleepRoutine()
        {
            StopSleepRoutine();
            _sleepRoutine = CoroutineManager.Run(SleepRoutine());
        }

        private void StopSleepRoutine()
        {
            CoroutineManager.Stop(_sleepRoutine);
            _sleepRoutine = null;
        }

        #endregion

        #region Coroutines

        private IEnumerator SleepRoutine()
        {
            yield return CoroutineManager.Wait(_sleepDelay.Value);
            _rigidbody.isKinematic = true;
            _sleepRoutine = null;
        }

        #endregion
        
        #region Overrides

        protected override void OnInteract(GameObject interactor)
        {
            if(_isSorted.Value || IsPicked)
                return;
            
            IsPicked = true;
            StopSleepRoutine();
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }
            EventManager.Invoke(_onInteractStartEventName.ToString(), this);
        }

        protected override void OnStopInteract(GameObject interactor)
        {
            if(_isSorted.Value || !IsPicked)
                return;
            
            EventManager.Invoke(_onInteractStopEventName.ToString(), this);
            transform.SetParent(null);
            
            var dir = (interactor.transform.position - transform.position).normalized;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(transform.forward * _pushForce.Value, _forceMode);

            IsPicked = false;
            RestartSleepRoutine();
        }

        #endregion
    }
}