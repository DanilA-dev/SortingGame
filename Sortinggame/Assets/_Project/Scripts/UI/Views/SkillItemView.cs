using _Project.Scripts.Core.Skills;
using D_Dev.TimerSystem;
using D_Dev.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Views
{
    public class SkillItemView : MonoBehaviour
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _keyText;
        [Title("Using")]
        [SerializeField] private BaseTimerComponent _usingTimer;
        [SerializeField] private ImageFillUpdater _usingFill;
        [Title("Cooldown")]
        [SerializeField] private BaseTimerComponent _cooldownTimer;
        [SerializeField] private ImageFillUpdater _cooldownFill;

        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _onLock;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onUnlock;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onReady;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onNotAvailable;

        private SkillInfo _info;

        #endregion

        #region Properties

        public SkillInfo Info => _info;

        #endregion

        #region Monobehaviour

        private void OnDestroy() => Unsubscribe();

        #endregion

        #region Public

        public void Init(SkillInfo info, string key)
        {
            Unsubscribe();
            _info = info;
            Subscribe();

            _icon.sprite = _info.Icon;
            _keyText?.SetText(key);

            _usingTimer.TimeValue = _info.Duration;
            _cooldownTimer.TimeValue = _info.Cooldown;
            
            OnLockedStateChanged(info.IsLocked.Value);
            HideTimers();
        }

        #endregion

        #region Private

        private void Subscribe()
        {
            _info.OnUseStarted += OnUseStarted;
            _info.OnUseStopped += OnUseStopped;
            _info.IsLocked.OnValueChanged += OnLockedStateChanged;
            _info.OnReady += OnReady;
            _info.OnNotAvailable += OnNotAvailable;
        }

        private void Unsubscribe()
        {
            if (_info == null)
                return;

            _info.OnUseStarted -= OnUseStarted;
            _info.OnUseStopped -= OnUseStopped;
            _info.IsLocked.OnValueChanged -= OnLockedStateChanged;
            _info.OnReady -= OnReady;
            _info.OnNotAvailable -= OnNotAvailable;
        }

        private void HideTimers()
        {
            Hide(_usingTimer, _usingFill);
            Hide(_cooldownTimer, _cooldownFill);
        }

        private void Hide(BaseTimerComponent timer, ImageFillUpdater fill)
        {
            timer.StopTimer();
            fill.ResetFill();
            fill.gameObject.SetActive(false);
        }

        #endregion

        #region Listeners

        private void OnLockedStateChanged(bool isLocked)
        {
            if(isLocked)
                _onLock?.Invoke();
            else
                _onUnlock?.Invoke();
        }
        
        private void OnUseStarted()
        {
            _cooldownTimer.StopTimer();
            _usingTimer.StartTimer();
        }

        private void OnUseStopped()
        {
            _usingTimer.StopTimer();
            _cooldownTimer.StartTimer();
        }

        private void OnReady()
        {
            HideTimers();
            _onReady?.Invoke();
        }

        private void OnNotAvailable() => _onNotAvailable?.Invoke();

        #endregion
    }
}
