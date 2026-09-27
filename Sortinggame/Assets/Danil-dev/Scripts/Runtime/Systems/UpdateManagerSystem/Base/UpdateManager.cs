using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.UpdateManagerSystem
{
    public class UpdateManager : MonoBehaviour
    {
        #region Fields

        private static readonly HashSet<ITickable> _tickables = new();
        private static readonly HashSet<ITickable> _pendingAdd = new();
        private static readonly HashSet<ITickable> _pendingRemove = new();

        private static readonly List<ITickable> _sortedTickables = new();
        private static bool _isSorted = true;

        #endregion

        #region Properties

        public static int Count => _tickables.Count + _pendingAdd.Count;

        #endregion

        #region Domain Reload

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Clear();

        #endregion

        #region Monobehavior

        private void Update()
        {
            ProcessPending();
            EnsureSorted();

            foreach (var tickable in _sortedTickables)
                tickable?.Tick();
        }

        #endregion

        #region Public

        public static void Add(ITickable tickable)
        {
            if (tickable != null)
                _pendingAdd.Add(tickable);
        }

        public static void AddWithPriority(ITickable tickable, int priority)
        {
            if (tickable != null)
            {
                tickable.SetPriority(priority);
                Add(tickable);
            }
        }

        public static void Remove(ITickable tickable)
        {
            if (tickable != null)
                _pendingRemove.Add(tickable);
        }

        public static void Clear()
        {
            _tickables.Clear();
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
                    if (_tickables.Add(tickable))
                        _isSorted = false;
                }
                _pendingAdd.Clear();
            }

            if (_pendingRemove.Count > 0)
            {
                foreach (var tickable in _pendingRemove)
                {
                    _tickables.Remove(tickable);
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
            _sortedTickables.AddRange(_tickables);
            _sortedTickables.Sort((a, b) => b.GetPriority().CompareTo(a.GetPriority()));
            _isSorted = true;
        }

        #endregion
    }
}