using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace  D_Dev.StateMachineBehaviour
{
    public class ComponentStateMachineController  : StateMachineController
    {
        #region Fields

        [FoldoutGroup("Module Settings", 99)]
        [SerializeField] protected bool _findOnObject;
        [FoldoutGroup("Module Settings", 99)]
        [HideIf(nameof(_findOnObject))]
        [SerializeField] protected BaseComponentState[] _states;

        #endregion

        #region Virtual/Abstract

        protected override void InitStates()
        {
            if (_findOnObject)
            {
                List<BaseComponentState> allObjectStates = new();

                var componentStates = GetComponents<BaseComponentState>();
                var childStates = GetComponentsInChildren<BaseComponentState>();

                if (componentStates != null)
                {
                    foreach (var state in componentStates)
                        allObjectStates.Add(state);
                }

                if (childStates != null)
                {
                    foreach (var state in childStates)
                        allObjectStates.Add(state);
                }

                _states = allObjectStates.ToArray();
            }
            
            if (_states == null || _states.Length == 0)
                return;
            
            foreach (var state in _states)
                AddState(state.StateName, state);
            
            InitTransitions();
            OnStatesInitialized();
        }

        protected virtual void InitTransitions()
        {
            if (_states == null || _states.Length == 0)
                return;
            
            foreach (var state in _states)
            {
                if (state == null || state.Transitions == null || state.Transitions.Length == 0)
                    continue;
                
                foreach (var transition in state.Transitions)
                {
                    if (transition == null)
                        continue;

                    var condition = new CompositeTransitionCondition(
                        transition.Conditions,
                        transition.FixedConditions,
                        transition.MatchMode,
                        () => state.CanBeTransitioned.Value);

                    if (condition.IsEmpty)
                        continue;

                    if (condition.HasFixedConditions)
                        AddFixedTransition(transition.FromStates, state.StateName, condition);
                    else
                        AddTransition(transition.FromStates, state.StateName, condition);
                }
            }
        }
        
        protected virtual void OnStatesInitialized(){}
        
        #endregion
    }
}
