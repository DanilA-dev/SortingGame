using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Singleton
{
    public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [SerializeField] private bool _createInstanceIfNotFound = true;
      
        protected static T _instance;
        private static readonly object _lock = new object();
        
        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            var baseSingleton = FindObjectOfType<BaseSingleton<T>>();
                            if (baseSingleton != null && baseSingleton._createInstanceIfNotFound)
                            {
                                var singletonObject = new GameObject();
                                _instance = singletonObject.AddComponent<T>();
                                singletonObject.name = $"{typeof(T).Name} (Singleton)";

                                Debug.Log($"[Singleton] Created new instance of {typeof(T)}");
                            }
                        }
                    }
                    return _instance;
                }
            }
        }

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (_dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.Log($"[Singleton] Another instance of {typeof(T)} already exists. Destroying this one.");
                Destroy(gameObject);
            }
        }
       
        #endregion
    }
}
