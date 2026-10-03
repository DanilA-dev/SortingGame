using UnityEngine;

namespace _Project.Scripts.Core.Upgrades
{
    public class UpgradesLoader : MonoBehaviour
    {
        #region Fields

        [SerializeField] private UpgradesInfoContainer _upgradesInfoContainer;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if(_upgradesInfoContainer != null)
                foreach (var upgradeInfo in _upgradesInfoContainer.Upgrades)
                    upgradeInfo.Apply();
        }

        #endregion
    }
}