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
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeReference] private PolymorphicValue<float> _sleepDelay = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<bool> _isSorted = new BoolConstantValue();
        [Space]
        [Title("Stop Interact Settings")] 
        [SerializeReference] private PolymorphicValue<float> _pushForce = new FloatConstantValue();
        [SerializeField] private ForceMode _forceMode;

        [Space]
        [Title("Variables")]
        [SerializeField] private IntScriptableVariable _currentSortedItemsVariable;
        [Space]
        [SerializeField] private StringScriptableVariable _onInteractStartEventName;
        [SerializeField] private StringScriptableVariable _onInteractStopEventName;

        private Coroutine _sleepRoutine;
        private Collider _collider;
        private bool _isRevealed;
        private bool _isSettled;
        private bool _isSortedApplied;

        #endregion

        #region Properties

        public bool IsPicked { get; private set; }
        public bool IsSorted => _isSorted.Value;

        public MeshRenderer Renderer
        {
            get => _meshRenderer;
            set => _meshRenderer = value;
        }

        public MeshFilter Filter
        {
            get => _meshFilter;
            set => _meshFilter = value;
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            if (_isSorted.Value)
            {
                SetSorted();
                return;
            }

            if (_isSettled || IsPicked)
                return;

            TryStartSleepLogic();
        }

        private void OnDestroy()
        {
            DisableInstancing();
        }

        private void TryStartSleepLogic()
        {
            RestartSleepRoutine();
        }

        #endregion

        #region Public

        public void SetSorted()
        {
            if (_isSortedApplied)
                return;

            _isSortedApplied = true;
            StopSleepRoutine();
            _isSorted.Value = true;
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _currentSortedItemsVariable.Value++;
            EnableInstancing();
        }

        public void SetSettled()
        {
            if (_isSorted.Value || IsPicked)
                return;

            _isSettled = true;
            StopSleepRoutine();
            _rigidbody.isKinematic = true;

            if (!IsFocused && !_isRevealed)
                EnableInstancing();
        }

        public void CancelPick()
        {
            IsPicked = false;
            IsInteracting = false;
            _rigidbody.isKinematic = false;
            RestartSleepRoutine();
        }

        public void SetRevealed(bool isRevealed)
        {
            _isRevealed = isRevealed;
            if (isRevealed)
                DisableInstancing();
            else if (!IsPicked && !IsFocused && _sleepRoutine == null && _rigidbody.isKinematic)
                EnableInstancing();
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

        private void EnableInstancing()
        {
            if (InstancedItemsRenderer.Instance != null)
                InstancedItemsRenderer.Instance.Register(_meshRenderer, _meshFilter);
        }

        private void DisableInstancing()
        {
            if (InstancedItemsRenderer.Instance != null)
                InstancedItemsRenderer.Instance.Unregister(_meshRenderer);
        }

        #endregion

        #region Coroutines

        private IEnumerator SleepRoutine()
        {
            yield return CoroutineManager.Wait(_sleepDelay.Value);
            _rigidbody.isKinematic = true;
            _sleepRoutine = null;
            if (!IsFocused && !_isRevealed)
                EnableInstancing();
        }

        #endregion

        #region Overrides

        protected override void OnFocus(GameObject interactor)
        {
            DisableInstancing();
        }

        protected override void OnUnfocus(GameObject interactor)
        {
            if (!IsPicked && !_isRevealed && _sleepRoutine == null && _rigidbody.isKinematic)
                EnableInstancing();
        }

        protected override void OnInteract(GameObject interactor)
        {
            if(_isSorted.Value || IsPicked)
                return;
            
            IsPicked = true;
            DisableInstancing();
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