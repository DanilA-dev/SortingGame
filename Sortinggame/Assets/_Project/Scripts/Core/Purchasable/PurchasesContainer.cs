using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Core.Purchasable
{
    [CreateAssetMenu(menuName = "Game/Purchases/Container")]
    public class PurchasesContainer : ScriptableObject
    {
        #region Fields

        [SerializeField] private List<BasePurchasableInfo> _purchases;

        #endregion

        #region Properties

        public IReadOnlyList<BasePurchasableInfo> Purchases => _purchases;

        #endregion
    }
}
