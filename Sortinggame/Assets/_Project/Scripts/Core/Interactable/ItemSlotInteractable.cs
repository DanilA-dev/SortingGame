using D_Dev.InteractableSystem;
using D_Dev.PolymorphicValueSystem;
using D_Dev.TagSystem;
using D_Dev.TagSystem.Extensions;
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
        public UnityEvent OnItemSet;
        
        private Tag _slotTag;
        private MeshFilter _activeItemMeshFilter;
        

        #endregion

        #region Properties

        public bool IsBusy => _gameObjectSlot.IsBusy;
        public bool CanPlaceItem { get; private set; }

        #endregion
        
        #region Public

        public void Init(Tag itemTag)
        {
            _slotTag = itemTag;
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

            if (!_currentActiveItem.Value.HasTag(_slotTag))
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
            _gameObjectSlot.TryPutItem(item, true);
            OnItemSet?.Invoke();

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
            _meshFilter.mesh = null;
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

            _meshFilter.mesh = _activeItemMeshFilter.mesh;
        }

        #endregion

        #region Helpers

        private bool IsActveItemExists() => _currentActiveItem != null && _currentActiveItem.Value != null;

        #endregion
    }
}