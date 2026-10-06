using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Pool;

namespace D_Dev.CustomEventManager
{
    public static class EventManager
    {
        #region Fields

        private static readonly Dictionary<string, List<Delegate>> _events = new();

        #endregion

        #region Private

        private static void Add(string eventType, Delegate value,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null)
        {
            if (value == null)
            {
                onFailCallback?.Invoke(value);
                return;
            }

            if (!_events.TryGetValue(eventType, out var listeners))
            {
                listeners = new List<Delegate>();
                _events[eventType] = listeners;
            }

            listeners.Add(value);
            onAddCallBack?.Invoke(value);
        }

        private static void Remove(string eventType, Delegate value,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null)
        {
            if (!_events.TryGetValue(eventType, out var listeners))
                return;

            int index = listeners.LastIndexOf(value);
            if (index < 0)
                return;

            listeners.RemoveAt(index);
            if (listeners.Count == 0)
                _events.Remove(eventType);

            onRemoveCallback?.Invoke(value);
        }

        private static bool TryGetListeners(string eventType, out List<Delegate> snapshot)
        {
            snapshot = null;

            if (!_events.TryGetValue(eventType, out var listeners) || listeners.Count == 0)
                return false;

            snapshot = ListPool<Delegate>.Get();
            snapshot.AddRange(listeners);
            return true;
        }

        #endregion

        #region Public

        #region Adders

        public static void AddListener(string eventType, Action action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);
        public static void AddListener<T1>(string eventType, Action<T1> action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);
        public static void AddListener<T1, T2>(string eventType, Action<T1, T2> action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);
        public static void AddListener<T1, T2, T3>(string eventType, Action<T1, T2, T3> action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);
        public static void AddListener<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);
        public static void AddListener<T1, T2, T3, T4, T5>(string eventType, Action<T1, T2, T3, T4, T5> action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);
        public static void AddListener<T1, T2, T3, T4, T5, T6>(string eventType, Action<T1, T2, T3, T4, T5, T6> action,
            Action<Delegate> onAddCallBack = null,
            Action<Delegate> onFailCallback = null) => Add(eventType, action, onAddCallBack, onFailCallback);


        #endregion

        #region Removers

        public static void RemoveListener(string eventType, Action action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);
        public static void RemoveListener<T1>(string eventType, Action<T1> action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);
        public static void RemoveListener<T1, T2>(string eventType, Action<T1, T2> action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);
        public static void RemoveListener<T1, T2, T3>(string eventType, Action<T1, T2, T3> action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);
        public static void RemoveListener<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);
        public static void RemoveListener<T1, T2, T3, T4, T5>(string eventType, Action<T1, T2, T3, T4, T5> action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);
        public static void RemoveListener<T1, T2, T3, T4, T5, T6>(string eventType, Action<T1, T2, T3, T4, T5, T6> action,
            Action<Delegate> onRemoveCallback = null,
            Action<Delegate> onFailCallback = null) => Remove(eventType, action, onRemoveCallback, onFailCallback);

        #endregion

        #region Invokers

        public static void Invoke(string eventType, Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action action)
                        action();
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        public static void Invoke<T1>(string eventType, T1 arg1, Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action<T1> action)
                        action(arg1);
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        public static void Invoke<T1, T2>(string eventType, T1 arg1, T2 arg2, Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action<T1, T2> action)
                        action(arg1, arg2);
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        public static void Invoke<T1, T2, T3>(string eventType, T1 arg1, T2 arg2, T3 arg3,
            Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action<T1, T2, T3> action)
                        action(arg1, arg2, arg3);
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        public static void Invoke<T1, T2, T3, T4>(string eventType, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4, Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action<T1, T2, T3, T4> action)
                        action(arg1, arg2, arg3, arg4);
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        public static void Invoke<T1, T2, T3, T4, T5>(string eventType, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4, T5 arg5, Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action<T1, T2, T3, T4, T5> action)
                        action(arg1, arg2, arg3, arg4, arg5);
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        public static void Invoke<T1, T2, T3, T4, T5, T6>(string eventType, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4, T5 arg5, T6 arg6, Action onInvokeCallback = null)
        {
            if (!TryGetListeners(eventType, out var listeners))
                return;

            try
            {
                foreach (var listener in listeners)
                    if (listener is Action<T1, T2, T3, T4, T5, T6> action)
                        action(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            finally
            {
                ListPool<Delegate>.Release(listeners);
            }

            onInvokeCallback?.Invoke();
        }

        #endregion

        #endregion

#if UNITY_EDITOR
        #region Domain Reload

        [InitializeOnEnterPlayMode]
        public static void ResetDomain()
        {
            _events.Clear();
        }

        #endregion
#endif
    }
}
