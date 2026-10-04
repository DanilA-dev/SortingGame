using System.Linq;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class ItemSlotsContainer : MonoBehaviour
    {
        #region Fields

        [Title("Base Settings")]
        [SerializeReference] private PolymorphicValue<EntityInfo> _itemInfo = new EntityInfoConstantValue();
        
        [SerializeField] private Transform _root;
        [SerializeReference] private PolymorphicValue<bool> _isSorted = new BoolConstantValue();
        
        [Space]
        [Title("Variables")]
        [SerializeField] private IntScriptableVariable _currentSortedShelvesVariable;
        
        [Space]
        [FoldoutGroup("Events")] 
        public UnityEvent OnAllSlotsTaken;

        private ItemSlotInteractable[] _slots;
        private bool _isSortedApplied;

        #endregion

        #region Properties

        public ItemSlotInteractable[] Slots => _slots;
        public EntityInfo ItemInfo => _itemInfo.Value;
        public bool IsSorted => _isSorted.Value;

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
        }

        

        private void OnDestroy()
        {
            UnsubscribeFromSlotsEvents();
        }

        #endregion

        #region Public

        public bool TryGetSlotIndex(GameObject item, out int index)
        {
            index = -1;
            if (_slots == null || item == null)
                return false;

            index = System.Array.FindIndex(_slots, s => s.Item == item);
            return index >= 0;
        }

        public bool TryRestoreItem(int slotIndex, GameObject item)
        {
            if (_slots == null || slotIndex < 0 || slotIndex >= _slots.Length)
                return false;

            if (!_slots[slotIndex].TryPlaceItemImmediate(item))
                return false;

            if (_slots.All(s => s.IsBusy))
                SetSorted();

            return true;
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
            if (_isSortedApplied)
                return;

            _isSortedApplied = true;
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