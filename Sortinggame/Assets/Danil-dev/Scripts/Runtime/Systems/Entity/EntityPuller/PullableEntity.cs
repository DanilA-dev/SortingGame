using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.EntityPuller
{
    public class PullableEntity : MonoBehaviour
    {
        #region Fields

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent OnPulled;

        #endregion
        
        #region Monobehaviour

        private void Awake()
        {
            EntityPuller.Subscribe(this);
        }

        private void OnDestroy()
        {
            EntityPuller.Unsubscribe(this);
        }

        #endregion

        #region Public

        public void OnPulledCallback() => OnPulled?.Invoke();

        #endregion
    }
}