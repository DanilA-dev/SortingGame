using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.AnalyticsService
{
    public class AnalyticsService : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _initializeAllModulesOnStart = true;
        [SerializeReference] private List<IAnalyticsModule> _analyticsModules = new();

        #endregion

        #region Monobehaviour

        private void Start() => InitializeAnalyticsModules();

        #endregion

        #region Public

        public void SendEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning("[AnalyticsService] Event name is null or empty");
                return;
            }

            foreach (var module in _analyticsModules)
            {
                if (module.IsInitialized)
                {
                    module.SendEvent(eventName);
                    Debug.Log("[AnalyticsService] " + module.GetType().Name + " event sent: " + eventName);
                    return;
                }
            }

            Debug.Log("[AnalyticsService] No analytics modules are initialized");
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogWarning("[AnalyticsService] Event name is null or empty");
                return;
            }

            foreach (var module in _analyticsModules)
            {
                if (module.IsInitialized)
                {
                    module.SendEvent(eventName, parameters);
                    Debug.Log("[AnalyticsService] " + module.GetType().Name + " event sent: " + eventName + " with parameters: " + 
                             (parameters != null ? string.Join(", ", parameters) : "null"));
                    return;
                }
            }

            Debug.Log("[AnalyticsService] No analytics modules are initialized");
        }

        public void AddAnalyticsModule(IAnalyticsModule module)
        {
            if (module != null && !_analyticsModules.Contains(module))
            {
                _analyticsModules.Add(module);
                Debug.Log($"[AnalyticsModule] Added module {module.GetType().Name}");
            }
        }

        public void RemoveAnalyticsModule(IAnalyticsModule module)
        {
            if (module != null && _analyticsModules.Contains(module))
            {
                _analyticsModules.Remove(module);
                Debug.Log($"[AnalyticsModule] Removed module {module.GetType().Name}");
            }
        }

        #endregion

        #region Private

        private void InitializeAnalyticsModules()
        {
            foreach (var module in _analyticsModules)
            {
                module.Initialize();
                Debug.Log($"[AnalyticsModule] Initialized module {module.GetType().Name}");
            }
            
            if(_initializeAllModulesOnStart)
                Debug.Log("[AnalyticsService] All analytics modules initialized on start");
        }

        #endregion
    }
}
