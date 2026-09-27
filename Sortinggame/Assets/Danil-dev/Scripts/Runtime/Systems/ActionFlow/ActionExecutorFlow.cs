using D_Dev.Base;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ActionFlow
{
    public class ActionExecutorFlow : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _executeOnStart;
        [SerializeField] private bool _resetOnFinish;
        [SerializeField] private bool _saveState;
        [ShowIf(nameof(_saveState))]
        [SerializeField] private string _saveID;

        [Space]
        [Title("Actions")]
        [SerializeReference] private List<ActionGroup> _actionGroups = new();
        [Title("Breakers")]
        [SerializeReference] private List<ICondition> _breakConditions = new();
        [Space]
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onStarted;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onFinished;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPaused;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onResumed;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<int> _onGroupStarted;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<int> _onGroupFinished;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent<int> _onGroupTimeout;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onBroken;
        [PropertySpace(10)]
        [SerializeField, ReadOnly] private bool _isPaused;
        [SerializeField, ReadOnly] private bool _isFinished;

        private int _currentGroupIndex;
        private float _currentGroupStartTime;
        private bool _isStarted;
        private Coroutine _executionCoroutine;

        #endregion

        #region Properties

        public bool IsFinished => _isFinished;
        public bool IsPaused => _isPaused;
        public int CurrentGroupIndex => _currentGroupIndex;
        public ActionGroup CurrentGroup =>
            _actionGroups != null && _currentGroupIndex < _actionGroups.Count
                ? _actionGroups[_currentGroupIndex]
                : null;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if (_executeOnStart)
                StartExecution();
        }

        private void OnDisable()
        {
            if (_executionCoroutine != null)
            {
                StopCoroutine(_executionCoroutine);
                _executionCoroutine = null;
            }
            _isStarted = false;
        }

        #endregion

        #region Public

        [FoldoutGroup("Debug")]
        [Button]
        public void StartExecution()
        {
            if (IsFinished || _isStarted)
                return;

            if (_actionGroups == null || _actionGroups.Count == 0)
                return;

            if (GetSavedState())
            {
                _isFinished = true;
                return;
            }

            _currentGroupIndex = GetLastActiveState()
                ? GetLastSavedGroup()
                : 0;

            _currentGroupIndex = Mathf.Clamp(_currentGroupIndex, 0, _actionGroups.Count - 1);

            _isStarted = true;
            _executionCoroutine = StartCoroutine(ExecuteCoroutine());
            _onStarted?.Invoke();
        }

        public void Pause()
        {
            if (!_isPaused && !IsFinished)
            {
                _isPaused = true;
                _onPaused?.Invoke();
            }
        }

        public void Resume()
        {
            if (_isPaused && !IsFinished)
            {
                _isPaused = false;
                _onResumed?.Invoke();
            }
        }

        public void Stop()
        {
            _isFinished = true;
            _isStarted = false;

            if (_executionCoroutine != null)
            {
                StopCoroutine(_executionCoroutine);
                _executionCoroutine = null;
            }

            _onFinished?.Invoke();

            if (_saveState)
                SaveState();
        }

        public void ResetBreakers()
        {
            if (_breakConditions == null)
                return;

            foreach (var condition in _breakConditions)
                condition?.Reset();
        }

        public void ResetActions()
        {
            if(_actionGroups == null)
                return;

            foreach (var group in _actionGroups)
            {
                foreach (var groupAction in group.Actions)
                    groupAction.Undo();
            }
        }

        public void FullReset()
        {
            _isFinished = false;
            _isStarted = false;
            
            ResetActions();
            ResetBreakers();
        }

        #endregion

        #region Private

        private IEnumerator ExecuteCoroutine()
        {
            while (true)
            {
                if (CheckBreakers())
                    yield break;

                if (_actionGroups == null || _actionGroups.Count == 0)
                {
                    Finish();
                    yield break;
                }

                if (_currentGroupIndex >= _actionGroups.Count)
                {
                    Finish();
                    yield break;
                }

                ActionGroup currentGroup = _actionGroups[_currentGroupIndex];
                if (currentGroup == null)
                {
                    MoveToNext();
                    continue;
                }

                if (currentGroup.IsAllActionsCompleted)
                {
                    OnGroupFinished();
                    MoveToNext();
                    continue;
                }

                if (_currentGroupStartTime == 0)
                    StartCurrent();

                currentGroup.ExecuteActions();

                if (_isPaused)
                {
                    while (_isPaused)
                        yield return null;
                }

                yield return null;
            }
        }

        private void StartCurrent()
        {
            _currentGroupStartTime = Time.time;
            _onGroupStarted?.Invoke(_currentGroupIndex);

            if (_saveState)
                SaveLastActive();
        }

        private void OnGroupFinished()
        {
            _onGroupFinished?.Invoke(_currentGroupIndex);
            _currentGroupStartTime = 0;
        }

        private void MoveToNext()
        {
            _currentGroupIndex++;
            _currentGroupStartTime = 0;

            if (_saveState)
                SaveLastActive();
        }

        private void Finish()
        {
            if (!IsFinished)
            {
                _isFinished = true;
                _isStarted = false;
                _currentGroupIndex = 0;
                _onFinished?.Invoke();
                
                if(_resetOnFinish)
                    FullReset();
                
                if (_saveState)
                    SaveState();
            }
        }

        private bool CheckBreakers()
        {
            if (_breakConditions == null || _breakConditions.Count == 0)
                return false;

            foreach (var condition in _breakConditions)
            {
                if (condition == null)
                    continue;

                if (condition.IsConditionMet())
                {
                    Debug.Log($"[ActionExecuter] Broken by condition");

                    _onBroken?.Invoke();

                    Finish();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Saves

        private bool GetSavedState()
        {
            return _saveState && PlayerPrefs.GetInt(_saveID, 0) == 1;
        }

        private bool GetLastActiveState()
        {
            return _saveState && PlayerPrefs.HasKey(GetLastSaved());
        }

        private int GetLastSavedGroup()
        {
            _currentGroupIndex = PlayerPrefs.GetInt(GetLastSaved(), 0);
            return _currentGroupIndex;
        }

        private void SaveState()
        {
            PlayerPrefs.SetInt(_saveID, IsFinished ? 1 : 0);
        }

        private void SaveLastActive()
        {
            PlayerPrefs.SetInt(GetLastSaved(), _currentGroupIndex);
        }

        private string GetLastSaved()
        {
            return $"{_saveID}_lastActive";
        }

        #endregion
    }
}
