using D_Dev.TweenAnimations.Types;
using DG.Tweening;
using UnityEngine;

#if DOTWEEN
namespace D_Dev.TweenAnimations
{
    [System.Serializable]
    public abstract class BaseTweenValueAnimation<T> : BaseAnimationTween
    {
        #region Fields

        [SerializeField] protected T _startValue;
        [SerializeField] protected T _endValue;
        [SerializeField] protected TMPro.TextMeshProUGUI _text;

        #endregion

        #region Properties

        public T StartValue
        {
            get => _startValue;
            set => _startValue = value;
        }

        public T EndValue
        {
            get => _endValue;
            set => _endValue = value;
        }

        public TMPro.TextMeshProUGUI Text
        {
            get => _text;
            set => _text = value;
        }

        #endregion

        #region Constructors

        public BaseTweenValueAnimation() {}

        public BaseTweenValueAnimation(T startValue, T endValue, float duration, Ease ease = Ease.Linear)
        {
            _startValue = startValue;
            _endValue = endValue;
            _duration = duration;
            _ease = ease;
        }

        public BaseTweenValueAnimation(TMPro.TextMeshProUGUI target, T startValue, T endValue, float duration, Ease ease = Ease.Linear)
            : this(startValue, endValue, duration, ease)
        {
            _text = target;
        }

        #endregion

        #region Virtual/Abstract

        protected virtual void ApplyValue(T value)
        {
            if(_text != null)
                _text.text = value.ToString();
        }

        public abstract override Tween Play();

        #endregion
    }
}
#endif
