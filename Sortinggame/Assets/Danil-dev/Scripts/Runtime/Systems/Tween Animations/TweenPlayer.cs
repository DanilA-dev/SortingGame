#if DOTWEEN
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.TweenAnimations
{
    public class TweenPlayer : MonoBehaviour
    {
        #region Enums

        public enum PlayMode
        {
            PlayAll = 0,
            PlayAtIndex = 1
        }

        #endregion

        #region Fields

        [SerializeField] private TweenPlayable _tweenPlayer;
        [Space]
        [PropertyOrder(-100)]
        [SerializeField] private bool _playOnEnable;
        [ShowIf("@_playOnEnable")]
        [PropertyOrder(-10)]
        [SerializeField] private PlayMode _playMode;
        [ShowIf("@_playMode == PlayMode.PlayAtIndex")]
        [PropertyOrder(-10)]
        [SerializeField] private int _startIndex;
        [PropertySpace(20)]
        [FoldoutGroup("Events")]
        public UnityEvent OnStart;
        [FoldoutGroup("Events")]
        public UnityEvent OnComplete;

        #endregion

        #region Properties

        public bool IsComplete { get; private set; }

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_playOnEnable && _tweenPlayer != null)
            {
                if (_playMode == PlayMode.PlayAll)
                    Play();
                else
                    Play(_startIndex);
            }
        }

        private void OnDisable()
        {
            if (_tweenPlayer != null)
            {
                _tweenPlayer.OnStart -= OnTweenPlayableStart;
                _tweenPlayer.OnComplete -= OnTweenPlayableComplete;
            }
        }

        private void OnDestroy()
        {
            _tweenPlayer?.Kill();
        }

        #endregion

        #region Public

        public void Play()
        {
            if (_tweenPlayer == null)
                return;

            IsComplete = false;

            _tweenPlayer.OnStart -= OnTweenPlayableStart;
            _tweenPlayer.OnStart += OnTweenPlayableStart;
            
            _tweenPlayer.OnComplete -= OnTweenPlayableComplete;
            _tweenPlayer.OnComplete += OnTweenPlayableComplete;
            
            _tweenPlayer.Play();
        }

        public void Play(int index)
        {
            if (_tweenPlayer == null)
                return;

            IsComplete = false;

            _tweenPlayer.OnStart += OnTweenPlayableStart;
            _tweenPlayer.OnComplete += OnTweenPlayableComplete;
            _tweenPlayer.Play(index);
        }

        public void Pause()
        {
            if (_tweenPlayer == null)
                return;

            _tweenPlayer.Pause();
        }

        public void Stop()
        {
            if (_tweenPlayer == null)
                return;

            _tweenPlayer.Kill();
        }

        public void Rewind()
        {
            if (_tweenPlayer == null)
                return;

            IsComplete = false;
            _tweenPlayer.Rewind();
        }

        #endregion

        #region Listeners

        private void OnTweenPlayableStart()
        {
            OnStart?.Invoke();
        }

        private void OnTweenPlayableComplete()
        {
            OnComplete?.Invoke();
            IsComplete = true;
        }

        #endregion
    }
}
#endif