using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace D_Dev.AudioSystem
{
    [System.Serializable]
    public class AudioMixerGroupVolumeConfig
    {
        #region Fields

        [SerializeField] private float _volume;
        [SerializeField] private MixerGroupType _type;
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private string _volumeProperty = "Volume";

        public UnityEvent<float> OnVolumeChanged;

        #endregion

        #region Properties

        public float Volume => _volume;
        public MixerGroupType Type => _type;
        public AudioMixerGroup MixerGroup => _mixerGroup;
        public string VolumeProperty => _volumeProperty;
        public bool IsMuted { get; private set; }

        #endregion

        #region Public

        public void SetVolume(float value)
        {
            _volume = value;
            OnVolumeChanged?.Invoke(_volume);
            MixerGroup.audioMixer.SetFloat(VolumeProperty, _volume);
            IsMuted = Volume <= -80f;
        }

        #endregion
    }
}