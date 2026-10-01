using System;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class PolymorphicValue<T>
    {
        #region Fields

        private Action<T> _onValueChanged;

        #endregion

        #region Events

        public event Action<T> OnValueChanged
        {
            add
            {
                bool hadListeners = _onValueChanged != null;
                _onValueChanged += value;

                if (!hadListeners && _onValueChanged != null)
                    SubscribeToSource();
            }
            remove
            {
                if (_onValueChanged == null)
                    return;

                _onValueChanged -= value;

                if (_onValueChanged == null)
                    UnsubscribeFromSource();
            }
        }

        #endregion

        #region Properties

        public abstract T Value { get; set; }

        #endregion

        #region Cloning

        public abstract PolymorphicValue<T> Clone();

        #endregion

        #region Protected

        protected void RaiseValueChanged(T value) => _onValueChanged?.Invoke(value);

        protected virtual void SubscribeToSource() { }

        protected virtual void UnsubscribeFromSource() { }

        #endregion

        #region Overrides

        public virtual void Dispose()
        {
            if (_onValueChanged != null)
                UnsubscribeFromSource();

            _onValueChanged = null;
        }

        #endregion
    }
}
