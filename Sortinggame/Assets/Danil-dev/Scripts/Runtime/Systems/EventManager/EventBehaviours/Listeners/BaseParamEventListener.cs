using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.CustomEventManager.Listeners
{
    public abstract class BaseParamEventListener<T> : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private List<BaseEventNameType> _eventNameTypes = new();
        [SerializeField] private bool _raiseOnlyOnce;
        [SerializeField] private UnityEvent<T> _onRaised;
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
                    EventManager.AddListener<T>(eventNameType.EventName, OnRaised,
                        onAddCallBack: (del) =>
                        {
                            if(!_debug)
                                return;

                            Debug.Log(
                                $"<color=yellow>[EventListener_{gameObject.name}]</color> subscribed to event <color=orange>{eventNameType.EventName} </color>");


                        }, onFailCallback: (del) =>
                        {
                            if(!_debug)
                                return;
                    
                            Debug.Log(
                                $"<color=yellow>[EventListener_{gameObject.name}]</color> fail to subscribe to event <color=orange>{eventNameType.EventName} </color>," +
                                $" - Type mismatch: {typeof(T).Name} and {del.GetMethodInfo().Name}");
                    
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
                    EventManager.RemoveListener<T>(eventNameType.EventName, OnRaised,
                        onRemoveCallback: (del) =>
                        {
                            if(!_debug)
                                return;

                            Debug.Log(
                                $"<color=yellow>[EventListener_{gameObject.name}]</color> unsubscribed from event <color=orange> {eventNameType.EventName} </color>");


                        }, onFailCallback: (del) =>
                        {
                            if(!_debug)
                                return;
                    
                            Debug.Log(
                                $"<color=yellow>[EventListener_{gameObject.name}]</color> fail to unsubscribe from event <color=orange> {eventNameType.EventName} </color>," +
                                $" - Type mismatch: {typeof(T).Name} and {del.GetMethodInfo().Name}");
                        });
                }
            }
        }

        #endregion

        #region Listeners

        private void OnRaised(T t)
        {
            _onRaised?.Invoke(t);

            if (_raiseOnlyOnce)
                Unsubscribe();
        }

        #endregion
    }
}
