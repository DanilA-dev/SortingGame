using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.AdsService
{
    [Serializable]
    public class DebugAdsModule : IAdsModule
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private bool _editorOnly = true;
        [SerializeField, SuffixLabel("sec")] private float _initializeDelay;
        [SerializeField, SuffixLabel("sec")] private float _showDelay = 1f;

        [Title("Results")]
        [SerializeField] private AdResult _bannerResult = AdResult.Shown;
        [SerializeField] private AdResult _interstitialResult = AdResult.Shown;
        [SerializeField] private AdResult _rewardedResult = AdResult.Rewarded;

        private CancellationTokenSource _cts;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region Public

        public async UniTask Initialize()
        {
            if (_editorOnly && !Application.isEditor)
                return;

            _cts = new CancellationTokenSource();

            if (await Wait(_initializeDelay))
                return;

            IsInitialized = true;
        }

        public void Dispose()
        {
            IsInitialized = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void ShowBannerAd(Action<AdResult> callback) => Show(_bannerResult, callback).Forget();
        public void ShowInterstitialAd(Action<AdResult> callback) => Show(_interstitialResult, callback).Forget();
        public void ShowRewardedAd(Action<AdResult> callback) => Show(_rewardedResult, callback).Forget();

        #endregion

        #region Private

        private async UniTaskVoid Show(AdResult result, Action<AdResult> callback)
        {
            if (await Wait(_showDelay))
                return;

            callback?.Invoke(result);
        }

        private async UniTask<bool> Wait(float seconds)
        {
            if (seconds <= 0f)
                return false;

            return await UniTask.Delay(TimeSpan.FromSeconds(seconds), DelayType.Realtime, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
        }

        #endregion
    }
}
