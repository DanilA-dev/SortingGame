namespace D_Dev.AnimatorView.Extensions
{
    public class AnimatorViewStatePlayer : BaseAnimationStatePlayer<AnimationClipConfig, AnimatorView>
    {
        protected override void OnPlay(AnimationClipConfig config)
        {
            _player.PlayAnimation(config);
        }
    }
}
