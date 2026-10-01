#if DOTWEEN
using DG.Tweening;

namespace D_Dev.TweenAnimations
{
    [System.Serializable]
    public class DoubleTweenAnimation : BaseTweenValueAnimation<double>
    {
        #region Constructors

        public DoubleTweenAnimation() {}

        public DoubleTweenAnimation(double startValue, double endValue, float duration, Ease ease = Ease.Linear)
            : base(startValue, endValue, duration, ease)
        {
        }

        #endregion

        #region Overrides
        public override Tween Play()
        {
            if (_text != null)
                SetTarget(_text.gameObject);
            return Tween = DOTween.To(() => _startValue, x => ApplyValue(x), _endValue, Duration).SetEase(_ease);
        }
        #endregion
    }
}
#endif
