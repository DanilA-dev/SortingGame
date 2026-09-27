using UnityEngine.Animations;
using UnityEngine.Playables;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    public class AnimationAnimatorPlayableMixer : BaseAnimationPlayableMixer
    {
        #region Fields

        private AnimatorControllerPlayable _animatorPlayable;

        #endregion

        #region Properties

        public AnimatorControllerPlayable AnimatorPlayable => _animatorPlayable;

        #endregion

        #region Monobehaviour

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_playableGraph?.PlayableGraph.IsValid() != true)
                return;

            _animatorPlayable = AnimatorControllerPlayable.Create(_playableGraph.PlayableGraph,
                _playableGraph.Animator.runtimeAnimatorController);
            _playableGraph.RootLayerMixer.ConnectInput(_layer, _animatorPlayable, 0, 1);
            _playableGraph.RootLayerMixer.SetLayerAdditive((uint)_layer, _isAdditive);

            _playableGraph.Animator.runtimeAnimatorController = null;
        }

        private void OnDisable()
        {
            if (_playableGraph?.PlayableGraph.IsValid() == true &&
                _playableGraph.RootLayerMixer.IsValid())
            {
                _playableGraph.RootLayerMixer.DisconnectInput(_layer);
            }

            if (_animatorPlayable.IsValid())
            {
                _playableGraph?.PlayableGraph.DestroyPlayable(_animatorPlayable);
            }

            if (_playableGraph?.Animator != null && _playableGraph.Animator.runtimeAnimatorController == null)
            {
                // Restore controller if needed
            }
        }

        #endregion
    }
}
