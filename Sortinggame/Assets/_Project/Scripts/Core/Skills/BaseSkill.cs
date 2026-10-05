using System.Collections;
using D_Dev.CoroutineManagerSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core.Skills
{
    public abstract class BaseSkill : MonoBehaviour
    {
        #region Enums

        public enum SkillState
        {
            Ready = 0,
            Using = 1,
            Reloading = 2
        }

        #endregion
        
        #region Fields

        [Title("Data")]
        [SerializeField] private SkillInfo _info;
        [SerializeField] private BoolScriptableVariable _skillTriggerVariable;
        [SerializeField, ReadOnly] private SkillState _state;
        [Space]
        [FoldoutGroup("Events"), PropertyOrder(100)] 
        [SerializeField] private UnityEvent _onSkillUseStart;
        [FoldoutGroup("Events"), PropertyOrder(100)] 
        [SerializeField] private UnityEvent _onSkillNotAvailable;
        [FoldoutGroup("Events"), PropertyOrder(100)] 
        [SerializeField] private UnityEvent _onSkillUseStop;
        [FoldoutGroup("Events"), PropertyOrder(100)] 
        [SerializeField] private UnityEvent _onSkillLocked;

        private Coroutine _routine;
        private bool _isUsingImmediately;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_skillTriggerVariable != null)
                _skillTriggerVariable.OnValueUpdate += OnTriggerUpdate;

            if (_info != null)
            {
                _info.OnUseRequested += Use;
                _info.OnUseImmediately += OnUseImmediately;
                _info.OnStopImmediately += OnStopImmediately;
            }
        }

       

        private void OnDisable()
        {
            if (_skillTriggerVariable != null)
                _skillTriggerVariable.OnValueUpdate -= OnTriggerUpdate;

            if (_info != null)
            {
                _info.OnUseRequested -= Use;
                _info.OnUseImmediately -= OnUseImmediately;
                _info.OnStopImmediately -= OnStopImmediately;
            }

            Cancel();
        }

        #endregion

        #region Listeners

        private void OnTriggerUpdate(bool isActive)
        {
            if (isActive)
                Use();
        }

        #endregion

        #region Private

        private void Use()
        {
            if(_info == null)
                return;

            if(_info.IsLocked.Value)
            {
                _onSkillLocked?.Invoke();
                return;
            }

            if (_state != SkillState.Ready || _isUsingImmediately || !CanUse())
            {
                _info.NotifyNotAvailable();
                _onSkillNotAvailable?.Invoke();
                return;
            }

            _routine = CoroutineManager.Run(SkillRoutine());
        }

        private void StopUse()
        {
            OnUseStop();
            _info.NotifyUseStopped();
            _onSkillUseStop?.Invoke();
        }

        private void SetReady()
        {
            _state = SkillState.Ready;
            _info.NotifyReady();
        }

        private void Cancel()
        {
            CoroutineManager.Stop(_routine);
            _routine = null;

            if (_state == SkillState.Using)
                StopUse();

            if (_info != null && _state != SkillState.Ready)
                SetReady();

            OnStopImmediately();
        }

        private void OnUseImmediately()
        {
            if (_info == null || _state == SkillState.Using || _isUsingImmediately)
                return;

            if (!CanUse())
                return;

            _isUsingImmediately = true;
            OnUseStart();
        }

        private void OnStopImmediately()
        {
            if (!_isUsingImmediately)
                return;

            _isUsingImmediately = false;
            OnUseStop();
        }

        #endregion

        #region Coroutine

        private IEnumerator SkillRoutine()
        {
            _state = SkillState.Using;
            OnUseStart();
            _info.NotifyUseStarted();
            _onSkillUseStart?.Invoke();
            yield return CoroutineManager.Wait(_info.Duration.Value);

            StopUse();
            _state = SkillState.Reloading;
            yield return CoroutineManager.Wait(_info.Cooldown.Value);

            _routine = null;
            SetReady();
        }

        #endregion
        
        #region Abstract

        protected abstract void OnUseStart();
        protected virtual void OnUseStop() {}
        protected virtual bool CanUse() => true;

        #endregion
    }
}
