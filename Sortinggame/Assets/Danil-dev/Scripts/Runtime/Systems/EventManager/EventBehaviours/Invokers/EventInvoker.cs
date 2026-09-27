using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.CustomEventManager
{
    public class EventInvoker : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private List<BaseEventNameType> _eventNameTypes = new();
        [SerializeField] private bool _invokeOnStart;
        [SerializeField] private bool _invokeOnlyOnce;
        [Space] 
        [SerializeField] private bool _debug;

        private bool _isInvoked;

        #endregion
        
        #region Monobehaviour

        private void Start()
        {
            if(_invokeOnStart)
                Raise();
        }

        #endregion
        
        #region Public

        public void Raise()
        {
            if(_invokeOnlyOnce && _isInvoked)
                return;
            
            if (_eventNameTypes != null && _eventNameTypes.Count > 0)
            {
                foreach (var eventNameType in _eventNameTypes)
                {
                    EventManager.Invoke(eventNameType.EventName, () =>
                    {
                        if(!_debug)
                            return;

                        Debug.Log(
                            $"<color=yellow>[EventInvoker_{gameObject.name}]</color> raised event <color=orange>{eventNameType.EventName}</color>");

                    });
                }
                _isInvoked = true;
            }
        }

        #endregion
    }
}
