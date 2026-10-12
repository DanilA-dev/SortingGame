using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.ShopSystem
{
    [CreateAssetMenu(menuName = "D-Dev/Shop/Items Container")]
    public class ShopItemsContainer : ScriptableObject
    {
        #region Fields

        [SerializeField] private List<ShopItemInfo> _items = new();

        #endregion

        #region Properties

        public IReadOnlyList<ShopItemInfo> Items => _items;

        #endregion
    }
}
