using System.Collections;
using D_Dev.CoroutineManagerSystem;
using D_Dev.InteractableSystem;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace _Project.Scripts
{
    public class ItemInteractable : BaseInteractable
    {
        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeReference] private PolymorphicValue<float> _sleepDelay = new FloatConstantValue();
        
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
            Debug.Log($"interaction start, name is {interactor.name}");
        }

        protected override void OnStopInteract(GameObject interactor)
        {
            Debug.Log($"interaction end, name is {interactor.name}");
        }

        #endregion
    }
}