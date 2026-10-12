using D_Dev.Entity;
using UnityEngine;

namespace D_Dev.InventorySystem
{
    [CreateAssetMenu(menuName = "D-Dev/Info/InventoryItemEntityInfo")]
    public class InventoryItemEntityInfo : EntityInfo
    {
        #region Fields

        [SerializeField] private int _maxAmount;

        #endregion

        #region Properties

        public int MaxAmount => _maxAmount;
        public bool IsStackLimited => _maxAmount > 0;

        #endregion
    }
}
