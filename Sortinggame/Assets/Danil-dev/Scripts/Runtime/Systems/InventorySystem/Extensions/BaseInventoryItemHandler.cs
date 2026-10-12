using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.InventorySystem.Extensions
{
    public abstract class BaseInventoryItemHandler : MonoBehaviour
    {
        #region Fields

        [Title("Inventory")]
        [SerializeReference] private PolymorphicValue<Inventory> _inventory = new InventoryScriptableVariableValue();
        [Title("Item")]
        [SerializeReference] private PolymorphicValue<EntityInfo> _item = new EntityInfoConstantValue();
        [SerializeReference] private PolymorphicValue<int> _amount = new IntConstantValue { Value = 1 };

        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<EntityInfo, int> OnSuccess;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<EntityInfo> OnFail;

        #endregion

        #region Protected

        protected void Handle() => Handle(_item?.Value);

        protected void Handle(GameObject itemObject)
        {
            if (itemObject == null || !itemObject.TryGetComponent(out EntityInfoRouter router))
            {
                OnFail?.Invoke(null);
                return;
            }

            Handle(router.Info);
        }

        protected void Handle(EntityInfo info)
        {
            var inventory = _inventory?.Value;
            var amount = _amount?.Value ?? 0;

            if (inventory != null && info is InventoryItemEntityInfo itemInfo && Apply(inventory, itemInfo, amount))
                OnSuccess?.Invoke(info, amount);
            else
                OnFail?.Invoke(info);
        }

        #endregion

        #region Abstract

        protected abstract bool Apply(Inventory inventory, InventoryItemEntityInfo itemInfo, int amount);

        #endregion
    }
}
