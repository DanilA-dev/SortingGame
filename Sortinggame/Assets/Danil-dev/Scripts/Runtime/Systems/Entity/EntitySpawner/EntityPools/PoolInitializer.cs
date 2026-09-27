using UnityEngine;

namespace D_Dev.EntityPool
{
    public class PoolInitializer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private PoolableDataList[] _pools;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            foreach (var pool in _pools)
                pool.Initialize();
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools)
                pool.Dispose();
        }

        #endregion
    }
}