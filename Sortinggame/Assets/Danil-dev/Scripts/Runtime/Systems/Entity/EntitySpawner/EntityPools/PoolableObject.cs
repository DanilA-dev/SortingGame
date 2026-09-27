using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.EntityPool
{
    public class PoolableObject : MonoBehaviour
    {
        #region Fields

        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<PoolableObject> OnEntityGet;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<PoolableObject> OnEntityRelease;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<PoolableObject> OnEntityDestroy;

        #endregion

        #region Monobehaviour

        private void OnDestroy() => OnEntityDestroy?.Invoke(this);
        
        

        #endregion

        #region Public

        public void Get()
        {
            OnGet();
            OnEntityGet?.Invoke(this);
        }
        
        public void Release()
        {
            OnRelease();
            OnEntityRelease?.Invoke(this);
        }

        #endregion

        #region Virtual

        protected virtual void OnGet() {}
        protected virtual void OnRelease() {}

        #endregion
    }
}
