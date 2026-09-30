using System;
using System.Linq;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class ItemSlotsContainer : MonoBehaviour
    {
        #region Fields

        [Title("Base Settings")]
        [SerializeReference] private PolymorphicValue<EntityInfo> _itemInfo = new EntityInfoConstantValue();
        [SerializeField] private TMP_Text _nameText;
        [SerializeReference] private PolymorphicValue<string> _shelfName = new StringConstantValue();
        
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

            SetInfo();
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
                itemSlotInteractable.Init(_itemInfo.Value);
                itemSlotInteractable.OnItemPlaced.AddListener(CheckSlotsState);
            }
        }

        private void UnsubscribeFromSlotsEvents()
        {
            if(_slots == null)
                return;

            foreach (var itemSlotInteractable in _slots)
                itemSlotInteractable.OnItemPlaced.RemoveListener(CheckSlotsState);
        }

        private void SetSorted()
        {
            _currentSortedShelvesVariable.Value++;
            _isSorted.Value = true;
        }
        
        private void SetInfo()
        {
            _nameText.SetText(_shelfName.Value);   
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