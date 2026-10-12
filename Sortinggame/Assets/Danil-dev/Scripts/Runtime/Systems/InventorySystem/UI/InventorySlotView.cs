using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace D_Dev.InventorySystem.UI
{
    public class InventorySlotView : MonoBehaviour
    {
        #region Fields

        [Title("Components")]
        [SerializeField, Required] private InventoryItemView _itemView;
        [SerializeField] private Button _button;

        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent OnFilled;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent OnEmptied;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent OnSelectedEvent;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent OnDeselectedEvent;

        private IInventoryCell _cell;
        private bool? _isFilled;

        public event Action<InventorySlotView> OnClicked;

        #endregion

        #region Properties

        public IInventoryCell Cell => _cell;
        public InventoryItem Item => _cell?.Data ?? default;
        public bool IsEmpty => _cell == null || _cell.IsEmpty;
        public bool IsSelected { get; private set; }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClicked);

            Unbind();
        }

        #endregion

        #region Public

        public void Bind(IInventoryCell cell)
        {
            Unbind();

            _cell = cell;
            if (_cell != null)
                _cell.OnContentChanged += Refresh;

            Refresh(Item);
        }

        public void Unbind()
        {
            if (_cell != null)
                _cell.OnContentChanged -= Refresh;

            _cell = null;
            _isFilled = null;
        }

        public void Select()
        {
            if (IsSelected)
                return;

            IsSelected = true;
            OnSelectedEvent?.Invoke();
        }

        public void Deselect()
        {
            if (!IsSelected)
                return;

            IsSelected = false;
            OnDeselectedEvent?.Invoke();
        }

        #endregion

        #region Private

        private void Refresh(InventoryItem item)
        {
            var isFilled = !item.IsEmpty;
            _itemView.gameObject.SetActive(isFilled);

            _itemView.Set(item);

            if (_isFilled == isFilled)
                return;

            _isFilled = isFilled;

            if (isFilled)
                OnFilled?.Invoke();
            else
                OnEmptied?.Invoke();
        }

        #endregion

        #region Listeners

        private void OnButtonClicked() => OnClicked?.Invoke(this);

        #endregion
    }
}
