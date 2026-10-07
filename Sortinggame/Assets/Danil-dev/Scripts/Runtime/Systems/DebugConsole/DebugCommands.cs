using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.DebugConsole
{
    public class DebugCommands : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private List<IDebugCommand> _commands = new();

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            foreach (var command in _commands)
                command?.Register();
        }

        private void OnDisable()
        {
            foreach (var command in _commands)
                command?.Unregister();
        }

        #endregion
    }
}
