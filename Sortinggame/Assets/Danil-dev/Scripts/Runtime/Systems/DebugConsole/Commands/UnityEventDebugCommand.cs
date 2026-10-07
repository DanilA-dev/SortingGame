using System;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.DebugConsole
{
    [Serializable]
    public class UnityEventDebugCommand : BaseDebugCommand
    {
        #region Fields

        [SerializeField] private UnityEvent _onExecute;

        #endregion

        #region Overrides

        protected override Delegate GetMethod() => (Action)Execute;

        #endregion

        #region Private
        private void Execute() => _onExecute?.Invoke();

        #endregion
    }

    [Serializable]
    public abstract class UnityEventDebugCommand<T> : BaseDebugCommand
    {
        #region Fields

        [SerializeField] private string _parameterName = "value";
        [SerializeField] private UnityEvent<T> _onExecute;

        #endregion

        #region Overrides

        protected override Delegate GetMethod() => (Action<T>)Execute;
        protected override string[] GetParameterNames() => new[] { _parameterName };

        #endregion

        #region Private
        private void Execute(T value) => _onExecute?.Invoke(value);

        #endregion
    }

    [Serializable] public class IntUnityEventDebugCommand : UnityEventDebugCommand<int> { }
    [Serializable] public class FloatUnityEventDebugCommand : UnityEventDebugCommand<float> { }
    [Serializable] public class BoolUnityEventDebugCommand : UnityEventDebugCommand<bool> { }
    [Serializable] public class StringUnityEventDebugCommand : UnityEventDebugCommand<string> { }
}
