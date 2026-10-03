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

        [SerializeField] private Button _sfxButton;
        [SerializeField] private Button _musicButton;
        [Space]
        [SerializeReference] private PolymorphicValue<float> _isSfxVolume = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _isMusicVolume = new FloatConstantValue();

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
            _isSfxVolume.OnValueChanged += VolumeSfxChanged;
            _isMusicVolume.OnValueChanged += VolumeMusicChanged;
            
            _musicButton.onClick.AddListener(ToogleMusicVolume);
            _sfxButton.onClick.AddListener(ToogleSfxVolume);
        }


        private void OnDisable()
        {
            _isSfxVolume.OnValueChanged -= VolumeSfxChanged;
            _isMusicVolume.OnValueChanged -= VolumeMusicChanged;
            
            _musicButton.onClick.RemoveListener(ToogleMusicVolume);
            _sfxButton.onClick.RemoveListener(ToogleSfxVolume);
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

        private void ToogleSfxVolume()
        {
            if (_isSfxVolume.Value >= 0)
            {
                _isSfxVolume.Value = -80;
                VolumeSfxChanged(_isSfxVolume.Value);
            }
            else
            {
                _isSfxVolume.Value = 0;
                VolumeSfxChanged(-_isSfxVolume.Value);
            }
        }

        private void ToogleMusicVolume()
        {
            if (_isMusicVolume.Value >= 0)
            {
                _isMusicVolume.Value = -80;
                VolumeMusicChanged(_isMusicVolume.Value);
            }
            else
            {
                _isMusicVolume.Value = 0;
                VolumeMusicChanged(_isMusicVolume.Value);
            }
        }
        
        #endregion
    }
}