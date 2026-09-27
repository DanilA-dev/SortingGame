using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.UpdateManagerSystem
{
    public class LateUpdateManager : MonoBehaviour
    {
        #region Fields

        private static readonly HashSet<ILateTickable> _lateTickables = new();
        private static readonly HashSet<ILateTickable> _pendingAdd = new();
        private static readonly HashSet<ILateTickable> _pendingRemove = new();

        private static readonly List<ILateTickable> _sortedTickables = new();
        private static bool _isSorted = true;

        #endregion

        #region Properties

        public static int Count => _lateTickables.Count + _pendingAdd.Count;

        #endregion

        #region Monobehavior

        private void LateUpdate()
        {
            ProcessPending();
            EnsureSorted();

            foreach (var tickable in _sortedTickables)
            {
                tickable?.LateTick();
            }
        }

        #endregion

        #region Public

        public static void Add(ILateTickable tickable)
        {
            if (tickable != null)
                _pendingAdd.Add(tickable);
        }

        public static void AddWithPriority(ILateTickable tickable, int priority)
        {
            if (tickable != null)
            {
                tickable.SetPriority(priority);
                Add(tickable);
            }
        }

        public static void Remove(ILateTickable tickable)
        {
            if (tickable != null)
                _pendingRemove.Add(tickable);
        }

        public static void Clear()
        {
            _lateTickables.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
            _sortedTickables.Clear();
            _isSorted = true;
        }

        #endregion

        #region Private

        private static void ProcessPending()
        {
            if (_pendingAdd.Count > 0)
            {
                foreach (var tickable in _pendingAdd)
                {
                    if (_lateTickables.Add(tickable))
                        _isSorted = false;
                }
                _pendingAdd.Clear();
            }

            if (_pendingRemove.Count > 0)
            {
                foreach (var tickable in _pendingRemove)
                {
                    _lateTickables.Remove(tickable);
                    _sortedTickables.Remove(tickable);
                }
                _pendingRemove.Clear();
            }
        }

        private static void EnsureSorted()
        {
            if (_isSorted)
                return;

            _sortedTickables.Clear();
            _sortedTickables.AddRange(_lateTickables);
            _sortedTickables.Sort((a, b) => b.GetPriority().CompareTo(a.GetPriority()));
            _isSorted = true;
        }

        #endregion
    }
}