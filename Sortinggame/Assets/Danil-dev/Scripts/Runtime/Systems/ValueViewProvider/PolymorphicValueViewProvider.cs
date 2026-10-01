using D_Dev.PolymorphicValueSystem;
using D_Dev.TweenAnimations;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public abstract class PolymorphicValueViewProvider<TValue, TAnimation> : GenericValueViewProvider<TValue, TAnimation>
        where TAnimation : BaseTweenValueAnimation<TValue>
    {
        #region Fields

        [SerializeReference] protected PolymorphicValue<TValue> _value;

        #endregion

        #region Monobehaviour

        protected virtual void OnEnable()
        {
            if (_value == null)
                return;

            _value.OnValueChanged += UpdateView;
            SetViewInstant(_value.Value);
        }

        protected virtual void OnDisable()
        {
            if (_value == null)
                return;

            _value.OnValueChanged -= UpdateView;
        }

        #endregion

        #region Public

        public void SetValue(TValue value)
        {
            if (_value != null)
                _value.Value = value;
            else
                UpdateView(value);
        }

        #endregion
    }
}
