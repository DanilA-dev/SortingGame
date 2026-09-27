using System;
using D_Dev.Base;
using UnityEngine;

namespace D_Dev.Actions
{
    [System.Serializable]
    public class DebugLogAction : BaseAction
    {
        #region Enum

        private enum DebugLogType
        {
            Log,
            Warning,
            Error,
        }

        #endregion
        
        #region Fields

        [SerializeField] private string _message;
        [SerializeField] private DebugLogType _logType;
        [SerializeField] private GameObject _owner;

        #endregion

        #region IAction

        public override void Execute()
        {
            string ownerName = _owner == null? string.Empty : _owner.name;
            switch (_logType)
            {
                case DebugLogType.Log:
                    Debug.Log($"Owner: {ownerName}, Message : {_message}");
                    break;
                case DebugLogType.Warning:
                    Debug.LogWarning($"Owner: {ownerName}, Message : {_message}");
                    break;
                case DebugLogType.Error:
                    Debug.LogError($"Owner: {ownerName}, Message : {_message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            IsFinished = true;
        }

        #endregion
    }
}