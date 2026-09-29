using System;
using System.Linq;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using D_Dev.TagSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class ItemSlotsContainer : MonoBehaviour
    {
        #region Fields

        [Title("Base Settings")]
        [SerializeField] private Tag _itemTag;
        [SerializeField] private Transform _root;
        [SerializeReference] private PolymorphicValue<bool> _isSorted = new BoolConstantValue();
        
        [Space]
        [Title("Variables")]
        [SerializeField] private IntScriptableVariable _maxSortedShelvesVariable;
        [SerializeField] private IntScriptableVariable _currentSortedShelvesVariable;
        
        [Space]
        [FoldoutGroup("Events")] 
        public UnityEvent OnAllSlotsTaken;

        private ItemSlotInteractable[] _slots;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _slots = _root.GetComponentsInChildren<ItemSlotInteractable>();
            InitSlots();
        }

        private void Start()
        {
            if(_isSorted.Value)
                SetSorted();
            else
                _maxSortedShelvesVariable.Value++;
        }

        private void OnDestroy()
        {
            UnsubscribeFromSlotsEvents();
        }

        #endregion

        #region Private

        private void InitSlots()
        {
            if(_slots == null)
                return;

            foreach (var itemSlotInteractable in _slots)
            {
                itemSlotInteractable.Init(_itemTag);
                itemSlotInteractable.OnItemSet.AddListener(CheckSlotsState);
            }
        }

        private void UnsubscribeFromSlotsEvents()
        {
            if(_slots == null)
                return;

            foreach (var itemSlotInteractable in _slots)
                itemSlotInteractable.OnItemSet.RemoveListener(CheckSlotsState);
        }

        private void SetSorted()
        {
            _currentSortedShelvesVariable.Value++;
            _isSorted.Value = true;
        }
        
        #endregion

        #region Listeners

        private void CheckSlotsState()
        {
            if (_slots.All(s => s.IsBusy))
            {
                OnAllSlotsTaken?.Invoke();
                SetSorted();
            }
        }

        #endregion
    }
}