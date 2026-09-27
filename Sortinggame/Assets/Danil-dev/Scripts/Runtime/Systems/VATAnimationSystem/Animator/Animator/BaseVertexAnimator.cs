using System.Collections.Generic;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.VATAnimationSystem
{
    public abstract class BaseVertexAnimator : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class VATAnimationState
        {
            
            [SerializeReference] public PolymorphicValue<string> StateName = new StringConstantValue();
            [SerializeReference] public PolymorphicValue<string> AnimationName = new StringConstantValue();
            [field: SerializeField] public bool IsLooping { get; set; } = true;
            [field: SerializeField] public float Speed { get; set; } = 1;
            [field: SerializeField] public bool PlayOnStart { get; set; }
        }

        #endregion

        #region Fields

        [Title("Base")]
        [SerializeField] protected VertexAnimationData _data;
        [SerializeField] protected Renderer _renderer;
        [Title("Animation States")]
        [SerializeField] protected List<VATAnimationState> _states = new();

        protected MaterialPropertyBlock _props;
        protected VertexAnimationClipInfo _currentClip;
        protected float _time;
        protected bool _playing;
        protected bool _isLooping;
        protected float _speed = 1f;

        #endregion

        #region Properties

        public VertexAnimationClipInfo CurrentClip => _currentClip;
        public bool IsPlaying => _playing;
        public float NormalizedTime => _currentClip == null ? 0f : _time / _currentClip.Duration;
        public List<VATAnimationState> States => _states;

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            _props = new MaterialPropertyBlock();

            if (_data != null)
                _renderer.GetPropertyBlock(_props);
        }

        protected virtual void Start()
        {
            foreach (var state in _states)
            {
                if (state.PlayOnStart)
                    PlayFromState(state);
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
            ApplyFrame();
        }

        public void UpdateAnimation()
        {
            if (!_playing || _currentClip == null)
                return;

            _time += Time.deltaTime * _speed;

            if (_time >= _currentClip.Duration)
            {
                if (_isLooping)
                {
                    _time %= _currentClip.Duration;
                }
                else
                {
                    _time = _currentClip.Duration;
                    _playing = false;
                }
            }

            ApplyFrame();
        }

        #endregion

        #region Private

        private void PlayFromState(VATAnimationState state)
        {
            if (_data == null)
                return;
            
            if (!_data.TryGetClip(state.AnimationName.Value, out var info))
                return;

            _currentClip = info;
            _time = 0f;
            _playing = true;
            _isLooping = state.IsLooping;
            _speed = Mathf.Max(0f, state.Speed);

            if (_data.TotalFrames > 0)
            {
                _props.SetFloat("_TotalFrames", _data.TotalFrames);
                _renderer.SetPropertyBlock(_props);
            }
        }

        private void ApplyFrame()
        {
            if (_currentClip == null || _renderer == null)
                return;

            float normalizedInClip = _currentClip.Duration > 0f ? _time / _currentClip.Duration : 0f;
            float frame = _currentClip.StartFrame + normalizedInClip * (_currentClip.FrameCount - 1);

            _renderer.GetPropertyBlock(_props);
            _props.SetFloat("_CurrentFrame", frame);
            _renderer.SetPropertyBlock(_props);
        }

        #endregion
    }
}