using UnityEngine;

namespace D_Dev.RuntimeLists.Adders
{
    public abstract class BaseRuntimeListAdder<T, TList> : MonoBehaviour
        where T : Object
        where TList : BaseRuntimeList<T>
    {
        #region Fields

        [SerializeField] protected TList _list;
        [SerializeField] protected T _item;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_list == null)
                return;

            _list.Add(GetItem());
        }

        private void OnDisable()
        {
            if (_list == null)
                return;

            _list.Remove(GetItem());
        }

        #endregion

        #region Protected

        protected abstract T GetDefaultItem();

        #endregion

        #region Private

        private T GetItem() => _item != null ? _item : GetDefaultItem();

        #endregion
    }
}
