#if DOTWEEN
using D_Dev.Base;
using UnityEngine;

namespace D_Dev.TweenAnimations.Extensions.Actions
{
    [System.Serializable]
    public class TweenPlayableAction : BaseAction
    {
        #region Fields

        [SerializeField] private TweenPlayable _tweenPlayable;

        private bool _isPlaying;

        #endregion

        #region Overrides

        public override void Execute()
        {
            if (_tweenPlayable == null)
                return;
            
            if (_isPlaying)
                return;

            _tweenPlayable.OnComplete += OnComplete;
            _tweenPlayable.Play();
            _isPlaying = true;
        }

        public override void Undo()
        {
            base.Undo();
            _isPlaying = false;
        }

        #endregion

        #region Listeners

        private void OnComplete()
        {
            IsFinished = true;
            _isPlaying = false;
            if (_tweenPlayable != null)
                _tweenPlayable.OnComplete -= OnComplete;
        }

        #endregion
    }
}
#endif
