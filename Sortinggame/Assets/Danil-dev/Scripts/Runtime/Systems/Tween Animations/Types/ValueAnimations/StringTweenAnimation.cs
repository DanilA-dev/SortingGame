#if DOTWEEN
using DG.Tweening;
using UnityEngine;

namespace D_Dev.TweenAnimations
{
    [System.Serializable]
    public class StringTweenAnimation : BaseTweenValueAnimation<string>
    {
        #region Constructors

        public StringTweenAnimation() {}

        public StringTweenAnimation(string startText, string endText, float duration, Ease ease = Ease.Linear)
            : base(startText, endText, duration, ease)
        {
        }

        #endregion

        #region Overrides

        public override Tween Play()
        {
            if (_text != null)
                SetTarget(_text.gameObject);
            var fullText = _endValue;
            return Tween = DOTween.To(() => 0, x =>
            {
                var currentLength = x;
                var displayText = fullText.Substring(0, Mathf.Min(currentLength, fullText.Length));
                ApplyValue(displayText);
            }, fullText.Length, Duration);
        }
        #endregion
    }
}
#endif
