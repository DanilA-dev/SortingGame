using System;
using Cysharp.Threading.Tasks;
using D_Dev.AdsService;
using UnityEngine;
using YG;

namespace _Project.Scripts.SDK
{
    [Serializable]
    public class YG2AdsModule : IAdsModule
    {
        #region Fields

        [SerializeField] private string _rewardedId = "reward";

        private Action<AdResult> _rewardedCallback;
        private Action<AdResult> _interstitialCallback;
        private bool _rewardReceived;
        private bool _interstitialRequested;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region Public

        public async UniTask Initialize()
        {
            await UniTask.WaitUntil(() => YG2.isSDKEnabled);

            YG2.onRewardAdv += OnReward;
            YG2.onCloseRewardedAdv += OnRewardedClose;
            YG2.onErrorRewardedAdv += OnRewardedError;
            YG2.onCloseInterAdvWasShow += OnInterstitialClose;
            YG2.onErrorInterAdv += OnInterstitialError;

            IsInitialized = true;
        }

        public void Dispose()
        {
            YG2.onRewardAdv -= OnReward;
            YG2.onCloseRewardedAdv -= OnRewardedClose;
            YG2.onErrorRewardedAdv -= OnRewardedError;
            YG2.onCloseInterAdvWasShow -= OnInterstitialClose;
            YG2.onErrorInterAdv -= OnInterstitialError;
            YG2.onAdvNotification -= OnInterstitialRequested;

            IsInitialized = false;
            _rewardedCallback = null;
            _interstitialCallback = null;
        }

        public void ShowBannerAd(Action<AdResult> callback) => callback?.Invoke(AdResult.NotSupported);

        public void ShowInterstitialAd(Action<AdResult> callback)
        {
            if (YG2.nowAdsShow || _interstitialCallback != null)
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            _interstitialCallback = callback;
            _interstitialRequested = false;

            YG2.onAdvNotification += OnInterstitialRequested;
            YG2.InterstitialAdvShow();
            YG2.onAdvNotification -= OnInterstitialRequested;

            if (!_interstitialRequested)
                Complete(ref _interstitialCallback, AdResult.Skipped);
        }

        public void ShowRewardedAd(Action<AdResult> callback)
        {
            if (YG2.nowAdsShow || _rewardedCallback != null)
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            _rewardReceived = false;
            _rewardedCallback = callback;
            YG2.RewardedAdvShow(_rewardedId);
        }

        #endregion

        #region Private

        private void OnReward(string id)
        {
            if (id == _rewardedId)
                _rewardReceived = true;
        }

        private void OnRewardedClose() => CompleteRewardedNextFrame(_rewardReceived ? AdResult.Rewarded : AdResult.Skipped).Forget();
        private void OnRewardedError() => CompleteRewardedNextFrame(AdResult.Failed).Forget();

        private void OnInterstitialRequested() => _interstitialRequested = true;
        private void OnInterstitialClose(bool wasShown) => CompleteInterstitialNextFrame(wasShown ? AdResult.Shown : AdResult.Skipped).Forget();
        private void OnInterstitialError() => CompleteInterstitialNextFrame(AdResult.Failed).Forget();

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
