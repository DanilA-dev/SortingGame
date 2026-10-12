using D_Dev.Entity.Extensions;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.InventorySystem.UI
{
    public class InventoryItemView : MonoBehaviour
    {
        #region Fields

        [Title("Components")]
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _amount;
        [Title("Variables")]
        [SerializeField] private StringScriptableVariable _nameVariableId;
        [SerializeField] private StringScriptableVariable _iconVariableId;
        [Title("Amount")]
        [SerializeField] private string _amountFormat = "x{0}";

        #endregion

        #region Properties

        public InventoryItem Item { get; private set; }

        #endregion

        #region Public

        public void Set(InventoryItem item)
        {
            if (item.IsEmpty)
            {
                Clear();
                return;
            }

            Item = item;
            SetIcon(item.Info);
            SetName(item.Info);
            SetAmount(item.CurrentAmount);
        }

        public void Clear()
        {
            Item = default;

            if (_icon != null)
                _icon.sprite = null;

            if (_name != null)
                _name.text = string.Empty;

            if (_amount != null)
                _amount.text = string.Empty;
        }

        #endregion

        #region Private

        private void SetIcon(InventoryItemEntityInfo info)
        {
            if (_icon == null)
                return;

            info.TryGetVariableValue(_iconVariableId, out Sprite icon);
            _icon.sprite = icon;
        }

        private void SetName(InventoryItemEntityInfo info)
        {
            if (_name == null)
                return;

            if (!info.TryGetVariableValue(_nameVariableId, out string itemName))
                itemName = info.name;

            _name.text = itemName;
        }

        private void SetAmount(int amount)
        {
            if (_amount == null)
                return;

            var isShown =  amount > 0;
            _amount.text = isShown
                ? string.Format(_amountFormat, amount)
                : string.Empty;
        }

        #endregion
    }
}
