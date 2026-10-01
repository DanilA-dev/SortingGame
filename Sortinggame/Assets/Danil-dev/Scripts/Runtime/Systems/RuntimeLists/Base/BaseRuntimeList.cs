using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace D_Dev.RuntimeLists
{
    public abstract class BaseRuntimeList<T> : RuntimeList
    {
        #region Fields

        [ShowInInspector, ReadOnly] private readonly List<T> _items = new();
        private readonly HashSet<T> _lookup = new();

        private T[] _cachedArray;

        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;

        #endregion

        #region Properties

        public IReadOnlyList<T> Items => _items;
        public override int Count => _items.Count;

        public T this[int index] => _items[index];

        #endregion

        #region Public

        public bool Add(T item)
        {
            if (item == null || !_lookup.Add(item))
                return false;

            _items.Add(item);
            _cachedArray = null;

            OnItemAdded?.Invoke(item);
            RaiseChanged();
            return true;
        }

        public bool Remove(T item)
        {
            if (item == null || !_lookup.Remove(item))
                return false;

            _items.Remove(item);
            _cachedArray = null;

            OnItemRemoved?.Invoke(item);
            RaiseChanged();
            return true;
        }

        public bool Contains(T item) => item != null && _lookup.Contains(item);

        public void Set(IEnumerable<T> items)
        {
            Clear();

            if (items == null)
                return;

            foreach (var item in items)
                Add(item);
        }

        public override void Clear()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (i < _items.Count)
                    Remove(_items[i]);
            }
        }

        public T[] ToArray()
        {
            _cachedArray ??= _items.ToArray();
            return _cachedArray;
        }

        #endregion

        #region Overrides

        protected override void ClearSilently()
        {
            _items.Clear();
            _lookup.Clear();
            _cachedArray = null;
        }

        #endregion
    }
}
