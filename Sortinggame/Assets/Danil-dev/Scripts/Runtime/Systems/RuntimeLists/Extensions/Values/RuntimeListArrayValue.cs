using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeLists.Extensions
{
    [System.Serializable]
    public abstract class RuntimeListArrayValue<TList, T> : PolymorphicValue<T[]>
        where TList : BaseRuntimeList<T>
    {
        #region Fields

        [SerializeField] protected TList _list;

        [NonSerialized] private TList _subscribedList;

        #endregion

        #region Properties

        public override T[] Value
        {
            get => _list != null ? _list.ToArray() : null;
            set
            {
                if (_list == null)
                    return;

                _list.Set(value);
            }
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

        private void OnListChanged() => RaiseValueChanged(_subscribedList.ToArray());

        #endregion
    }
}
