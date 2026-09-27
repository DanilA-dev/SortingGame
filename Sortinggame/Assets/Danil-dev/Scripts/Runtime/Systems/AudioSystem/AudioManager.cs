using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using D_Dev.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.AudioSystem
{
    public class AudioManager : BaseSingleton<AudioManager>
    {
        #region Classes

        private struct SoundRequest
        {
            public AudioClip Clip;
            public AudioConfig Config;
            public float Priority;
            public Vector3 WorldPos;
        }

        #endregion

        #region Fields

        [Title("Audio Mixer Groups")]
        [SerializeField] private AudioMixerGroupVolumeConfig[] _mixerGroupVolumeConfigs;

        [Title("Optimizations")]
        [SerializeField] private int maxSimultaneousSFX = 10;
        [SerializeField] private int poolSize = 14;
        [SerializeField] private int musicPoolSize = 2;
        [SerializeField] private float cullDistance = 30f;
        [SerializeField] private float throttleTime = 0.05f;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<bool> _onMuteAll;

        private List<SoundRequest> _requests = new(64);
        private Dictionary<AudioClip, float> _lastPlayed = new();
        private Dictionary<AudioConfig, AudioSource> _activeSources = new();

        private AudioSource[] _sfxPool;
        private AudioSource[] _musicPool;
        private Camera _mainCamera;
        private int _sfxPoolIdx;
        private int _musicPoolIdx;
        private bool _isMuted;

        public static event Action<bool> OnMuteStateChanged;

        #endregion

        #region Monobehaviour

        protected override void Awake()
        {
            base.Awake();
            InitPool();
        }

        private void Start() => _mainCamera = Camera.main;

        private void LateUpdate()
        {
            if (_requests.Count == 0)
                return;

            _requests.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            float now = Time.time;
            int played = 0;

            for (int i = 0; i < _requests.Count; i++)
            {
                var req = _requests[i];

                if (IsMusic(req.Config))
                {
                    PlayFromMusicPool(req.Config, req.WorldPos);
                    continue;
                }

                if (played >= maxSimultaneousSFX)
                    continue;

                if (_lastPlayed.TryGetValue(req.Clip, out float last) && now - last < throttleTime)
                    continue;

                _lastPlayed[req.Clip] = now;
                PlayFromSfxPool(req.Config, req.WorldPos);
                played++;
            }

            _requests.Clear();
        }

        #endregion

        #region Public

        public void SetVolume(MixerGroupType type, float value)
        {
            var config = _mixerGroupVolumeConfigs.FirstOrDefault(x => x.Type == type);
            config?.SetVolume(value);
        }

        public AudioMixerGroupVolumeConfig GetMixerGroup(MixerGroupType type)
        {
            return _mixerGroupVolumeConfigs.FirstOrDefault(x => x.Type == type);
        }

        public void SetMusicVolume(float value)
        {
            var config = _mixerGroupVolumeConfigs.FirstOrDefault(x => x.Type == MixerGroupType.Music);
            config?.SetVolume(value);
        }

        public void SetSFXVolume(float value)
        {
            var config = _mixerGroupVolumeConfigs.FirstOrDefault(x => x.Type == MixerGroupType.SFX);
            config?.SetVolume(value);
        }

        public void MuteAllAudioSources(bool isMute)
        {
            _isMuted = isMute;
            SetMuteAllSources(_isMuted);
            OnMuteStateChanged?.Invoke(_isMuted);
            _onMuteAll?.Invoke(_isMuted);
        }

        public bool IsMuted => _isMuted;

        public void RequestSound(AudioConfig config, Vector3 worldPos)
        {
            if (config == null)
                return;

            AudioClip clip = config.GetClip();
            if (clip == null)
                return;

            float priority = config.Priority;

            if (_mainCamera != null && config.SpatialBlend > 0f)
            {
                float dist = Vector3.Distance(worldPos, _mainCamera.transform.position);
                if (dist > cullDistance) return;
                priority /= 1f + dist * 0.1f;
            }

            _requests.Add(new SoundRequest { Clip = clip, Config = config, Priority = priority, WorldPos = worldPos });
        }

        public void StopSound(AudioConfig config)
        {
            if (_activeSources.TryGetValue(config, out var src))
            {
                src.Stop();
                _activeSources.Remove(config);
            }
        }

        public void StopSoundWithFade(AudioConfig config)
        {
            if (_activeSources.TryGetValue(config, out var src))
                StartCoroutine(FadeStop(src, config));
        }

        #endregion

        #region Private

        private bool IsMusic(AudioConfig config)
        {
            return config.GroupType == MixerGroupType.Music;
        }

        private void PlayFromMusicPool(AudioConfig config, Vector3 worldPos)
        {
            AudioSource src = null;
            for (int i = 0; i < _musicPool.Length; i++)
            {
                int j = (_musicPoolIdx + i) % _musicPool.Length;
                if (!_musicPool[j].isPlaying)
                {
                    src = _musicPool[j];
                    break;
                }
            }

            if (src == null)
            {
                if (_activeSources.TryGetValue(config, out var existing) && existing.isPlaying)
                    return;

                src = _musicPool[_musicPoolIdx % _musicPool.Length];
            }

            _musicPoolIdx = (_musicPoolIdx + 1) % _musicPool.Length;

            config.SetAudioSource(ref src);
            src.mute = _isMuted;
            src.transform.position = worldPos;
            _activeSources[config] = src;
            src.Play();
        }

        private void PlayFromSfxPool(AudioConfig config, Vector3 worldPos)
        {
            AudioSource src = null;
            for (int i = 0; i < _sfxPool.Length; i++)
            {
                int j = (_sfxPoolIdx + i) % _sfxPool.Length;
                if (!_sfxPool[j].isPlaying)
                {
                    src = _sfxPool[j];
                    break;
                }
            }

            src ??= _sfxPool[_sfxPoolIdx % _sfxPool.Length];
            _sfxPoolIdx = (_sfxPoolIdx + 1) % _sfxPool.Length;

            config.SetAudioSource(ref src);
            src.mute = _isMuted;
            src.transform.position = worldPos;
            _activeSources[config] = src;
            src.Play();
        }

        private void SetMuteAllSources(bool mute)
        {
            if (_sfxPool != null)
                foreach (var src in _sfxPool)
                    src.mute = mute;

            if (_musicPool != null)
                foreach (var src in _musicPool)
                    src.mute = mute;
        }

        private void InitPool()
        {
            _sfxPool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var go = new GameObject($"SFX_Pool_{i}");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.priority = 128;
                src.playOnAwake = false;
                _sfxPool[i] = src;
            }

            _musicPool = new AudioSource[musicPoolSize];
            for (int i = 0; i < musicPoolSize; i++)
            {
                var go = new GameObject($"Music_Pool_{i}");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.priority = 0;
                src.playOnAwake = false;
                _musicPool[i] = src;
            }
        }

        private IEnumerator FadeStop(AudioSource src, AudioConfig config)
        {
            float startVolume = src.volume;

            for (float i = 0; i < config.FadeTime; i += Time.deltaTime)
            {
                src.volume = startVolume * (1 - i / config.FadeTime);
                yield return null;
            }

            src.Stop();
            src.volume = startVolume;
            _activeSources.Remove(config);
        }

        #endregion
    }
}
