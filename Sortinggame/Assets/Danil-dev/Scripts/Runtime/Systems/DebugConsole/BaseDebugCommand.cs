using System;
using IngameDebugConsole;
using UnityEngine;

namespace D_Dev.DebugConsole
{
    [Serializable]
    public abstract class BaseDebugCommand : IDebugCommand
    {
        #region Fields

        [SerializeField] protected string _command;
        [SerializeField] protected string _description;

        #endregion

        #region Properties

        public string Command => _command;

        #endregion

        #region IDebugCommand

        public virtual void Register()
        {
            if (string.IsNullOrWhiteSpace(_command))
            {
                Debug.Log($"[{GetType().Name}] Command name is empty, skipped");
                return;
            }

            DebugLogConsole.AddCommand(_command, _description, GetMethod(), GetParameterNames());
        }

        public virtual void Unregister()
        {
            if (!string.IsNullOrWhiteSpace(_command))
                DebugLogConsole.RemoveCommand(_command);
        }

        #endregion

        #region Abstract

        protected abstract Delegate GetMethod();
        protected virtual string[] GetParameterNames() => null;

        #endregion
    }
}
