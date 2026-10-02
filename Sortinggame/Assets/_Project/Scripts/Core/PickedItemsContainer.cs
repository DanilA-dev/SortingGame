using System.Collections.Generic;
using System.Linq;
using D_Dev.Base;
using D_Dev.CustomEventManager;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;
using D_Dev.ScriptableVariables;
using DG.Tweening;
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
        [SerializeField] private Vector3 _defaultItemStep = new(0, 0.3f, 0);
        [SerializeReference] private PolymorphicValue<int> _absoluteMaxCapaicty = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _currentMaxCapacity = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _currentItemsAmount = new IntConstantValue();
        [Title("Variables")]
        [SerializeField] private StringScriptableVariable _onItemInteractStartEventName;

        [SerializeField] private StringScriptableVariable _itemPickLocalRotationId;
        [SerializeField] private StringScriptableVariable _itemPickStepVariableId;
        
        [SerializeReference] private PolymorphicValue<GameObject> _selectedItem = new GameObjectConstantValue();

        [FoldoutGroup("Item Animation Tween")]
        [SerializeReference] private PolymorphicValue<float> _duration = new FloatConstantValue();
        [FoldoutGroup("Item Animation Tween")]
        [SerializeReference] private PolymorphicValue<float> _jumpPower = new FloatConstantValue();
        [FoldoutGroup("Item Animation Tween")]
        [SerializeReference] private PolymorphicValue<int> _jumpsNum = new IntConstantValue();
        [FoldoutGroup("Item Animation Tween")]
        [SerializeField] private Ease _jumpEase;
        [FoldoutGroup("Events")]
        public UnityEvent OnFullCapacity;
        [FoldoutGroup("Events")]
        public UnityEvent OnItemPicked;
        [FoldoutGroup("Events")]
        public UnityEvent OnItemDropped;

        private List<GameObjectSlot> _createdSlots = new();

        private Dictionary<GameObjectSlot, IInteractable> _pickedItems = new();
        private Dictionary<IInteractable, Vector3> _itemSteps = new();

        private Vector3 _itemPickedLocalRotation;
        
        #endregion

        #region Properties

        private int Capacity => Mathf.Min(_currentMaxCapacity.Value, _createdSlots.Count);

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            InitPositions();
        }

        private void OnEnable()
        {
            if (_onItemInteractStartEventName == null)
            {
                Debug.LogError($"[PickedItemsContainer : {gameObject.name}] Event name variable is not assigned");
                return;
            }

            EventManager.AddListener<IInteractable>(_onItemInteractStartEventName.ToString(), OnItemStartInteraction);
        }

        private void OnDisable()
        {
            if (_onItemInteractStartEventName == null)
                return;

            EventManager.RemoveListener<IInteractable>(_onItemInteractStartEventName.ToString(), OnItemStartInteraction);
        }

        private void OnDestroy()
        {
            foreach (var item in _pickedItems.Values)
            {
                if (item?.GameObject != null)
                    DOTween.Kill(item.GameObject.transform);
            }
        }

        #endregion

        #region Public

        public void TryRemoveLastItem()
        {
            TryRemoveItem(GetLastBusySlot());
        }

        public void TryRemoveFirstItem()
        {
            if (TryRemoveItem(GetFirstBusySlot()))
                ShiftItemsDown();
        }

        public void ForceRemoveFirstItem()
        {
            var slot = GetFirstBusySlot();
            if (slot == null)
                return;

            DOTween.Kill(_pickedItems[slot].GameObject.transform);
            if (TryRemoveItem(slot))
                ShiftItemsDown();
        }

        public void OnDropInput(bool isPressed)
        {
            if (isPressed)
                TryRemoveFirstItem();
        }

        public IInteractable GetLastItem()
        {
            var slot = GetLastBusySlot();
            return slot != null ? _pickedItems[slot] : null;
        }

        public IInteractable GetFirstItem()
        {
            var slot = GetFirstBusySlot();
            return slot != null ? _pickedItems[slot] : null;
        }

        #endregion

        #region Listeners

        private void OnItemStartInteraction(IInteractable interactable)
        {
            TryPickItem(interactable);
        }

        #endregion

        #region Private
        private void InitPositions()
        {
            var root = _rootPoint != null ? _rootPoint : transform;

            foreach (var point in _createdSlots)
            {
                if (point != null)
                    Destroy(point.gameObject);
            }
            _createdSlots.Clear();

            for (int i = 0; i < _absoluteMaxCapaicty.Value; i++)
            {
                GameObject newItemPoint = new GameObject("ItemStep");
                var slot = newItemPoint.AddComponent<GameObjectSlot>();
                newItemPoint.transform.SetParent(root, false);
                newItemPoint.transform.localPosition = _defaultItemStep * i;
                _createdSlots.Add(slot);
            }
        }
        
        private GameObjectSlot GetFirstBusySlot() => _createdSlots.FirstOrDefault(s => s.IsBusy);
        private GameObjectSlot GetLastBusySlot() => _createdSlots.LastOrDefault(s => s.IsBusy);
        private GameObjectSlot GetFirstFreeSlot() => _createdSlots.Take(Capacity).FirstOrDefault(s => !s.IsBusy);

        private Vector3 GetSlotPosition(int index)
        {
            if (index == 0)
                return Vector3.zero;

            var prevSlot = _createdSlots[index - 1];
            var step = _pickedItems.TryGetValue(prevSlot, out var prevItem) && _itemSteps.TryGetValue(prevItem, out var s)
                ? s
                : _defaultItemStep;

            return prevSlot.transform.localPosition + step;
        }

        private bool TryRemoveItem(GameObjectSlot slot)
        {
            if (slot == null)
                return false;

            var item = _pickedItems[slot];
            if (DOTween.IsTweening(item.GameObject.transform))
                return false;

            FreeSlot(slot);
            _itemSteps.Remove(item);
            item.StopInteract(gameObject);
            UpdateSelectedItem();
            
            if(_currentItemsAmount.Value > 0)
                _currentItemsAmount.Value--;
            
            OnItemDropped?.Invoke();
            return true;
        }

        private bool TryPutToSlot(GameObjectSlot slot, IInteractable item)
        {
            if (!slot.TryPutItem(item.GameObject, true))
                return false;

            _pickedItems[slot] = item;
            return true;
        }

        private void FreeSlot(GameObjectSlot slot)
        {
            slot.FreeSlot();
            _pickedItems.Remove(slot);
        }

        private void ShiftItemsDown()
        {
            int targetIndex = 0;
            for (int i = 0; i < _createdSlots.Count; i++)
            {
                var slot = _createdSlots[i];
                if (!slot.IsBusy)
                    continue;

                if (i != targetIndex)
                {
                    var item = _pickedItems[slot];
                    FreeSlot(slot);
                    var targetSlot = _createdSlots[targetIndex];
                    targetSlot.transform.localPosition = GetSlotPosition(targetIndex);
                    if (TryPutToSlot(targetSlot, item))
                        AnimateShift(item);
                }
                targetIndex++;
            }
            UpdateSelectedItem();
        }

        private void UpdateSelectedItem()
        {
            var first = GetFirstItem();
            _selectedItem.Value = null;
            _selectedItem.Value = first != null ? first.GameObject : null;
        }

        private void TryPickItem(IInteractable interactable)
        {
            if (interactable == null || _pickedItems.ContainsValue(interactable))
                return;

            var slot = GetFirstFreeSlot();
            if (slot == null)
            {
                if (interactable is ItemInteractable item)
                    item.CancelPick();

                OnFullCapacity?.Invoke();
                return;
            }

            slot.transform.localPosition = GetSlotPosition(_createdSlots.IndexOf(slot));

            if (!TryPutToSlot(slot, interactable))
                return;

            ReadItemVariables(interactable);
            AnimatePick(interactable);
            UpdateSelectedItem();
            
            if (_currentItemsAmount.Value < _currentMaxCapacity.Value)
                _currentItemsAmount.Value++;
            
            OnItemPicked?.Invoke();
        }

        private void ReadItemVariables(IInteractable interactable)
        {
            _itemSteps[interactable] = _defaultItemStep;

            if (!interactable.GameObject.TryGetComponent(out RuntimeEntityVariablesContainer container))
                return;

            if (container.TryGetVariable<Vector3EntityVariable>(_itemPickLocalRotationId, out var rotVariable))
                _itemPickedLocalRotation = rotVariable.Value.Value;

            if (container.TryGetVariable<Vector3EntityVariable>(_itemPickStepVariableId, out var stepVariable))
                _itemSteps[interactable] = stepVariable.Value.Value;
        }

        private void AnimatePick(IInteractable interactable)
        {
            var itemTransform = interactable.GameObject.transform;
            DOTween.Kill(itemTransform);

            DOTween.Sequence()
                .Join(itemTransform.DOLocalJump(Vector3.zero, _jumpPower.Value, _jumpsNum.Value, _duration.Value)
                    .SetEase(_jumpEase))
                .Join(itemTransform.DOLocalRotate(_itemPickedLocalRotation, _duration.Value))
                .SetTarget(itemTransform)
                .SetAutoKill(true);
        }

        private void AnimateShift(IInteractable interactable)
        {
            var itemTransform = interactable.GameObject.transform;
            DOTween.Kill(itemTransform);

            DOTween.Sequence()
                .Join(itemTransform.DOLocalMove(Vector3.zero, _duration.Value).SetEase(Ease.OutQuad))
                .SetTarget(itemTransform)
                .SetAutoKill(true);
        }

        #endregion
    }
}
