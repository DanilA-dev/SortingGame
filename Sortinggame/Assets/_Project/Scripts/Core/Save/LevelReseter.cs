using D_Dev.SaveSystem.SaveableData;
using D_Dev.ScriptableVariables;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class LevelReseter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private SaveableDataHandler _saveableDataHandler;
        [SerializeField] private IntScriptableVariable[] _runtimeVariables;

        public UnityEvent OnReset;

        #endregion

        #region Public

        public void ResetLevel()
        {
            _saveableDataHandler.ResetAll();

            foreach (var variable in _runtimeVariables)
            {
                if (variable != null)
                    variable.ResetValue();
            }

            OnReset?.Invoke();
        }

        #endregion
    }
}
