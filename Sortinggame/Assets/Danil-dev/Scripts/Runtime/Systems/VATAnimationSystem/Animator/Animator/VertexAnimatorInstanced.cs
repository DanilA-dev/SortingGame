using System.Collections.Generic;
using D_Dev.PolymorphicValueSystem;
using D_Dev.UpdateManagerSystem;
using D_Dev.VATAnimationSystem.GroupInstanceRenderer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.VATAnimationSystem
{
    public class VertexAnimatorInstanced : MonoBehaviour, ITickable, IVertexAnimatorInfo
    {
        #region Classes

        [System.Serializable]
        public class VATAnimationState
        {
            [SerializeReference] public PolymorphicValue<string> StateName = new StringConstantValue();
            [SerializeReference] public PolymorphicValue<string> AnimationName = new StringConstantValue();
            [field: SerializeField] public bool IsLooping { get; set; } = true;
            [field: SerializeField] public float Speed { get; set; } = 1f;
            [field: SerializeField] public bool PlayOnStart { get; set; }
        }

        #endregion

        #region Fields

        [Title("Disabled Components")]
        [SerializeField] private Renderer _renderer;
        [Title("Base")]
        [SerializeField] private VertexAnimationData _data;
        [SerializeField] private Material _material;
        [Title("Animation States")]
        [SerializeField] private List<VATAnimationState> _states = new();

        private GroupInstancesRenderer.VATInstance _instance;
        private VertexAnimationClipInfo _currentClip;
        private float _time;
        private bool _playing;
        private bool _isLooping;
        private float _speed = 1f;

        #endregion

        #region Properties

        public VertexAnimationClipInfo CurrentClip => _currentClip;
        public bool IsPlaying => _playing;
        public float NormalizedTime => _currentClip == null ? 0f : _time / _currentClip.Duration;

        #endregion

        #region Monobehaviour

        private void Reset()
        {
            TryGetComponent(out _renderer);
        }

        private void Awake()
        {
            if (GroupInstancesRenderer.Instance == null)
                return;

            _renderer.enabled = false;
            
            _instance = GroupInstancesRenderer.Instance.AddUnit(
                _data, _material, transform.localToWorldMatrix);
        }

        private void Start()
        {
            foreach (var state in _states)
            {
                if (state.PlayOnStart)
                    PlayFromState(state);
            }
        }

        private void OnEnable()
        {
            if (_instance == null || GroupInstancesRenderer.Instance == null)
                return;

            UpdateManager.Add(this);
            GroupInstancesRenderer.Instance.AddExistingUnit(_data, _instance);
        }

        private void OnDisable()
        {
            UpdateManager.Remove(this);

            if (GroupInstancesRenderer.Instance != null && _instance != null)
                GroupInstancesRenderer.Instance.RemoveUnit(_data, _instance);
        }

        private void OnDestroy()
        {
            UpdateManager.Remove(this);

            if (GroupInstancesRenderer.Instance != null && _instance != null)
                GroupInstancesRenderer.Instance.RemoveUnit(_data, _instance);
        }

        #endregion

        #region ITickable

        public void Tick()
        {
            if (this == null)
                return;

            if (_instance != null)
                _instance.Matrix = transform.localToWorldMatrix;

            if (!_playing || _currentClip == null)
                return;

            _time += Time.deltaTime * _speed;

            if (_time >= _currentClip.Duration)
            {
                if (_isLooping)
                    _time %= _currentClip.Duration;
                else
                {
                    _time = _currentClip.Duration;
                    _playing = false;
                }
            }

            if (_instance != null)
            {
                _instance.Clip = _currentClip;
                _instance.Time = _time;
                _instance.Playing = _playing;
            }
        }

        #endregion

        #region Public

        public void Play(string animationState)
        {
            foreach (var state in _states)
            {
                if (state.StateName.Value == animationState)
                    PlayFromState(state);
            }
        }

        public void Play(PolymorphicValue<string> animationState)
        {
            foreach (var state in _states)
            {
                if (state.StateName.Value == animationState.Value)
                    PlayFromState(state);
            }
        }

        public void Stop() => _playing = false;

        public void SetNormalizedTime(float t)
        {
            if (_currentClip == null) return;
            _time = Mathf.Clamp01(t) * _currentClip.Duration;
            if (_instance != null) _instance.Time = _time;
        }

        #endregion

        #region Private

        private void PlayFromState(VATAnimationState state)
        {
            if (_data == null) return;
            if (!_data.TryGetClip(state.AnimationName.Value, out var info)) return;

            _currentClip = info;
            _time = 0f;
            _playing = true;
            _isLooping = state.IsLooping;
            _speed = Mathf.Max(0f, state.Speed);

            if (_instance != null)
            {
                _instance.Clip = _currentClip;
                _instance.Time = 0f;
                _instance.Playing = true;
            }
        }

        #endregion
    }
}