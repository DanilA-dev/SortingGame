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
        [SerializeReference] private PolymorphicValue<float> _sfxVolume = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _musicVolume = new FloatConstantValue();
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
            _sfxVolume.OnValueChanged += VolumeSfxChanged;
            _musicVolume.OnValueChanged += VolumeMusicChanged;
            _lookSensSlider.onValueChanged.AddListener(UpdateLookSensValue);
            
            _musicButton.onClick.AddListener(ToggleMusicVolume);
            _sfxButton.onClick.AddListener(ToggleSfxVolume);
        }
        private void OnDisable()
        {
            _sfxVolume.OnValueChanged -= VolumeSfxChanged;
            _musicVolume.OnValueChanged -= VolumeMusicChanged;
            
            _lookSensSlider.onValueChanged.RemoveListener(UpdateLookSensValue);
            _musicButton.onClick.RemoveListener(ToggleMusicVolume);
            _sfxButton.onClick.RemoveListener(ToggleSfxVolume);
        }

        #endregion

        #region Public

        public void UpdateLookSensValue(float sliderValue)
        {
            _lookSens.Value = sliderValue;
        }

        #endregion
        
        #region Listeners

        private void VolumeMusicChanged(float volume)
        {
            if(volume >= 0)
                _onMusicOn?.Invoke();
            else
                _onMusicOff?.Invoke();
        }

        private void VolumeSfxChanged(float volume)
        {
            if(volume >= 0)
                _onSfxOn?.Invoke();
            else
                _onSfxOff?.Invoke();
        }

        private void ToggleSfxVolume()
        {
            if (_sfxVolume.Value >= 0)
            {
                _sfxVolume.Value = -80;
                VolumeSfxChanged(_sfxVolume.Value);
            }
            else
            {
                _sfxVolume.Value = 0;
                VolumeSfxChanged(-_sfxVolume.Value);
            }
        }

        private void ToggleMusicVolume()
        {
            if (_musicVolume.Value >= 0)
            {
                _musicVolume.Value = -80;
                VolumeMusicChanged(_musicVolume.Value);
            }
            else
            {
                _musicVolume.Value = 0;
                VolumeMusicChanged(_musicVolume.Value);
            }
        }
        
        #endregion
    }
}