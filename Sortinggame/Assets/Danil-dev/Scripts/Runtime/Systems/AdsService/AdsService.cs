using System;
using System.Collections.Generic;
using System.Linq;
using D_Dev.Singleton;
using UnityEngine;

namespace D_Dev.AdsService
{
    #region Enums

    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded
    }

    public enum AdResult
    {
        Shown, 
        Rewarded,  
        Skipped,    
        Failed,     
        NotSupported
    }

    #endregion
    
    public class AdsService : BaseSingleton<AdsService>
    {
        
        #region Fields

        [SerializeField] private bool _loadAllTypesOnStart = true;
        [SerializeReference] private List<IAdsModule> _adsModules = new();

        private Dictionary<AdType, bool> _adTypes = new Dictionary<AdType, bool>
        {
            { AdType.Banner, false },
            { AdType.Interstitial, false },
            { AdType.Rewarded, false }
        };

        #endregion

        #region Monobehaviour

        private void Start() => InitializeAdsModules();
        private void OnDestroy() => DisposeAdsModules();

        #endregion

        #region Public
        public void SetAdTypeLoadState(AdType adType, bool isLoaded) => _adTypes[adType] = isLoaded;
        public void LoadInterstitialAd() => SetAdTypeLoadState(AdType.Interstitial, true);
        public void LoadRewardedAd() => SetAdTypeLoadState(AdType.Rewarded, true);
        public void LoadBannerAd() => SetAdTypeLoadState(AdType.Banner, true);
        
        public void LoadAllAdTypes()
        {
            foreach (var adType in _adTypes.Keys.ToList())
                SetAdTypeLoadState(adType, true);
        }

        public void ShowBanner(Action<AdResult> callback)
        {
            if (!_adTypes[AdType.Banner])
            {
                Debug.Log("[AdsService] Banner ad is not loaded");
                callback?.Invoke(AdResult.Failed);
                return;
            }
        
            foreach (var module in _adsModules)
            {
                if(module == null)
                    continue;
                
                if (module.IsInitialized)
                {
                    module.ShowBannerAd((result) => {
                        Debug.Log("[AdsService] " + module.GetType().Name + " banner ad result: " + result);
                        callback?.Invoke(result);
                    });
                    return;
                }
            }
        
            Debug.Log("[AdsService] No banner ad modules are initialized");
            callback?.Invoke(AdResult.Failed);
        }

        public void ShowInterstitial(Action<AdResult> callback)
        {
            if (!_adTypes[AdType.Interstitial])
            {
                Debug.Log("[AdsService] Interstitial ad is not loaded");
                callback?.Invoke(AdResult.Failed);
                return;
            }
 
            foreach (var module in _adsModules)
            {
                if (module == null) continue;
                if (module.IsInitialized)
                {
                    module.ShowInterstitialAd((result) => {
                        Debug.Log($"[AdsService] {module.GetType().Name} interstitial ad result: {result}");
                        callback?.Invoke(result);
                    });
                    return;
                }
            }
 
            Debug.Log("[AdsService] No interstitial ad modules are initialized");
            callback?.Invoke(AdResult.Failed);
        }

        public void ShowRewarded(Action<AdResult> callback)
        {
            if (!_adTypes[AdType.Rewarded])
            {
                Debug.Log("[AdsService] Rewarded ad is not loaded");
                callback?.Invoke(AdResult.Failed);
                return;
            }
 
            foreach (var module in _adsModules)
            {
                if (module == null) continue;
                if (module.IsInitialized)
                {
                    module.ShowRewardedAd((result) => {
                        Debug.Log($"[AdsService] {module.GetType().Name} rewarded ad result: {result}");
                        callback?.Invoke(result);
                    });
                    return;
                }
            }
 
            Debug.Log("[AdsService] No rewarded ad modules are initialized");
            callback?.Invoke(AdResult.Failed);
        }
        
        #endregion

        #region Private
        private async void InitializeAdsModules()
        {
            foreach (var module in _adsModules)
            {
                if(module == null)
                    continue;
                
                await module.Initialize();
                Debug.Log($"[AdsModule] Initialized module {module.GetType().Name}");
            }
            
            if(_loadAllTypesOnStart)
                LoadAllAdTypes();
        }

        private void DisposeAdsModules()
        {
            foreach (var module in _adsModules)
            {
                if(module == null)
                    continue;
                
                module.Dispose();
            }
        }
        #endregion
    }
}
