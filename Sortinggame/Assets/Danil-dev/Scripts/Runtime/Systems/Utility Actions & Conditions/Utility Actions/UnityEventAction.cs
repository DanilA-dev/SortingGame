using D_Dev.Base;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.Actions
{
    [System.Serializable]
    public class UnityEventAction : BaseAction
    {
        #region Fields

        [SerializeField] private UnityEvent Event;

        #endregion
        public override void Execute()
        {
            Event?.Invoke();
            IsFinished = true;
        }
    }
}