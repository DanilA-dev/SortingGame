using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.AnimatorView
{
    public class AnimatorEventHandler : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class AnimatorEvent
        {
            [field: SerializeField] public string EventName { get; private set; }
            [field: SerializeField] public UnityEvent Event { get; private set; }
        }

        #endregion

        #region Fields

        [SerializeField] private AnimatorEvent[] _animatorEvents;

        #endregion

        #region Public

        public void InvokeEvent(string eventName)
        {
            if(_animatorEvents.Length <= 0)
                return;

            for (var i = 0; i < _animatorEvents.Length; i++)
            {
                if(_animatorEvents[i].EventName == eventName)
                    _animatorEvents[i].Event?.Invoke();
            }
        }

        #endregion
    }
}
