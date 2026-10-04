using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.EntityInfoBinder;
using D_Dev.EntityVariable.Types;
using D_Dev.InteractableSystem;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using D_Dev.TweenAnimations.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class ItemSlotInteractable : BaseInteractable
    {
        #region Fields

        [SerializeField] private GameObjectSlot _gameObjectSlot;
        [SerializeField] private Transform _itemView;
        [SerializeReference] private PolymorphicValue<GameObject> _currentActiveItem = new GameObjectConstantValue();
        [SerializeReference] private PolymorphicValue<Vector3> _currentActiveItemOffset = new Vector3ConstantValue();
        [SerializeReference] private PolymorphicValue<Vector3> _currentActiveItemRotation = new Vector3ConstantValue();
        [Title("Item Place Materials")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _allowPlaceMat;
        [SerializeField] private Material _blockPlaceMat;
        [Space]
        [FoldoutGroup("Item Set Animation")] 
        [SerializeField] private MoveAnimationTween _moveAnimationTween;

        [Space]
        [FoldoutGroup("Slot Events")]
        public UnityEvent OnItemPlacing;
        [FoldoutGroup("Slot Events")]
        public UnityEvent OnItemPlaced;
        
        private EntityInfo _slotItemInfo;
        
        private Vector3 _itemSlotLocalRotation;
        
        private Vector3 _defaultPos;
        private Vector3 _defaultEulerAngles;

        #endregion

        #region Properties

        public bool IsBusy => _gameObjectSlot.IsBusy;
        public GameObject Item => _gameObjectSlot.Item;
        public bool CanPlaceItem { get; private set; }

        #endregion

        #region Public

        public void Init(EntityInfo itemInfo)
        {
            _slotItemInfo = itemInfo;

            _defaultPos = _itemView.localPosition;
            _defaultEulerAngles = _itemView.localEulerAngles;

            HideView();
        }

        public bool TryPlaceItemImmediate(GameObject item)
        {
            if (item == null || !_gameObjectSlot.TryPutItem(item, true))
                return false;

            item.transform.localPosition = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;

            if (item.TryGetComponent(out ItemInteractable itemInteractable))
                itemInteractable.SetSorted();

            return true;
        }

        #endregion
        
        #region Overrides

        protected override void OnFocus(GameObject interactor)
        {
            if(IsBusy)
                return;
            
            if(!IsActiveItemExists())
                return;
            
            if (!IsMatchingItem(_currentActiveItem.Value))
            {
                SetBlockMaterial();
                CanPlaceItem = false;
            }
            else
            {
                CanPlaceItem = true;
                SetAllowMaterial();
            }
            
            ShowItemMesh();
            SetItemOffset();
        }

        protected override void OnUnfocus(GameObject interactor)
        {
            if(IsBusy)
                return;
            
            if(!IsActiveItemExists())
                return;

            CanPlaceItem = false;
            HideView();
            ResetItemOffset();
        }

        protected override void OnInteract(GameObject interactor)
        {
            if(IsBusy)
                return;
            
            if(!CanPlaceItem)
                return;

            var item = _currentActiveItem.Value;

            OnItemPlacing?.Invoke();
            _gameObjectSlot.TryPutItem(item, true);
            OnItemPlaced?.Invoke();
            if(item.TryGetComponent(out ItemInteractable itemInteractable))
                itemInteractable.SetSorted();

            item.transform.localEulerAngles = Vector3.zero;
            
            _moveAnimationTween.MovedObjects = new[] { item.transform };
            _moveAnimationTween.MoveType = MoveAnimationTween.MoveObjectType.Vector;
            _moveAnimationTween.PositionEnd = _gameObjectSlot.transform.position;
            _moveAnimationTween.Play();
            
            HideView();
            SetItemOffset();
        }

        #endregion

        #region Private

        private void SetItemOffset()
        {
            _itemView.localPosition = _defaultPos + _currentActiveItemOffset.Value;
            _itemView.localEulerAngles = _currentActiveItemRotation.Value;
        }

        private void ResetItemOffset()
        {
            _itemView.localPosition = _defaultPos;
            _itemView.localEulerAngles = _defaultEulerAngles;
        }
        
        private void SetBlockMaterial()
        {
            if(!IsActiveItemExists())
                return;

            _meshRenderer.sharedMaterial = _blockPlaceMat;
        }

        private void SetAllowMaterial()
        {
            if(!IsActiveItemExists())
                return;

            _meshRenderer.sharedMaterial = _allowPlaceMat;
        }

        private void HideView()
        {
            _meshFilter.sharedMesh = null;
            _meshRenderer.sharedMaterial = null;
        }
        
        private void ShowItemMesh()
        {
            if(!IsActiveItemExists())
                return;

            if (!_currentActiveItem.Value.TryGetComponent(out ItemInteractable item))
                return;

            _meshFilter.sharedMesh = item.Filter.sharedMesh;
        }

        #endregion

        #region Helpers

        private bool IsActiveItemExists() => _currentActiveItem != null && _currentActiveItem.Value != null;

        
        private bool IsMatchingItem(GameObject item)
        {
            return _slotItemInfo != null
                   && item.TryGetComponent(out EntityInfoBinder binder)
                   && binder.Info == _slotItemInfo;
        }

        #endregion
    }
}