using GamePush;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.SDK
{
    public class GamePushSoundsHandler : MonoBehaviour
    {
        #region Fields

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<bool> _onMuteChanged;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<bool> _onMusicMuteChanged;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<bool> _onSfxMuteChanged;

        private bool _isWaitingForSdk;

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
        }

        private void OnDisable()
        {
            GP_Sounds.OnMute -= OnMute;
            GP_Sounds.OnUnmute -= OnUnmute;
            GP_Sounds.OnMuteMusic -= OnMuteMusic;
            GP_Sounds.OnUnmuteMusic -= OnUnmuteMusic;
            GP_Sounds.OnMuteSFX -= OnMuteSfx;
            GP_Sounds.OnUnmuteSFX -= OnUnmuteSfx;
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
            _onMusicMuteChanged?.Invoke(GP_Sounds.IsMuted(SoundType.Music));
            _onSfxMuteChanged?.Invoke(GP_Sounds.IsMuted(SoundType.SFX));
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
        private void OnMuteMusic() => _onMusicMuteChanged?.Invoke(true);
        private void OnUnmuteMusic() => _onMusicMuteChanged?.Invoke(false);
        private void OnMuteSfx() => _onSfxMuteChanged?.Invoke(true);
        private void OnUnmuteSfx() => _onSfxMuteChanged?.Invoke(false);

        #endregion
    }
}
