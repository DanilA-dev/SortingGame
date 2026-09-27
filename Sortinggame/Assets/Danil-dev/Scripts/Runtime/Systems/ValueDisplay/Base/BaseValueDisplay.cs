using System;
using D_Dev.TweenAnimations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public abstract class BaseValueDisplay<T, TAnimation> : MonoBehaviour
        where TAnimation : BaseTweenValueAnimation<T>
    {
        #region Fields

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private bool _animate;
        [ShowIf(nameof(_animate))]
        [SerializeField] protected TAnimation _tweenAnimation;

        #endregion

        #region Properties

        protected TextMeshProUGUI Text => _text;
        protected bool Animate => _animate;

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            if (_tweenAnimation != null)
                _tweenAnimation.Text = _text;
        }

        #endregion

        #region Public

        public void UpdateText(T value)
        {
            if (!_animate || _tweenAnimation == null)
            {
                _text.SetText(FormatValue(value));
                return;
            }

            if (TryParseCurrentValue(out var parsedValue))
            {
                _tweenAnimation.StartValue = parsedValue;
                _tweenAnimation.EndValue = value;
                _tweenAnimation.Play();
            }
        }

        #endregion

        #region Abstract

        protected abstract string FormatValue(T value);
        protected abstract bool TryParseCurrentValue(out T result);

        #endregion
    }
}