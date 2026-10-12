using System;
using Cysharp.Threading.Tasks;
using D_Dev.AdsService;
using GamePush;
using UnityEngine;

namespace _Project.Scripts.SDK
{
    [Serializable]
    public class GamePushAdsModule : IAdsModule
    {
        #region Fields

        [SerializeField] private bool _showFullscreenCountdown;

        private Action<AdResult> _rewardedCallback;
        private Action<AdResult> _interstitialCallback;
        private bool _rewardReceived;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region Public

        public async UniTask Initialize()
        {
            await UniTask.WaitUntil(() => GP_Init.isReady);
            IsInitialized = true;
        }

        public void Dispose()
        {
            IsInitialized = false;
            _rewardedCallback = null;
            _interstitialCallback = null;
        }

        public void ShowBannerAd(Action<AdResult> callback)
        {
            if (!GP_Ads.IsStickyAvailable())
            {
                callback?.Invoke(AdResult.NotSupported);
                return;
            }

            GP_Ads.ShowSticky();
            callback?.Invoke(AdResult.Shown);
        }

        public void ShowInterstitialAd(Action<AdResult> callback)
        {
            if (IsAnyAdPlaying() || _interstitialCallback != null)
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            if (!GP_Ads.IsFullscreenAvailable())
            {
                callback?.Invoke(AdResult.Skipped);
                return;
            }

            _interstitialCallback = callback;
            GP_Ads.ShowFullscreen(_showFullscreenCountdown, null, OnFullscreenClose);
        }

        public void ShowRewardedAd(Action<AdResult> callback)
        {
            if (IsAnyAdPlaying() || _rewardedCallback != null)
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            if (!GP_Ads.IsRewardedAvailable())
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            _rewardReceived = false;
            _rewardedCallback = callback;
            GP_Ads.ShowRewarded(onRewardedReward: OnRewardedReward, onRewardedClose: OnRewardedClose);
        }

        #endregion

        #region Private

        private static bool IsAnyAdPlaying()
            => GP_Ads.IsFullscreenPlaying() || GP_Ads.IsRewardPlaying() || GP_Ads.IsPreloaderPlaying();

        private void OnRewardedReward(string tag) => _rewardReceived = true;

        private void OnRewardedClose(bool success)
        {
            var result = _rewardReceived ? AdResult.Rewarded : success ? AdResult.Skipped : AdResult.Failed;
            CompleteRewardedNextFrame(result).Forget();
        }

        private void OnFullscreenClose(bool success)
            => CompleteInterstitialNextFrame(success ? AdResult.Shown : AdResult.Failed).Forget();

        private async UniTaskVoid CompleteRewardedNextFrame(AdResult result)
        {
            await UniTask.Yield();
            Complete(ref _rewardedCallback, result);
        }

        private async UniTaskVoid CompleteInterstitialNextFrame(AdResult result)
        {
            await UniTask.Yield();
            Complete(ref _interstitialCallback, result);
        }

        private static void Complete(ref Action<AdResult> callback, AdResult result)
        {
            Action<AdResult> pending = callback;
            callback = null;
            pending?.Invoke(result);
        }

        #endregion
    }
}
