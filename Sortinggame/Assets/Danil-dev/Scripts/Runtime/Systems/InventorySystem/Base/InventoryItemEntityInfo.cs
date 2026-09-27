using D_Dev.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InventorySystem
{
    [CreateAssetMenu(menuName = "D-Dev/Info/InventoryItemEntityInfo")]
    public class InventoryItemEntityInfo : EntityInfo
    {
        #region Fields

        [SerializeField] private string _name;
        [PreviewField(75, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;
        [TextArea(3, 5)]
        [SerializeField] private string _description;
        [SerializeField] private int _maxAmount = 1;

        #endregion

        #region Properties

        public string Name => _name;
        public Sprite Icon => _icon;
        public string Description => _description;
        public int MaxAmount => _maxAmount;

        #endregion
    }
}
