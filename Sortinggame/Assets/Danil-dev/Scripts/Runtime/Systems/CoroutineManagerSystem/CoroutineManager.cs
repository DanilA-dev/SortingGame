using System.Collections;
using System.Collections.Generic;
using D_Dev.Singleton;
using UnityEngine;

namespace D_Dev.CoroutineManagerSystem
{
    public class CoroutineManager : BaseSingleton<CoroutineManager>
    {
        #region Classes

        private class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y) => Mathf.Abs(x - y) < Mathf.Epsilon;
            public int GetHashCode(float obj) => obj.GetHashCode();
        }

        #endregion

        #region Fields

        private static readonly Dictionary<float, WaitForSeconds> _timeIntervals = 
            new Dictionary<float, WaitForSeconds>(new FloatComparer());
            
        private static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeIntervals = 
            new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());
    
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        #endregion

        #region Domain Reload

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _timeIntervals.Clear();
            _realTimeIntervals.Clear();
        }

        #endregion

        #region Public

        public static WaitForSeconds Wait(float seconds)
        {
            if (!_timeIntervals.TryGetValue(seconds, out var wait))
            {
                wait = new WaitForSeconds(seconds);
                _timeIntervals[seconds] = wait;
            }
            return wait;
        }
    
        public static WaitForSecondsRealtime WaitRealtime(float seconds)
        {
            if (!_realTimeIntervals.TryGetValue(seconds, out var wait))
            {
                wait = new WaitForSecondsRealtime(seconds);
                _realTimeIntervals[seconds] = wait;
            }
            return wait;
        }
        
        public static Coroutine Run(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }
    
        public static void Stop(Coroutine routine)
        {
            if (routine != null && _instance != null)
                Instance.StopCoroutine(routine);
        }
        
        public static void StopAll()
        {
            if (_instance != null)
                Instance.StopAllCoroutines();
        }

        #endregion
    }
}