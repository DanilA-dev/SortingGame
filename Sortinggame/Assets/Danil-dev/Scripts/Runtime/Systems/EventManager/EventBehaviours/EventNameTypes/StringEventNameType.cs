using UnityEngine;

namespace D_Dev.CustomEventManager
{
    [System.Serializable]
    public class StringEventNameType : BaseEventNameType
    {
        #region Fields

        [SerializeField] private string _eventName;

        #endregion

        #region Properties
        public override string EventName => _eventName;

        #endregion
    }
}