using D_Dev.MenuHandler;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class PauseMenu : BaseMenu
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private Button _sfxButton;
        [SerializeField] private Button _musicButton;
        [SerializeField] private Slider _lookSensSlider;
        [Space]
        [Title("Values")]
        [SerializeReference] private PolymorphicValue<bool> _isSfxMuted = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<bool> _isMusicMuted = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<float> _lookSens = new FloatConstantValue();

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onSfxOn;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onSfxOff;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onMusicOn;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onMusicOff;


        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _isSfxMuted.OnValueChanged += SfxMutedChanged;
            _isMusicMuted.OnValueChanged += MusicMutedChanged;
            _lookSensSlider.onValueChanged.AddListener(UpdateLookSensValue);

            _musicButton.onClick.AddListener(ToggleMusic);
            _sfxButton.onClick.AddListener(ToggleSfx);

            SfxMutedChanged(_isSfxMuted.Value);
            MusicMutedChanged(_isMusicMuted.Value);
        }
        private void OnDisable()
        {
            _isSfxMuted.OnValueChanged -= SfxMutedChanged;
            _isMusicMuted.OnValueChanged -= MusicMutedChanged;

            _lookSensSlider.onValueChanged.RemoveListener(UpdateLookSensValue);
            _musicButton.onClick.RemoveListener(ToggleMusic);
            _sfxButton.onClick.RemoveListener(ToggleSfx);
        }

        #endregion

        #region Public

        public void UpdateLookSensValue(float sliderValue)
        {
            _lookSens.Value = sliderValue;
        }

        #endregion

        #region Listeners

        private void MusicMutedChanged(bool isMuted)
        {
            if (isMuted)
                _onMusicOff?.Invoke();
            else
                _onMusicOn?.Invoke();
        }

        private void SfxMutedChanged(bool isMuted)
        {
            if (isMuted)
                _onSfxOff?.Invoke();
            else
                _onSfxOn?.Invoke();
        }

        private void ToggleSfx() => _isSfxMuted.Value = !_isSfxMuted.Value;
        private void ToggleMusic() => _isMusicMuted.Value = !_isMusicMuted.Value;

        #endregion
    }
}
