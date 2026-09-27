#if DOTWEEN
using System;
using System.Collections.Generic;
using DG.Tweening;
using D_Dev.TweenAnimations.Types;
using UnityEngine;

namespace D_Dev.TweenAnimations
{
    [System.Serializable]
    public class TweenPlayable
    {
        #region Enums

        public enum PlayMode
        {
            Parallel = 0,
            Sequential = 1
        }

        #endregion

        #region Fields

        [SerializeField] private PlayMode _playMode;
        [SerializeField] private bool _ignoreTimeScale;
        [SerializeReference] protected List<BaseAnimationTween> _tweens = new();
        
        private Sequence _currentSequence;
        
        public event Action OnStart;
        public event Action OnComplete;
        
        #endregion

        #region Public

        public void Play()
        {
            if (!HasTweensInArray())
                return; 

            Kill();
            OnStart?.Invoke();

            _currentSequence = DOTween.Sequence();
            if (_playMode == PlayMode.Parallel)
            {
                foreach (var tween in _tweens)
                    _currentSequence.Join(tween.Play());
            }
            else
            {
                foreach (var tween in _tweens)
                    _currentSequence.Append(tween.Play());
            }

            _currentSequence.SetAutoKill(true)
                .SetUpdate(_ignoreTimeScale)
                .OnComplete(OnTweensComplete);
        }

        public void Play(int index)
        {
            if (!HasTweensInArray() || index < 0 || index >= _tweens.Count)
                return;

            Kill();
            OnStart?.Invoke();

            _tweens[index].Play();
            _tweens[index].OnComplete.AddListener(OnTweensComplete);
        }

        public void Pause()
        {
            if (!HasTweensInArray())
                return;

            foreach (var tween in _tweens)
                tween.Pause();
        }

        public void Kill()
        {
            if (!HasTweensInArray())
                return;

            _currentSequence?.Kill();
            _currentSequence = null;

            foreach (var tween in _tweens)
            {
                tween.Kill();
                tween.OnComplete.RemoveAllListeners();
            }
        }

        public void Rewind()
        {
            if (!HasTweensInArray())
                return;

            if (_currentSequence != null && _currentSequence.IsActive())
            {
                _currentSequence.Rewind();
                return;
            }

            Kill();

            _currentSequence = DOTween.Sequence();
            if (_playMode == PlayMode.Parallel)
            {
                foreach (var tween in _tweens)
                    _currentSequence.Join(tween.Play());
            }
            else
            {
                foreach (var tween in _tweens)
                    _currentSequence.Append(tween.Play());
            }

            _currentSequence.SetAutoKill(false)
                .SetUpdate(_ignoreTimeScale);

            _currentSequence.Rewind();
        }

        #endregion

        #region Listeners

        private void OnTweensComplete()
        {
            if(!HasTweensInArray())
                return;
                
            OnComplete?.Invoke();
            
            foreach (var tween in _tweens)
                tween.OnComplete.RemoveAllListeners();
        }

        #endregion
        
        #region Private

        private bool HasTweensInArray() => _tweens != null && _tweens.Count > 0;

        #endregion
    }
}
#endif
