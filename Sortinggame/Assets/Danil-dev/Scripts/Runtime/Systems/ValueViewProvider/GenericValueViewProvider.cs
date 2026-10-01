using System;
using System.Globalization;
using D_Dev.TweenAnimations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public abstract class GenericValueViewProvider<TValue, TAnimation> : MonoBehaviour
        where TAnimation : BaseTweenValueAnimation<TValue>
    {
        #region Fields

        [SerializeField] protected TMP_Text _text;
        [SerializeField] protected string _format;
        [SerializeField] protected bool _isAnimated;
        [ShowIf(nameof(_isAnimated))]
        [PropertyOrder(100)]
        [SerializeField] protected TAnimation _tweenAnimation;

        private TValue _displayedValue;
        private bool _hasValue;

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            if (_tweenAnimation == null)
                return;

            _tweenAnimation.Text = _text;
            _tweenAnimation.Formatter = OnAnimatedValueApplied;
        }

        protected virtual void OnDestroy() => _tweenAnimation?.Kill();

        #endregion

        #region Protected

        protected virtual void UpdateView(TValue value)
        {
            if (_text == null)
                return;

            if (!_isAnimated || _tweenAnimation == null || !_hasValue)
            {
                SetViewInstant(value);
                return;
            }

            _tweenAnimation.Kill();
            _tweenAnimation.StartValue = _displayedValue;
            _tweenAnimation.EndValue = value;
            _tweenAnimation.Play();
        }

        protected void SetViewInstant(TValue value)
        {
            if (_text == null)
                return;

            _tweenAnimation?.Kill();
            _displayedValue = value;
            _hasValue = true;
            _text.text = FormatValue(value);
        }

        protected virtual string FormatValue(TValue value)
        {
            if (value == null)
                return string.Empty;

            if (string.IsNullOrEmpty(_format))
                return value.ToString();

            if (_format.Contains("{0"))
                return string.Format(CultureInfo.InvariantCulture, _format, value);

            if (value is IFormattable formattable)
                return formattable.ToString(_format, CultureInfo.InvariantCulture);

            return value.ToString();
        }

        #endregion

        #region Private

        private string OnAnimatedValueApplied(TValue value)
        {
            _displayedValue = value;
            return FormatValue(value);
        }

        #endregion
    }
}
