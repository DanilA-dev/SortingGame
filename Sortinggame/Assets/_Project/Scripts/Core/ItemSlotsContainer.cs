using System.Linq;
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
                itemSlotInteractable.OnItemSet.AddListener(OnSlotUpdated);
            }
        }

        private void UnsubscribeFromSlotsEvents()
        {
            if(_slots == null)
                return;

            foreach (var itemSlotInteractable in _slots)
                itemSlotInteractable.OnItemSet.RemoveListener(OnSlotUpdated);
        }
        
        #endregion

        #region Listeners

        private void OnSlotUpdated()
        {
            if (_slots.All(s => s.IsBusy))
                OnAllSlotsTaken?.Invoke();
        }

        #endregion
    }
}