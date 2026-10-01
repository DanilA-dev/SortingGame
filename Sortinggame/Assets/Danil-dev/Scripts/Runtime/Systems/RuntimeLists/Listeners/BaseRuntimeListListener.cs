using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.RuntimeLists.Listeners
{
    public abstract class BaseRuntimeListListener<T, TList> : MonoBehaviour
        where TList : BaseRuntimeList<T>
    {
        #region Fields

        [SerializeField] private TList _list;

        [FoldoutGroup("Events")]
        public UnityEvent<T> OnItemAdded;
        [FoldoutGroup("Events")]
        public UnityEvent<T> OnItemRemoved;
        [FoldoutGroup("Events")]
        public UnityEvent<int> OnCountChanged;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_list == null)
                return;

            _list.OnItemAdded += OnListItemAdded;
            _list.OnItemRemoved += OnListItemRemoved;
            _list.OnChanged += OnListChanged;
        }

        private void OnDestroy()
        {
            if (_list == null)
                return;

            _list.OnItemAdded -= OnListItemAdded;
            _list.OnItemRemoved -= OnListItemRemoved;
            _list.OnChanged -= OnListChanged;
        }

        #endregion

        #region Listeners

        protected virtual void OnListItemAdded(T item) => OnItemAdded?.Invoke(item);

        protected virtual void OnListItemRemoved(T item) => OnItemRemoved?.Invoke(item);

        protected virtual void OnListChanged() => OnCountChanged?.Invoke(_list.Count);

        #endregion
    }
}
