using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.Base
{
    [System.Serializable]
    public abstract class BaseAction
    {
        #region Fields

        [SerializeField, PropertyOrder(-1)] private string _actionName;
        
        [FoldoutGroup("Events"), PropertyOrder(100)] 
        public UnityEvent OnUndo;

        #endregion

        #region Properties

        [ShowInInspector, ReadOnly]
        public bool IsFinished { get; set; }

        #endregion

        #region Abstract/Virtual

        public abstract void Execute();

        public virtual void Undo()
        {
            IsFinished = false;
            OnUndo?.Invoke();
        }

        #endregion
    }
}