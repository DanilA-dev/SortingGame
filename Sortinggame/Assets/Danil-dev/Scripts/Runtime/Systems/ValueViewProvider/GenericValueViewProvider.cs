using System;
using System.Globalization;
using D_Dev.TweenAnimations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public abstract class GenericValueViewProvider<TValue, TAnimation> : MonoBehaviour where TAnimation : BaseTweenValueAnimation<TValue>
    {
        #region Fields

        [SerializeField] protected TextMeshProUGUI _text;
        [SerializeField] protected bool _isAnimated;
        [ShowIf(nameof(_isAnimated))]
        [PropertyOrder(100)]
        [SerializeField] protected TAnimation _tweenAnimation;
        [SerializeField] protected string _format;
        
        protected TValue? _currentValue;

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            _tweenAnimation.Text = _text;
        }

        #endregion
        
        #region Protected Methods

        protected virtual void UpdateView(TValue value)
        {
            if (_isAnimated && _tweenAnimation != null)
            {
                _tweenAnimation.StartValue = _currentValue ?? default(TValue);
                _tweenAnimation.EndValue = value;
                _tweenAnimation.Play();
            }
            else
            {
                if (value is IFormattable formattable && !string.IsNullOrEmpty(_format))
                    _text.text = formattable.ToString(_format, CultureInfo.InvariantCulture);
                else
                    _text.text = value.ToString();
            }

            _currentValue = value;
        }

        #endregion
    }
}
