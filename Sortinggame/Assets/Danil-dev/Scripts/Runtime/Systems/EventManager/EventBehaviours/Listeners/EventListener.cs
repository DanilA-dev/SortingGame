using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.CustomEventManager.Listeners
{
    public class EventListener : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private List<BaseEventNameType> _eventNameTypes = new();
        [SerializeField] private bool _raiseOnlyOnce;
        [SerializeField] private UnityEvent _onRaised;
        [Space]
        [SerializeField] private bool _debug;

        #endregion

        #region Monobehaviour

        private void Awake() => Subscribe();
        private void OnDestroy() => Unsubscribe();

        #endregion

        #region Private

        private void Subscribe()
        {
            if (_eventNameTypes != null && _eventNameTypes.Count > 0)
            {
                foreach (var eventNameType in _eventNameTypes)
                {
                    EventManager.AddListener(eventNameType.EventName, OnRaised,
                        onAddCallBack: (del) =>
                        {
                            if(!_debug)
                                return;

                            Debug.Log(
                                $"<color=green>[EventListener_{gameObject.name}]</color> subscribed to event <color=orange>{eventNameType.EventName} </color>");


                        }, onFailCallback: (del) =>
                        {
                            if(!_debug)
                                return;

                            Debug.Log(
                                $"<color=green>[EventListener_{gameObject.name}]</color> fail to subscribe to event <color=orange> {eventNameType.EventName} </color>");
                        });
                }
            }
        }

        private void Unsubscribe()
        {
            if (_eventNameTypes != null && _eventNameTypes.Count > 0)
            {
                foreach (var eventNameType in _eventNameTypes)
                {
                    EventManager.RemoveListener(eventNameType.EventName, OnRaised,
                        onRemoveCallback: (del) =>
                        {
                            if(!_debug)
                                return;

                            Debug.Log(
                                $"<color=green>[EventListener_{gameObject.name}]</color> unsubscribed from event <color=orange>{eventNameType.EventName} </color>");


                        }, onFailCallback: (del) =>
                        {
                            if(!_debug)
                                return;

                            Debug.Log(
                                $"<color=green>[EventListener_{gameObject.name}]</color> fail to unsubscribe from event <color=orange> {eventNameType.EventName} </color>");
                        });
                }
            }
        }

        #endregion
        
        #region Listeners

        private void OnRaised()
        {
            _onRaised?.Invoke();

            if (_raiseOnlyOnce)
                Unsubscribe();
        }

        #endregion
    }
}
