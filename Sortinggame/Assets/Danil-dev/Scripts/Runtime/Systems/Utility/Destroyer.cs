using System.Collections;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.Utility
{
    public class Destroyer : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<float> _destroyTime;
        [Space]
        [FoldoutGroup("Events")]
        public UnityEvent<GameObject> OnDestroyed;

        private Coroutine _destroyCoroutine;
        
        #endregion

        #region Monobehaviour

        private void OnDestroy()
        {
            if(_destroyCoroutine != null)
                StopCoroutine(_destroyCoroutine);
        }

        #endregion

        #region Public

        public void StartDestroying()
        {
            if(_destroyCoroutine != null)
                StopCoroutine(_destroyCoroutine);
            
            _destroyCoroutine = StartCoroutine(DestroyRoutine());
        }

        #endregion

        #region Coroutine

        private IEnumerator DestroyRoutine()
        {
            yield return new WaitForSeconds(_destroyTime.Value);
            OnDestroyed?.Invoke(gameObject);
            Destroy(gameObject);
        }

        #endregion
    }
}