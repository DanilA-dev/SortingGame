#if D_DEV_YG2
using UnityEngine;
using YG;

namespace D_Dev.PluginYG2
{
    public class YG2Handler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _callGameReadyOnStart = true;

        private bool _isGameplayStarted;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if (_callGameReadyOnStart)
                CallGameReady();
        }

        #endregion

        #region Public

        public void CallGameReady() => YG2.GameReadyAPI();

        public void GameplayStart()
        {
            _isGameplayStarted = true;
            YG2.GameplayStart();
        }

        public void GameplayResume()
        {
            if (_isGameplayStarted && !YG2.nowAdsShow && YG2.isFocusWindowGame)
                YG2.GameplayStart();
        }

        public void GameplayStop() => YG2.GameplayStop();

        #endregion
    }
}
#endif
