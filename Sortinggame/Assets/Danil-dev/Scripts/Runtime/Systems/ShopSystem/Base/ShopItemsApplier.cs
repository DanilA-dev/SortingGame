using UnityEngine;

namespace D_Dev.ShopSystem
{
    public class ShopItemsApplier : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ShopItemsContainer[] _containers;

        #endregion

        #region Public

        public void ApplyAll()
        {
            foreach (var container in _containers)
            {
                if (container == null)
                    continue;

                foreach (var item in container.Items)
                {
                    if (item != null && !item.IsConsumable)
                        item.Apply();
                }
            }
        }

        #endregion
    }
}
