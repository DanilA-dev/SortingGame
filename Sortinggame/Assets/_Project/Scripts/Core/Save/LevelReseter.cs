using D_Dev.SaveSystem.SaveableData;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class LevelReseter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private SaveableDataHandler _saveableDataHandler;

        public UnityEvent OnReset;

        #endregion

        #region Public

        public void ResetLevel()
        {
            _saveableDataHandler.ResetAll();
            OnReset?.Invoke();
        }

        #endregion
    }
}
