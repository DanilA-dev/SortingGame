using System.Collections.Generic;
using D_Dev.Base;
using D_Dev.CustomEventManager;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class PickedItemsContainer : MonoBehaviour
    {
        #region Fields

        [Title("Base Settings")]
        [SerializeField] private Transform _rootPoint;
        [SerializeReference] private Vector3 _itemStep;
        [SerializeReference] private PolymorphicValue<int> _capacity = new IntConstantValue();
        [Title("Events Variables")] 
        [SerializeField] private StringScriptableVariable _onItemInteractStartEventName;
        [SerializeField] private StringScriptableVariable _onItemInteractStopEventName;

        [FoldoutGroup("Item Animation Tween")] 
        [SerializeReference] private PolymorphicValue<float> _duration = new FloatConstantValue();
        [FoldoutGroup("Events")]
        public UnityEvent OnFullCapacity;

        private List<IInteractable> _currentItems = new();
        private Stack<Vector3> _steps;
        private Vector3 _lastStep;
        

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            EventManager.AddListener<IInteractable>(_onItemInteractStartEventName.ToString(), OnItemStartInteraction);
            InitPositions();
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<IInteractable>(_onItemInteractStartEventName.ToString(), OnItemStartInteraction);
        }

        #endregion

        #region Listeners

        private void OnItemStartInteraction(IInteractable interactable)
        {
            if(!_currentItems.Contains(interactable))
                _currentItems.Add(interactable);

            TryPopulateContainer(interactable);
        }

        #endregion

        #region Private
        private void InitPositions()
        {
            _lastStep = _rootPoint.position;
            for (int i = 0; i < _capacity.Value; i++)
            {
                var nextStep = _lastStep + _itemStep;
                _steps.Push(nextStep);
            }
        }
        
        private void TryPopulateContainer(IInteractable interactable)
        {
            if (_currentItems.Count >= _capacity.Value)
            {
                OnFullCapacity?.Invoke();
                return;
            }

            var pos = _steps.Pop();
        }

        #endregion
    }
}