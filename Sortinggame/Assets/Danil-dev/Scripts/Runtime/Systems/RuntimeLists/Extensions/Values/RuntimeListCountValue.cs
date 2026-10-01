using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeLists.Extensions
{
    [System.Serializable]
    public sealed class RuntimeListCountValue : PolymorphicValue<int>
    {
        #region Fields

        [SerializeField] private RuntimeList _list;

        private RuntimeList _subscribedList;

        #endregion

        #region Properties

        public override int Value
        {
            get => _list != null ? _list.Count : 0;
            set { }
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<int> Clone()
        {
            return new RuntimeListCountValue { _list = _list };
        }

        #endregion

        #region Overrides

        protected override void SubscribeToSource()
        {
            _subscribedList = _list;

            if (_subscribedList != null)
                _subscribedList.OnChanged += OnListChanged;
        }

        protected override void UnsubscribeFromSource()
        {
            if (_subscribedList != null)
                _subscribedList.OnChanged -= OnListChanged;

            _subscribedList = null;
        }

        #endregion

        #region Listeners

        private void OnListChanged() => RaiseValueChanged(_subscribedList.Count);

        #endregion
    }
}
