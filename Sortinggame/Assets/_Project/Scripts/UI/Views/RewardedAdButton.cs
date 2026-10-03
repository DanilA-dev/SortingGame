using System;
using D_Dev.AdsService;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Views
{
    public class RewardedAdButton : MonoBehaviour
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _cooldownText;
        [Title("Settings")]
        [SerializeField, SuffixLabel("sec")] private float _cooldown = 90f;

        [SerializeField] private string _format = @"m\:ss";

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onCooldownStart;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onCooldownEnd;

        private float _readyTime;
        private bool _isWaitingForAd;
        private bool _wasOnCooldown;

        public event Action OnRewarded;

        #endregion

        #region Properties

        public bool IsOnCooldown => Time.unscaledTime < _readyTime;

        #endregion

        #region Monobehaviour

        private void Awake() => _button.onClick.AddListener(OnClick);

        private void OnDestroy() => _button.onClick.RemoveListener(OnClick);

        private void OnEnable() => RefreshState();

        private void Update() => RefreshState();

        #endregion

        #region Listeners

        private void OnClick()
        {
            if (IsOnCooldown || _isWaitingForAd)
                return;

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
                _readyTime = Time.unscaledTime + _cooldown;
                OnRewarded?.Invoke();
            }

            RefreshState();
        }

        #endregion

        #region Private

        private void RefreshState()
        {
            var isOnCooldown = IsOnCooldown;
            _button.interactable = !isOnCooldown && !_isWaitingForAd;

            if (isOnCooldown)
                _cooldownText?.SetText(TimeSpan.FromSeconds(Mathf.CeilToInt(_readyTime - Time.unscaledTime)).ToString(_format));

            if (isOnCooldown == _wasOnCooldown)
                return;

            _wasOnCooldown = isOnCooldown;
            if (isOnCooldown)
                _onCooldownStart?.Invoke();
            else
                _onCooldownEnd?.Invoke();
        }

        #endregion
    }
}
