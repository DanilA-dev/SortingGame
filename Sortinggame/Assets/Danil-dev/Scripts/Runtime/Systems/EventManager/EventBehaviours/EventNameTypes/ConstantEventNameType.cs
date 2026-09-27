using UnityEngine;

namespace D_Dev.CustomEventManager
{
    [System.Serializable]
    public class ConstantEventNameType : BaseEventNameType
    {
        #region Fields

        [SerializeField] private EventNameConstants _eventNameConstants;

        #endregion
        
        #region Properties

        public override string EventName => _eventNameConstants.ToString();

        #endregion
    }
}