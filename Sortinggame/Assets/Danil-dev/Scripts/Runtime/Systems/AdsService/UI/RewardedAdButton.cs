using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace D_Dev.AdsService.UI
{
    public class RewardedAdButton : MonoBehaviour
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _cooldownText;
        [Title("Settings")]
        [SerializeField, SuffixLabel("sec"), Min(0f)] private float _cooldown = 90f;
        [Tooltip("Buttons with the same id share one cooldown. Leave empty for a per-button cooldown.")]
        [SerializeField] private string _cooldownId;
        [SerializeField] private string _format = @"m\:ss";

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onCooldownStart;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onCooldownEnd;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onRewarded;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onFailed;

        private static readonly Dictionary<string, float> _sharedReadyTimes = new();

        private float _readyTime;
        private bool _isWaitingForAd;
        private bool _wasOnCooldown;

        public event Action OnRewarded;
        public event Action OnFailed;

        #endregion

        #region Properties

        public bool IsOnCooldown => Time.unscaledTime < ReadyTime;
        public bool IsWaitingForAd => _isWaitingForAd;

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

        #region Monobehaviour

        private void Awake() => _button.onClick.AddListener(OnClick);

        private void OnDestroy() => _button.onClick.RemoveListener(OnClick);

        private void OnEnable()
        {
            _wasOnCooldown = !IsOnCooldown;
            RefreshState();
        }

        private void Update() => RefreshState();

        #endregion

        #region Listeners

        private void OnClick()
        {
            if (IsOnCooldown || _isWaitingForAd)
                return;

            if (AdsService.Instance == null)
            {
                Debug.LogWarning("[RewardedAdButton] AdsService is not found");
                Fail();
                return;
            }

            _isWaitingForAd = true;
            RefreshState();
            AdsService.Instance.ShowRewarded(OnAdResult);
        }

        private void OnAdResult(AdResult result)
        {
            if (this == null)
                return;

            _isWaitingForAd = false;

            if (result == AdResult.Rewarded)
            {
                ReadyTime = Time.unscaledTime + _cooldown;
                OnRewarded?.Invoke();
                _onRewarded?.Invoke();
            }
            else
            {
                Fail();
            }

            RefreshState();
        }

        #endregion

        #region Private

        private void Fail()
        {
            OnFailed?.Invoke();
            _onFailed?.Invoke();
        }

        private void RefreshState()
        {
            var isOnCooldown = IsOnCooldown;
            _button.interactable = !isOnCooldown && !_isWaitingForAd;

            if (isOnCooldown)
                _cooldownText?.SetText(TimeSpan.FromSeconds(Mathf.CeilToInt(ReadyTime - Time.unscaledTime)).ToString(_format));

            if (isOnCooldown == _wasOnCooldown)
                return;

            _wasOnCooldown = isOnCooldown;
            if (isOnCooldown)
                _onCooldownStart?.Invoke();
            else
                _onCooldownEnd?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSharedCooldowns() => _sharedReadyTimes.Clear();

        #endregion
    }
}
