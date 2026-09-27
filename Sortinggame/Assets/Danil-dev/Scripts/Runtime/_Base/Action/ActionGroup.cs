using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Base
{
    [System.Serializable]
    public class ActionGroup
    {
        #region Fields

        [SerializeField] private bool _executeInParallel = true;
        [Space]
        [ListDrawerSettings(ListElementLabelName = "_actionName")]     
        [SerializeReference] private List<BaseAction> _actions = new();
        
        #endregion

        #region Properties

        public bool ExecuteInParallel => _executeInParallel;
        public List<BaseAction> Actions => _actions;
        public bool IsAllActionsCompleted => Actions.All(a => a == null || a.IsFinished);
        
        #endregion
        
        #region Public
       
        public void ExecuteActions()
        {
            if (Actions == null || Actions.Count == 0)
                return;
            
            if (ExecuteInParallel)
            {
                foreach (var action in Actions)
                {
                    if (action != null && !action.IsFinished)
                        action.Execute();
                }
            }
            else
            {
                foreach (var action in Actions)
                {
                    if(action == null)
                        continue;
                    
                    if (!action.IsFinished)
                    {
                        action.Execute();
                        break;
                    }
                }
            }
        }
       
       #endregion
    }
}
