using D_Dev.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem.Extensions
{
    [CreateAssetMenu(menuName = "D-Dev/Shop/Entity Item")]
    public class EntityShopItemInfo : OneTimeShopItemInfo
    {
        #region Fields

        [Title("Entity")]
        [SerializeField, Required] private EntityInfo _entity;

        #endregion

        #region Properties

        public EntityInfo Entity => _entity;

        #endregion
    }
}
