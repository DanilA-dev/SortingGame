using System.Linq;
using D_Dev.PolymorphicValueSystem;
using D_Dev.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.StateMachineBehaviour
{
    public abstract class StateMachineController : MonoBehaviour
    {
        #region Fields

        [FoldoutGroup("Base Settings", order:100)]
        [SerializeField, ReadOnly] protected string _currentState;
        [FoldoutGroup("Base Settings", order:100)]
        [SerializeField] protected bool _debugStateChange;

        [Space] [FoldoutGroup("Base Settings", order: 100)] 
        [SerializeReference] protected PolymorphicValue<string> _startState = new StringConstantValue();
        [FoldoutGroup("Base Settings", order: 100)] 
        [SerializeField] protected bool _initStatesOnStart = true;
        [Title("Events")]
        [FoldoutGroup("Base Settings", order:100)]
        [SerializeField] protected StateEvent[] _stateEvents;
        [Space]
        [FoldoutGroup("Base Settings", order:100)]
        public UnityEvent<string> OnAnyStateEnter;
        [FoldoutGroup("Base Settings", order:100)]
        public UnityEvent<string> OnAnyStateExit;

        [FoldoutGroup("Update Intervals")]
        [SerializeField] protected float _minStateTickInterval = 0.05f;
        [FoldoutGroup("Update Intervals")]
        [SerializeField] protected float _maxStateTickInterval = 0.1f;

        [FoldoutGroup("Update Intervals")]
        [SerializeField] protected float _minTransitionTickInterval = 0.1f;
        [FoldoutGroup("Update Intervals")]
        [SerializeField] protected float _maxTransitionTickInterval = 0.2f;

        private float _nextUpdateTick;
        private float _nextTransitionTick;
        
        protected StateMachine.StateMachine _stateMachine;

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            _stateMachine = new StateMachine.StateMachine();
            _stateMachine.OnStateEnter += state =>
            {
                _currentState = state;
                OnAnyStateEnter?.Invoke(state);
                InvokeStateEnterEvent(state);
                StateChangedDebug(state);
            };
            _stateMachine.OnStateExit += InvokeStateExitEvent;

            float startupOffset = Time.time;
            _nextUpdateTick = startupOffset + Random.Range(_minStateTickInterval, _maxStateTickInterval);
            _nextTransitionTick = startupOffset + Random.Range(_minTransitionTickInterval, _maxTransitionTickInterval);
        }

        protected virtual void Start()
        {
            if (!_initStatesOnStart)
                return;

            InitStates();
            ChangeState(_startState.Value);
        }

        #endregion

        #region Public

        public void ManagedUpdate(float currentTime)
        {
            if (_stateMachine == null)
                return;

            if (currentTime >= _nextTransitionTick)
            {
                _stateMachine.CheckTransitions();
                _nextTransitionTick = currentTime + Random.Range(_minTransitionTickInterval, _maxTransitionTickInterval);
            }

            if (currentTime >= _nextUpdateTick)
            {
                _stateMachine.UpdateStates();
                _nextUpdateTick = currentTime + Random.Range(_minStateTickInterval, _maxStateTickInterval);
            }
        }

        public void ManagedFixedUpdate()
        {
            _stateMachine?.UpdateFixedTransitions();
            _stateMachine?.UpdateStatesFixed();
        }

        public void ManualInit()
        {
            InitStates();
            ChangeState(_startState.Value);
        }
        public void ReloadStartState() => ChangeState(_startState.Value);
        
        #endregion

        #region Virtual/Abstract

        protected abstract void InitStates();
        protected virtual void OnFixedUpdate() {}

        #endregion

        #region Protected

        protected void AddState(string stateName, IState state) => _stateMachine?.AddState(stateName, state);
        protected void AddTransition(string[] fromStates, string toState, IStateCondition condition)
        {
            foreach (var fromState in fromStates)
                _stateMachine?.AddTransition(fromState, toState, condition);
        }

        protected void AddFixedTransition(string[] fromStates, string toState, IFixedStateCondition condition)
        {
            foreach (var fromState in fromStates)
                _stateMachine?.AddFixedTransition(fromState, toState, condition);
        }
        
        protected void RemoveTransition(string keyState) => _stateMachine?.RemoveTransition(keyState);
        protected void RemoveFixedTransition(string keyState) => _stateMachine?.RemoveFixedTransition(keyState);
        protected void ChangeState(string stateName) => _stateMachine.ChangeState(stateName);
        
        #endregion

        #region Private

        private void InvokeStateEnterEvent(string state)
        {
            if(_stateEvents.Length <= 0)
                return;
            
            _stateEvents.FirstOrDefault(s => s.State.Value.Equals(state))?.OnStateEnter?.Invoke(state);
        }
        
        private void InvokeStateExitEvent(string state)
        {
            if(_stateEvents.Length <= 0)
                return;
            
            _stateEvents.FirstOrDefault(s => s.State.Value.Equals(state))?.OnStateExit?.Invoke(state);
        }

        #endregion
        
        #region Debug

        protected void StateChangedDebug(string stateName)
        {
            if(_debugStateChange)
                Debug.Log($"[StateBehaviour [<color=pink>{this.name}</color>] entered state <color=yellow>{stateName}</color>");
        }

        #endregion
    }
}
