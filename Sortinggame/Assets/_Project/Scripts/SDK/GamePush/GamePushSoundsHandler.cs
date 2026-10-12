using D_Dev.PolymorphicValueSystem;
using GamePush;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.SDK
{
    public class GamePushSoundsHandler : MonoBehaviour
    {
        #region Fields

        [Title("Values")]
        [SerializeReference] private PolymorphicValue<bool> _isMusicMuted = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<bool> _isSfxMuted = new BoolConstantValue();

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<bool> _onMuteChanged;

        private bool _isWaitingForSdk;
        private bool _isApplyingFromSdk;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            GP_Sounds.OnMute += OnMute;
            GP_Sounds.OnUnmute += OnUnmute;
            GP_Sounds.OnMuteMusic += OnMuteMusic;
            GP_Sounds.OnUnmuteMusic += OnUnmuteMusic;
            GP_Sounds.OnMuteSFX += OnMuteSfx;
            GP_Sounds.OnUnmuteSFX += OnUnmuteSfx;

            _isMusicMuted.OnValueChanged += OnMusicMutedChanged;
            _isSfxMuted.OnValueChanged += OnSfxMutedChanged;
        }

        private void OnDisable()
        {
            GP_Sounds.OnMute -= OnMute;
            GP_Sounds.OnUnmute -= OnUnmute;
            GP_Sounds.OnMuteMusic -= OnMuteMusic;
            GP_Sounds.OnUnmuteMusic -= OnUnmuteMusic;
            GP_Sounds.OnMuteSFX -= OnMuteSfx;
            GP_Sounds.OnUnmuteSFX -= OnUnmuteSfx;

            _isMusicMuted.OnValueChanged -= OnMusicMutedChanged;
            _isSfxMuted.OnValueChanged -= OnSfxMutedChanged;
        }

        private void Start()
        {
            if (GP_Init.isReady)
            {
                ApplyCurrentState();
                return;
            }

            _isWaitingForSdk = true;
            GP_Init.OnReady += OnSdkReady;
        }

        private void OnDestroy()
        {
            if (_isWaitingForSdk)
                GP_Init.OnReady -= OnSdkReady;
        }

        #endregion

        #region Private

        private void ApplyCurrentState()
        {
            _onMuteChanged?.Invoke(GP_Sounds.IsMuted(SoundType.All));
            SetFromSdk(_isMusicMuted, GP_Sounds.IsMuted(SoundType.Music));
            SetFromSdk(_isSfxMuted, GP_Sounds.IsMuted(SoundType.SFX));
        }

        private void SetFromSdk(PolymorphicValue<bool> value, bool isMuted)
        {
            _isApplyingFromSdk = true;
            value.Value = isMuted;
            _isApplyingFromSdk = false;
        }

        private void PushToSdk(SoundType type, bool isMuted)
        {
            if (_isApplyingFromSdk || !GP_Init.isReady || GP_Sounds.IsMuted(type) == isMuted)
                return;

            if (isMuted)
                GP_Sounds.Mute(type);
            else
                GP_Sounds.Unmute(type);
        }

        #endregion

        #region Listeners

        private void OnSdkReady()
        {
            _isWaitingForSdk = false;
            GP_Init.OnReady -= OnSdkReady;
            ApplyCurrentState();
        }

        private void OnMute() => _onMuteChanged?.Invoke(true);
        private void OnUnmute() => _onMuteChanged?.Invoke(false);
        private void OnMuteMusic() => SetFromSdk(_isMusicMuted, true);
        private void OnUnmuteMusic() => SetFromSdk(_isMusicMuted, false);
        private void OnMuteSfx() => SetFromSdk(_isSfxMuted, true);
        private void OnUnmuteSfx() => SetFromSdk(_isSfxMuted, false);

        private void OnMusicMutedChanged(bool isMuted) => PushToSdk(SoundType.Music, isMuted);
        private void OnSfxMutedChanged(bool isMuted) => PushToSdk(SoundType.SFX, isMuted);

        #endregion
    }
}
