using System;
using Cysharp.Threading.Tasks;
using D_Dev.AdsService;
using Sirenix.OdinInspector;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace _Project.Scripts.SDK
{
    [Serializable]
    public class YandexMobileAdsModule : IAdsModule
    {
        #region Fields

        [Title("Ad Units")]
        [SerializeField] private string _rewardedAdUnitId = "demo-rewarded-yandex";
        [SerializeField] private string _interstitialAdUnitId = "demo-interstitial-yandex";
        [SerializeField] private string _bannerAdUnitId = "demo-banner-yandex";

        [Title("Settings")]
        [SerializeField] private bool _ageRestricted;
        [SerializeField, SuffixLabel("sec"), Min(0f)] private float _reloadDelay = 10f;

        private RewardedAdLoader _rewardedLoader;
        private InterstitialAdLoader _interstitialLoader;
        private RewardedAd _rewardedAd;
        private Interstitial _interstitial;
        private Banner _banner;

        private Action<AdResult> _rewardedCallback;
        private Action<AdResult> _interstitialCallback;
        private Action<AdResult> _bannerCallback;
        private bool _isRewardedLoading;
        private bool _isInterstitialLoading;
        private bool _rewardReceived;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region Public

        public UniTask Initialize()
        {
            if (Application.isEditor)
                return UniTask.CompletedTask;

            YandexAds.SetAgeRestricted(_ageRestricted);

            _rewardedLoader = new RewardedAdLoader();
            _interstitialLoader = new InterstitialAdLoader();

            LoadRewarded();
            LoadInterstitial();

            IsInitialized = true;
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            IsInitialized = false;

            _rewardedLoader?.CancelLoading();
            _interstitialLoader?.CancelLoading();

            DestroyRewarded();
            DestroyInterstitial();
            DestroyBanner();

            _rewardedCallback = null;
            _interstitialCallback = null;
            _bannerCallback = null;
        }

        public void ShowRewardedAd(Action<AdResult> callback)
        {
            if (_rewardedCallback != null || _interstitialCallback != null)
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            if (_rewardedAd == null)
            {
                LoadRewarded();
                callback?.Invoke(AdResult.Failed);
                return;
            }

            _rewardReceived = false;
            _rewardedCallback = callback;
            _rewardedAd.Show();
        }

        public void ShowInterstitialAd(Action<AdResult> callback)
        {
            if (_rewardedCallback != null || _interstitialCallback != null)
            {
                callback?.Invoke(AdResult.Failed);
                return;
            }

            if (_interstitial == null)
            {
                LoadInterstitial();
                callback?.Invoke(AdResult.Skipped);
                return;
            }

            _interstitialCallback = callback;
            _interstitial.Show();
        }

        public void ShowBannerAd(Action<AdResult> callback)
        {
            if (string.IsNullOrEmpty(_bannerAdUnitId))
            {
                callback?.Invoke(AdResult.NotSupported);
                return;
            }

            DestroyBanner();
            _bannerCallback = callback;

            var screenWidthDp = ScreenUtils.ConvertPixelsToDp((int)Screen.safeArea.width);
            _banner = new Banner(BannerAdSize.Sticky(screenWidthDp), AdPosition.BottomCenter);
            _banner.OnAdLoaded += OnBannerLoaded;
            _banner.OnAdFailedToLoad += OnBannerFailedToLoad;
            _banner.LoadAd(new AdRequest(_bannerAdUnitId));
        }

        #endregion

        #region Private

        private void LoadRewarded()
        {
            if (_rewardedLoader == null || _isRewardedLoading || _rewardedAd != null)
                return;

            _isRewardedLoading = true;
            _rewardedLoader.LoadAd(new AdRequest(_rewardedAdUnitId), OnRewardedLoaded, OnRewardedFailedToLoad);
        }

        private void LoadInterstitial()
        {
            if (_interstitialLoader == null || _isInterstitialLoading || _interstitial != null)
                return;

            _isInterstitialLoading = true;
            _interstitialLoader.LoadAd(new AdRequest(_interstitialAdUnitId), OnInterstitialLoaded, OnInterstitialFailedToLoad);
        }

        private async UniTaskVoid ReloadDelayed(Action load)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_reloadDelay), DelayType.UnscaledDeltaTime);

            if (IsInitialized)
                load();
        }

        private void DestroyRewarded()
        {
            if (_rewardedAd == null)
                return;

            _rewardedAd.OnAdDismissed -= OnRewardedDismissed;
            _rewardedAd.OnAdFailedToShow -= OnRewardedFailedToShow;
            _rewardedAd.OnRewarded -= OnRewardedRewarded;
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        private void DestroyInterstitial()
        {
            if (_interstitial == null)
                return;

            _interstitial.OnAdDismissed -= OnInterstitialDismissed;
            _interstitial.OnAdFailedToShow -= OnInterstitialFailedToShow;
            _interstitial.Destroy();
            _interstitial = null;
        }

        private void DestroyBanner()
        {
            if (_banner == null)
                return;

            _banner.OnAdLoaded -= OnBannerLoaded;
            _banner.OnAdFailedToLoad -= OnBannerFailedToLoad;
            _banner.Destroy();
            _banner = null;
        }

        private async UniTaskVoid CompleteRewardedNextFrame(AdResult? forcedResult)
        {
            await UniTask.Yield();
            var result = forcedResult ?? (_rewardReceived ? AdResult.Rewarded : AdResult.Skipped);
            DestroyRewarded();
            Complete(ref _rewardedCallback, result);
            LoadRewarded();
        }

        private static void Complete(ref Action<AdResult> callback, AdResult result)
        {
            var pending = callback;
            callback = null;
            pending?.Invoke(result);
        }

        #endregion

        #region Listeners

        private void OnRewardedLoaded(RewardedAd ad)
        {
            _isRewardedLoading = false;
            _rewardedAd = ad;
            _rewardedAd.OnAdDismissed += OnRewardedDismissed;
            _rewardedAd.OnAdFailedToShow += OnRewardedFailedToShow;
            _rewardedAd.OnRewarded += OnRewardedRewarded;
        }

        private void OnRewardedFailedToLoad(AdFailedToLoadEventArgs args)
        {
            _isRewardedLoading = false;
            Debug.LogWarning($"[YandexMobileAdsModule] Rewarded failed to load: {args.Message}");
            ReloadDelayed(LoadRewarded).Forget();
        }

        private void OnRewardedRewarded(object sender, Reward reward) => _rewardReceived = true;
        private void OnRewardedDismissed(object sender, EventArgs args) => CompleteRewardedNextFrame(null).Forget();

        private void OnRewardedFailedToShow(object sender, AdFailureEventArgs args)
        {
            Debug.LogWarning($"[YandexMobileAdsModule] Rewarded failed to show: {args.Message}");
            CompleteRewardedNextFrame(AdResult.Failed).Forget();
        }

        private void OnInterstitialLoaded(Interstitial ad)
        {
            _isInterstitialLoading = false;
            _interstitial = ad;
            _interstitial.OnAdDismissed += OnInterstitialDismissed;
            _interstitial.OnAdFailedToShow += OnInterstitialFailedToShow;
        }

        private void OnInterstitialFailedToLoad(AdFailedToLoadEventArgs args)
        {
            _isInterstitialLoading = false;
            Debug.LogWarning($"[YandexMobileAdsModule] Interstitial failed to load: {args.Message}");
            ReloadDelayed(LoadInterstitial).Forget();
        }

        private void OnInterstitialDismissed(object sender, EventArgs args)
        {
            DestroyInterstitial();
            Complete(ref _interstitialCallback, AdResult.Shown);
            LoadInterstitial();
        }

        private void OnInterstitialFailedToShow(object sender, AdFailureEventArgs args)
        {
            Debug.LogWarning($"[YandexMobileAdsModule] Interstitial failed to show: {args.Message}");
            DestroyInterstitial();
            Complete(ref _interstitialCallback, AdResult.Failed);
            LoadInterstitial();
        }

        private void OnBannerLoaded(object sender, EventArgs args)
        {
            _banner?.Show();
            Complete(ref _bannerCallback, AdResult.Shown);
        }

        private void OnBannerFailedToLoad(object sender, AdFailureEventArgs args)
        {
            Debug.LogWarning($"[YandexMobileAdsModule] Banner failed to load: {args.Message}");
            DestroyBanner();
            Complete(ref _bannerCallback, AdResult.Failed);
        }

        #endregion
    }
}
