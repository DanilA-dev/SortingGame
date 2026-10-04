using UnityEngine;

namespace _Project.Scripts.Core.Purchasable
{
    public class PurchasesLoader : MonoBehaviour
    {
        #region Fields

        [SerializeField] private PurchasesContainer[] _containers;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            foreach (var container in _containers)
            {
                if (container == null)
                    continue;

                foreach (var purchase in container.Purchases)
                    purchase.Apply();
            }
        }

        #endregion
    }
}
