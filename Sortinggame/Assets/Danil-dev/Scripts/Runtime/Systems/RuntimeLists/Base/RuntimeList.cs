using System;
using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.RuntimeLists
{
    public abstract class RuntimeList : ScriptableObject
    {
        #region Fields

        private static readonly HashSet<RuntimeList> _activeLists = new();

        public event Action OnChanged;

        #endregion

        #region Properties

        public abstract int Count { get; }

        #endregion

        #region ScriptableObject

        protected virtual void OnEnable()
        {
            _activeLists.Add(this);
            ClearSilently();
        }

        protected virtual void OnDisable() => _activeLists.Remove(this);

        #endregion

        #region Public

        public abstract void Clear();

        #endregion

        #region Protected

        protected void RaiseChanged() => OnChanged?.Invoke();

        protected abstract void ClearSilently();

        #endregion

        #region DomainReload

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearAllOnEnterRuntime()
        {
            foreach (var list in _activeLists)
                list.ClearSilently();
        }

        #endregion
    }
}
