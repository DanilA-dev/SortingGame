using D_Dev.InteractableSystem;
using D_Dev.PolymorphicValueSystem;
using D_Dev.TagSystem;
using D_Dev.TagSystem.Extensions;
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
            if(!IsItemExists())
                return;

            if (!_currentActiveItem.Value.HasTag(_slotTag))
            {
                ShowItemMesh();
                SetBlockMaterial();
            }
        }

        protected override void OnUnfocus(GameObject interactor)
        {
            if(!IsItemExists())
                return;

            HideView();
        }

      

        protected override void OnInteract(GameObject interactor)
        {
            if(!CanPlaceItem)
                return;
            
        }

        #endregion

        #region Private

        private void SetBlockMaterial()
        {
            if(!IsItemExists())
                return;

            _meshRenderer.sharedMaterial = _blockPlaceMat;
        }

        private void SetAllowMaterial()
        {
            if(!IsItemExists())
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
            if(!IsItemExists())
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

        private bool IsItemExists() => _currentActiveItem != null && _currentActiveItem.Value != null;

        #endregion
    }
}