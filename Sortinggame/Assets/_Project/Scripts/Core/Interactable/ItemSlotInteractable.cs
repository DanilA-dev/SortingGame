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
        [Title("Item Place Materials")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _allowPlaceMat;
        [SerializeField] private Material _blockPlaceMat;

        [Title("Item Settings")] 
        [SerializeField] private StringScriptableVariable _itemRotationVariableId;
        [SerializeField] private StringScriptableVariable _itemOffsetVariableId;
        [Space]
        [FoldoutGroup("Item Set Animation")] 
        [SerializeField] private MoveAnimationTween _moveAnimationTween;

        [Space]
        [FoldoutGroup("Slot Events")]
        public UnityEvent OnItemPlacing;
        [FoldoutGroup("Slot Events")]
        public UnityEvent OnItemPlaced;
        
        private EntityInfo _itemInfo;
        private Vector3 _itemSlotLocalRotation;
        private Vector3 _initPos;
        private Vector3 _itemSlotOffset;

        #endregion

        #region Properties

        public bool IsBusy => _gameObjectSlot.IsBusy;
        public bool CanPlaceItem { get; private set; }

        #endregion
        
        #region Public

        public void Init(EntityInfo itemInfo)
        {
            _itemInfo = itemInfo;

            _initPos = _itemView.localPosition;
            GetItemData();
            HideView();
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
            _itemView.localPosition = _initPos + _itemSlotOffset;
        }

        private void ResetItemOffset()
        {
            _itemView.localPosition = _initPos;
        }
        
        private void GetItemData()
        {
            var rotationVariable = _itemInfo.GetVariable<Vector3EntityVariable>(_itemRotationVariableId);
            var offsetVariable = _itemInfo.GetVariable<Vector3EntityVariable>(_itemOffsetVariableId);
            if (rotationVariable != null)
            {
                _itemSlotLocalRotation = rotationVariable.Value.Value;
                transform.localEulerAngles = _itemSlotLocalRotation;
            }

            if (offsetVariable != null)
                _itemSlotOffset = offsetVariable.Value.Value;
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

        private bool IsMatchingItem(GameObject item) =>
            _itemInfo != null
            && item.TryGetComponent(out EntityInfoBinder binder)
            && binder.Info == _itemInfo;

        #endregion
    }
}