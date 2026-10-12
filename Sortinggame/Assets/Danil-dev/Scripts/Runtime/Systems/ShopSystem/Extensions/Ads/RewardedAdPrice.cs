using System;
using System.Collections.Generic;
using D_Dev.AdsService;
using D_Dev.ShopSystem.Prices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem.Extensions.Ads
{
    [Serializable]
    public class RewardedAdPrice : BaseShopItemPrice
    {
        #region Fields

        [SerializeField, Min(0f), SuffixLabel("sec")] private float _cooldown;
        [Tooltip("Prices with the same id share one cooldown. Leave empty for a per-item cooldown.")]
        [SerializeField] private string _cooldownId;
        [SerializeField] private string _cooldownFormat = @"m\:ss";
        [PreviewField(50, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;

        private static readonly Dictionary<string, float> _sharedReadyTimes = new();

        [NonSerialized] private float _readyTime;

        #endregion

        #region Properties

        public override ShopPriceType Type => ShopPriceType.RewardedAd;
        public override bool IsDynamic => _cooldown > 0f;

        public bool IsOnCooldown => Time.unscaledTime < ReadyTime;

        private bool IsShared => !string.IsNullOrEmpty(_cooldownId);

        private float ReadyTime
        {
            get => IsShared ? _sharedReadyTimes.GetValueOrDefault(_cooldownId) : _readyTime;
            set
            {
                if (IsShared)
                    _sharedReadyTimes[_cooldownId] = value;
                else
                    _readyTime = value;
            }
        }

        #endregion

        #region Public

        public override bool CanPay(ShopItemInfo item) => AdsService.AdsService.Instance != null && !IsOnCooldown;

        public override void Pay(ShopItemInfo item, Action<bool> onComplete)
        {
            if (AdsService.AdsService.Instance == null)
            {
                Debug.LogWarning("[RewardedAdPrice] AdsService is not found");
                onComplete?.Invoke(false);
                return;
            }

            AdsService.AdsService.Instance.ShowRewarded(result =>
            {
                var isRewarded = result == AdResult.Rewarded;
                if (isRewarded)
                    ReadyTime = Time.unscaledTime + _cooldown;

                onComplete?.Invoke(isRewarded);
            });
        }

        public override string GetLabel(ShopItemInfo item)
        {
            if (!IsOnCooldown)
                return string.Empty;

            var seconds = Mathf.CeilToInt(ReadyTime - Time.unscaledTime);
            return TimeSpan.FromSeconds(seconds).ToString(_cooldownFormat);
        }

        public override Sprite GetIcon(ShopItemInfo item) => _icon;

        #endregion

        #region Private

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSharedCooldowns() => _sharedReadyTimes.Clear();

        #endregion
    }
}
