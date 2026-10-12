using D_Dev.InventorySystem.Extensions;
using D_Dev.MenuHandler;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InventorySystem.UI
{
    public class InventoryMenu : BaseMenu
    {
        #region Fields

        [Title("Inventory")]
        [SerializeReference] private PolymorphicValue<Inventory> _inventory = new InventoryScriptableVariableValue();
        [SerializeField, Required] private InventorySlotsController _slotsController;

        private Inventory _overrideInventory;

        #endregion

        #region Properties

        public Inventory CurrentInventory => _overrideInventory != null ? _overrideInventory : _inventory?.Value;
        public InventorySlotsController SlotsController => _slotsController;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_inventory != null)
                _inventory.OnValueChanged += OnInventoryChanged;

            Rebind();
        }

        private void OnDisable()
        {
            if (_inventory != null)
                _inventory.OnValueChanged -= OnInventoryChanged;

            _slotsController.Unbind();
        }

        #endregion

        #region Public

        public void Open(Inventory inventory)
        {
            SetInventory(inventory);
            Open();
        }

        public void SetInventory(Inventory inventory)
        {
            _overrideInventory = inventory;

            if (isActiveAndEnabled)
                Rebind();
        }

        public void ResetInventory() => SetInventory(null);

        #endregion

        #region Private

        private void Rebind() => _slotsController.Bind(CurrentInventory);

        #endregion

        #region Listeners

        private void OnInventoryChanged(Inventory inventory)
        {
            if (_overrideInventory == null && isActiveAndEnabled)
                Rebind();
        }

        #endregion
    }
}
