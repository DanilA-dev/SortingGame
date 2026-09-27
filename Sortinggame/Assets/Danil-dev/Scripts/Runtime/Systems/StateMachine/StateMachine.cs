using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.StateMachine
{
    public class StateMachine
    {
        #region Fields

        private string _currentState;
        
        private IState _current;
        private Dictionary<string, IState> _states;
        private Dictionary<string, List<StateTransition>> _statesConditions;
        private Dictionary<string, List<FixedStateTransition>> _fixedStatesConditions;
        private CancellationTokenSource _tokenSource;
        
        private bool _isStateSwitching;

        private readonly List<string> _transitionKeys = new();
        private readonly List<string> _fixedTransitionKeys = new();
        
        public UnityAction<string> OnStateEnter;
        public UnityAction<string> OnStateExit;
            
        #endregion

        #region Properties

        public string CurrentState => _currentState;

        #endregion

        #region Constructors

        public StateMachine()
        {
            _states = new();
            _statesConditions = new();
            _fixedStatesConditions = new();
            _tokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Public

        public void AddState(string stateName, IState state) => _states.TryAdd(stateName, state);

        public void AddTransition(string fromState, string toState, IStateCondition condition)
        {
            if (!_statesConditions.TryAdd(fromState, new List<StateTransition> { new(toState, condition) }))
                _statesConditions[fromState].Add(new(toState, condition));
            else
                _transitionKeys.Add(fromState);
        }

        public void AddFixedTransition(string fromState, string toState, IFixedStateCondition condition)
        {
            if (!_fixedStatesConditions.TryAdd(fromState, new List<FixedStateTransition> { new(toState, condition) }))
                _fixedStatesConditions[fromState].Add(new(toState, condition));
            else
                _fixedTransitionKeys.Add(fromState);
        }

        public void RemoveTransition(string keyState)
        {
            if (_statesConditions.ContainsKey(keyState))
            {
                _statesConditions.Remove(keyState);
                _transitionKeys.Remove(keyState);
            }
        }

        public void RemoveFixedTransition(string keyState)
        {
            if (_fixedStatesConditions.ContainsKey(keyState))
            {
                _fixedStatesConditions.Remove(keyState);
                _fixedTransitionKeys.Remove(keyState);
            }
        }

        public void UpdateStates()
        {
            _current?.OnUpdate();
        }

        public void CheckTransitions()
        {
            UpdateTransitions();
        }

        public void UpdateStatesFixed()
        {
            _current?.OnFixedUpdate();
        }

        public async UniTaskVoid ChangeState(string newState)
        {
            if (_isStateSwitching)
                return;

            _isStateSwitching = true;

            try
            {
                if (!_states.TryGetValue(newState, out IState state))
                {
                    Debug.Log($"[StateMachine] {GetType().Name}, State {newState} not found");
                    return;
                }

                if (Equals(_currentState, newState))
                    return;

                if (_current != null)
                {
                    if (_current.ExitTime > 0)
                        await UniTask.Delay(TimeSpan.FromSeconds(_current.ExitTime),
                            cancellationToken: _tokenSource.Token);

                    _current.OnExit();
                    OnStateExit?.Invoke(_currentState);
                }

                _current = state;
                _currentState = newState;
                _current.OnEnter();
                OnStateEnter?.Invoke(newState);
            }
            finally
            {
                _isStateSwitching = false;
            }
        }

        #endregion

        #region Private

        private void UpdateTransitions()
        {
            if (_transitionKeys.Count <= 0)
                return;

            for (int i = 0; i < _transitionKeys.Count; i++)
            {
                var stateName = _transitionKeys[i];

                if (!stateName.Equals(_currentState))
                    continue;

                var transitions = _statesConditions[stateName];

                for (int j = 0; j < transitions.Count; j++)
                {
                    if (transitions[j].Condition.IsMatched() && !_isStateSwitching)
                    {
                        ChangeState(transitions[j].ToState);
                        break;
                    }
                }
            }
        }

        public void UpdateFixedTransitions()
        {
            if (_fixedTransitionKeys.Count <= 0)
                return;

            for (int i = 0; i < _fixedTransitionKeys.Count; i++)
            {
                var stateName = _fixedTransitionKeys[i];

                if (!stateName.Equals(_currentState))
                    continue;

                var transitions = _fixedStatesConditions[stateName];

                for (int j = 0; j < transitions.Count; j++)
                {
                    if (transitions[j].Condition.IsMatched() && !_isStateSwitching)
                    {
                        ChangeState(transitions[j].ToState);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}