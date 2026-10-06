using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using D_Dev.Base;
using D_Dev.CustomEventManager;
using D_Dev.EntityInfoBinder;
using D_Dev.RuntimeLists;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    public class LevelStateController : MonoBehaviour
    {
        #region Fields

        [Title("References")]
        [SerializeField] private SlotsBasedItemSpawner _spawner;
        [SerializeField] private GameObjectRuntimeList _shelvesRuntimeList;
        [Title("Settings")]
        [SerializeField, Min(0f)] private float _pickedContainerWaitTimeout = 10f;
        [Title("Change Tracking")]
        [SerializeField] private IntScriptableVariable _sortedItemsVariable;
        [SerializeField] private StringScriptableVariable _itemPickedEventName;
        [SerializeField] private StringScriptableVariable _itemDroppedEventName;
        [PropertySpace(15)]
        [SerializeField] private bool _debug;

        private readonly Dictionary<string, ItemSlotsContainer> _shelvesById = new();
        private LevelSaveData _loadedData;
        private bool _isRestoreStarted;

        public event Action OnStateChanged;

        #endregion

        #region Properties

        [ShowInInspector, ReadOnly]
        public bool IsRestored { get; private set; }

        public bool CanCapture => IsRestored && AreSpawnedItemsAlive();

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_sortedItemsVariable != null)
                _sortedItemsVariable.OnValueUpdate += OnSortedItemsUpdate;

            if (_itemPickedEventName != null)
                EventManager.AddListener<IInteractable>(_itemPickedEventName.ToString(), OnItemInteracted);

            if (_itemDroppedEventName != null)
                EventManager.AddListener<IInteractable>(_itemDroppedEventName.ToString(), OnItemInteracted);
        }

        private void OnDisable()
        {
            if (_sortedItemsVariable != null)
                _sortedItemsVariable.OnValueUpdate -= OnSortedItemsUpdate;

            if (_itemPickedEventName != null)
                EventManager.RemoveListener<IInteractable>(_itemPickedEventName.ToString(), OnItemInteracted);

            if (_itemDroppedEventName != null)
                EventManager.RemoveListener<IInteractable>(_itemDroppedEventName.ToString(), OnItemInteracted);
        }

        #endregion

        #region Public

        public void SetLoadedData(LevelSaveData data) => _loadedData = data;

        public void Restore()
        {
            if (_isRestoreStarted)
                return;

            _isRestoreStarted = true;
            RestoreAsync(destroyCancellationToken).Forget();
        }

        public void MarkChanged()
        {
            if (IsRestored)
                OnStateChanged?.Invoke();
        }

        public LevelSaveData Capture()
        {
            var items = new List<ItemSaveData>(_spawner.SpawnedItems.Count);

            foreach (var item in _spawner.SpawnedItems)
            {
                if (item != null && TryCaptureItem(item, out var itemData))
                    items.Add(itemData);
            }

            if (_debug)
                Debug.Log($"[LevelStateController] Captured {items.Count} items");

            return LevelSaveData.Pack(items);
        }

        #endregion

        #region Private

        private async UniTaskVoid RestoreAsync(CancellationToken token)
        {
            try
            {
                var savedItems = _loadedData?.Unpack();

                if (savedItems == null || savedItems.Count == 0)
                {
                    if (_debug)
                        Debug.Log("[LevelStateController] No saved state, spawning random items");

                    await _spawner.SpawnItemsAsync();
                }
                else
                {
                    CacheShelves();

                    var pickedItems = new List<ItemSaveData>();
                    await _spawner.SpawnItemsAsync(() => CreateSavedItemsAsync(savedItems, pickedItems));
                    await RestorePickedItemsAsync(pickedItems, token);

                    if (_debug)
                        Debug.Log($"[LevelStateController] Restored {savedItems.Count} items");
                }

                IsRestored = true;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask<List<GameObject>> CreateSavedItemsAsync(List<ItemSaveData> savedItems, List<ItemSaveData> pickedItems)
        {
            var freeItems = new List<GameObject>();

            foreach (var itemData in savedItems)
            {
                if (itemData == null)
                    continue;

                if (itemData.State == ItemSaveState.Picked)
                {
                    pickedItems.Add(itemData);
                    continue;
                }

                var item = await CreateItemAsync(itemData);
                if (item == null)
                    continue;

                if (itemData.State == ItemSaveState.Sorted && TryRestoreSortedItem(item, itemData))
                    continue;

                if (itemData.HasTransform)
                    item.transform.SetPositionAndRotation(itemData.Position, itemData.Rotation);

                freeItems.Add(item);
            }

            return freeItems;
        }

        private async UniTask RestorePickedItemsAsync(List<ItemSaveData> pickedItems, CancellationToken token)
        {
            if (pickedItems.Count == 0)
                return;

            pickedItems.Sort((a, b) => a.PickedIndex.CompareTo(b.PickedIndex));
            var container = await WaitForPickedItemsContainerAsync(token);

            await _spawner.SpawnItemsAsync(async () =>
            {
                var droppedItems = new List<GameObject>();

                foreach (var itemData in pickedItems)
                {
                    var item = await CreateItemAsync(itemData);
                    if (item == null)
                        continue;

                    if (container != null && TryRestorePickedItem(item, container))
                        continue;

                    if (itemData.HasTransform)
                        item.transform.SetPositionAndRotation(itemData.Position, itemData.Rotation);

                    droppedItems.Add(item);
                }

                return droppedItems;
            });
        }

        private async UniTask<GameObject> CreateItemAsync(ItemSaveData itemData)
        {
            if (!_spawner.TryGetItemInfo(itemData.InfoId, out var itemInfo))
            {
                Debug.LogWarning($"[LevelStateController] Unknown item id '{itemData.InfoId}', item skipped");
                return null;
            }

            return await _spawner.CreateItemAsync(itemInfo);
        }

        private async UniTask<PickedItemsContainer> WaitForPickedItemsContainerAsync(CancellationToken token)
        {
            var elapsed = 0f;

            while (true)
            {
                var container = FindAnyObjectByType<PickedItemsContainer>();
                if (container != null)
                    return container;

                if (elapsed >= _pickedContainerWaitTimeout)
                {
                    Debug.LogWarning("[LevelStateController] PickedItemsContainer not found, picked items will be dropped");
                    return null;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
                elapsed += Time.unscaledDeltaTime;
            }
        }

        private bool TryRestoreSortedItem(GameObject item, ItemSaveData itemData)
        {
            return !string.IsNullOrEmpty(itemData.ShelfId)
                   && _shelvesById.TryGetValue(itemData.ShelfId, out var shelf)
                   && shelf.TryRestoreItem(itemData.SlotIndex, item);
        }

        private bool TryRestorePickedItem(GameObject item, PickedItemsContainer container)
        {
            if (!item.TryGetComponent(out ItemInteractable interactable))
                return false;

            item.transform.position = container.transform.position;
            interactable.StartInteract(container.gameObject);
            return interactable.IsPicked;
        }

        private bool TryCaptureItem(GameObject item, out ItemSaveData itemData)
        {
            itemData = null;

            if (!item.TryGetComponent(out EntityInfoBinder binder) || binder.Info == null)
                return false;

            if (!item.TryGetComponent(out ItemInteractable interactable))
                return false;

            var itemTransform = item.transform;
            itemData = new ItemSaveData
            {
                InfoId = binder.Info.ID,
                State = ItemSaveState.Free,
                Position = itemTransform.position,
                Rotation = itemTransform.rotation
            };

            if (interactable.IsSorted)
            {
                var shelf = item.GetComponentInParent<ItemSlotsContainer>();
                if (shelf != null && shelf.ItemInfo != null && shelf.TryGetSlotIndex(item, out var slotIndex))
                {
                    itemData.State = ItemSaveState.Sorted;
                    itemData.ShelfId = shelf.ItemInfo.ID;
                    itemData.SlotIndex = slotIndex;
                }

                return true;
            }

            if (interactable.IsPicked)
            {
                var container = item.GetComponentInParent<PickedItemsContainer>();
                if (container != null && container.TryGetItemIndex(item, out var pickedIndex))
                {
                    itemData.State = ItemSaveState.Picked;
                    itemData.PickedIndex = pickedIndex;
                }
            }

            return true;
        }

        private void CacheShelves()
        {
            _shelvesById.Clear();

            if (_shelvesRuntimeList == null)
                return;

            foreach (var shelfObject in _shelvesRuntimeList.Items)
            {
                if (shelfObject == null || !shelfObject.TryGetComponent(out ItemSlotsContainer shelf))
                    continue;

                var itemInfo = shelf.ItemInfo;
                if (itemInfo != null && !string.IsNullOrEmpty(itemInfo.ID))
                    _shelvesById.TryAdd(itemInfo.ID, shelf);
            }
        }

        private bool AreSpawnedItemsAlive()
        {
            foreach (var item in _spawner.SpawnedItems)
            {
                if (item == null)
                    return false;
            }

            return true;
        }

        #endregion

        #region Listeners

        private void OnSortedItemsUpdate(int sortedItems) => MarkChanged();

        private void OnItemInteracted(IInteractable item) => MarkChanged();

        #endregion
    }
}
