using System;

namespace D_Dev.InventorySystem.UI.Filters
{
    [Serializable]
    public abstract class InventorySlotFilter
    {
        #region Public

        public abstract bool IsValid(InventoryItemEntityInfo info);

        #endregion
    }
}
