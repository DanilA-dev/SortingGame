using D_Dev.Entity;
using D_Dev.EntityInfoBinder;
using D_Dev.InteractableSystem;
using D_Dev.PolymorphicValueSystem;
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
        [SerializeReference] private PolymorphicValue<GameObject> _currentActiveItem = new GameObjectConstantValue();
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
        
        private EntityInfo _slotInfo;
        private MeshFilter _activeItemMeshFilter;
        

        #endregion

        #region Properties

        public bool IsBusy => _gameObjectSlot.IsBusy;
        public bool CanPlaceItem { get; private set; }

        #endregion
        
        #region Public

        public void Init(EntityInfo itemInfo)
        {
            _slotInfo = itemInfo;
            HideView();
        }

        #endregion
        
        #region Overrides

        protected override void OnFocus(GameObject interactor)
        {
            if(IsBusy)
                return;
            
            if(!IsActveItemExists())
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
        }

        protected override void OnUnfocus(GameObject interactor)
        {
            if(IsBusy)
                return;
            
            if(!IsActveItemExists())
                return;

            CanPlaceItem = false;
            HideView();
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
            
            item.transform.localRotation = Quaternion.identity;
            
            _moveAnimationTween.MovedObjects = new[] { item.transform };
            _moveAnimationTween.MoveType = MoveAnimationTween.MoveObjectType.Vector;
            _moveAnimationTween.PositionEnd = _gameObjectSlot.transform.position;
            _moveAnimationTween.Play();
            
            HideView();
        }

        #endregion

        #region Private

        private void SetBlockMaterial()
        {
            if(!IsActveItemExists())
                return;

            _meshRenderer.sharedMaterial = _blockPlaceMat;
        }

        private void SetAllowMaterial()
        {
            if(!IsActveItemExists())
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
            if(!IsActveItemExists())
                return;

            if (!_currentActiveItem.Value.TryGetComponent(out _activeItemMeshFilter))
            {
                Debug.LogError($"Item {_currentActiveItem.Value.name} does not have any mesh filter on it!");
                return;
            }

            _meshFilter.sharedMesh = _activeItemMeshFilter.sharedMesh;
        }

        #endregion

        #region Helpers

        private bool IsActveItemExists() => _currentActiveItem != null && _currentActiveItem.Value != null;

        private bool IsMatchingItem(GameObject item) =>
            _slotInfo != null
            && item.TryGetComponent(out EntityInfoBinder binder)
            && binder.Info == _slotInfo;

        #endregion
    }
}