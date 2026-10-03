using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Core.Upgrades
{
    [CreateAssetMenu(menuName = "Game/Upgrades/Container")]
    public class UpgradesInfoContainer : ScriptableObject
    {
        #region Fields

        [SerializeField] private List<BaseUpgradeInfo> _upgrades;

        #endregion

        #region Properties

        public IReadOnlyList<BaseUpgradeInfo> Upgrades => _upgrades;

        #endregion
    }
}