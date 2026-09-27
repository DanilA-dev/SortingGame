using System.Collections.Generic;
using D_Dev.AnimatorView.AnimationPlayableHandler;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace D_Dev.AnimatorView
{
    #region Enum

    public enum RuntimeAnimatorType
    {
        RegularAnimator = 0,
        AnimatorPlayable = 1
    }

    #endregion
    
    public class AnimatorView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private RuntimeAnimatorType _runtimeAnimatorType;
        [ShowIf(nameof(_runtimeAnimatorType), RuntimeAnimatorType.AnimatorPlayable)]
        [SerializeField] private AnimationAnimatorPlayableMixer _animationMixer;
        [ShowIf(nameof(_runtimeAnimatorType), RuntimeAnimatorType.RegularAnimator)]
        [SerializeField] private Animator _animator;
        
        [FoldoutGroup("Events")]
        public UnityEvent<string> OnAnimationChange;
        
        private Dictionary<AnimationClip, int> _animations = new();
        private Dictionary<string, int> _animationParams = new();
        private AnimatorOverrideController _overrideController;

        #endregion

        #region Properties

        public Animator Animator
        {
            get => _animator;
            set => _animator = value;
        }

        public AnimationAnimatorPlayableMixer AnimationMixer
        {
            get => _animationMixer;
            set => _animationMixer = value;
        }

        public RuntimeAnimatorType RuntimeAnimatorType => _runtimeAnimatorType;

        #endregion
        
        #region Monobehaviour

        private void Awake()
        {
            if(_runtimeAnimatorType == RuntimeAnimatorType.AnimatorPlayable)
                return;
            
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
        }

        #endregion
        
        #region Public

        public void PlayAnimation(AnimationClipConfig animationConfig)
        {
            var clip = animationConfig.GetAnimationClip();
            if(clip == null)
                return;

            if (_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator
                && animationConfig.IsAnimationClipOverride && animationConfig.OverrideAnimationClip != null)
            {
                if (_overrideController[animationConfig.OverrideAnimationClip] != null)
                    _overrideController[animationConfig.OverrideAnimationClip] = clip;
            }
            
            if (_animations.TryGetValue(clip, out var animationHash))
            {
                Play(animationConfig, animationHash);
                OnAnimationChange?.Invoke(animationConfig.GetAnimationClip().name);
                return;
            }
            var newAnimationHash = Animator.StringToHash(animationConfig.AnimationState);
            Play(animationConfig, newAnimationHash);
            _animations.Add(clip, newAnimationHash);
        }

        #region Setters

        public void SetBool(string boolParameter, bool value)
        {
            if(_animationParams.TryGetValue(boolParameter, out var hash))
                SetAnimatorBool(hash, value);
            else
            {
                var newHash = Animator.StringToHash(boolParameter);
                _animationParams.Add(boolParameter, newHash);
                SetAnimatorBool(newHash, value);
            }
        }
        
        public void SetFloat(string floatParameter, float value)
        {
            if(_animationParams.TryGetValue(floatParameter, out var hash))
                SetAnimatorFloat(hash, value);
            else
            {
                var newHash = Animator.StringToHash(floatParameter);
                _animationParams.Add(floatParameter, newHash);
                SetAnimatorFloat(newHash, value);
            }
        }
        
        public void SetInt(string intParameter, int value)
        {
            if(_animationParams.TryGetValue(intParameter, out var hash))
                SetAnimatorInt(hash, value);
            else
            {
                var newHash = Animator.StringToHash(intParameter);
                _animationParams.Add(intParameter, newHash);
                SetAnimatorInt(newHash, value);
            }
        }
        
        public void SetTrigger(string triggerParameter)
        {
            if(_animationParams.TryGetValue(triggerParameter, out var hash))
                SetAnimatorTrigger(hash);
            else
            {
                var newHash = Animator.StringToHash(triggerParameter);
                _animationParams.Add(triggerParameter, newHash);
                SetAnimatorTrigger(newHash);
            }
        }

        #endregion

        #region Getters

        public bool GetBool(string boolParameter)
        {
            bool result = false;
            if(_runtimeAnimatorType == RuntimeAnimatorType.AnimatorPlayable && _animationMixer != null)
                result = _animationMixer.AnimatorPlayable.GetBool(boolParameter);
            else if (_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator && Animator != null)
                result = _animator.GetBool(boolParameter);
            return result;
        }
        
        public float GetFloat(string floatParameter)
        {
            float result = 0;
            if(_runtimeAnimatorType == RuntimeAnimatorType.AnimatorPlayable && _animationMixer != null)
                result = _animationMixer.AnimatorPlayable.GetFloat(floatParameter);
            else if (_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator && Animator != null)
                result = _animator.GetFloat(floatParameter);
            return result;
        }

        public int GetInt(string intParameter)
        {
            int result = 0;
            if(_runtimeAnimatorType == RuntimeAnimatorType.AnimatorPlayable && _animationMixer!= null)
                result = _animationMixer.AnimatorPlayable.GetInteger(intParameter);
            else if (_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator && Animator!= null)
                result = _animator.GetInteger(intParameter);
            return result;
        }

        #endregion
        
        public void ChangeAnimatorUpdateMode(AnimatorUpdateMode newUpdateMode)
        {
            if(_animator == null)
                return;
            
            _animator.updateMode = newUpdateMode;
        }

        public void StopAnimator()
        {
            _animator?.StopPlayback();
        }

        #endregion

        #region Private

        private void Play(AnimationClipConfig animationConfig, int animationHash)
        {
            if (_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator)
            {
                if(_animator == null)
                    return;
                
                _animator.speed = animationConfig.AnimationSpeed;
                _animator.CrossFadeInFixedTime(animationHash, animationConfig.CrossFadeTime, animationConfig.Layer);
            }
            else
            {
                if(_animationMixer == null)
                    return;
                
                _animationMixer.AnimatorPlayable.SetSpeed(animationConfig.AnimationSpeed);
                _animationMixer.AnimatorPlayable.CrossFadeInFixedTime(animationHash, animationConfig.CrossFadeTime, animationConfig.Layer);
            }
        }

        private void SetAnimatorBool(int hash, bool value)
        {
            if(_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator)
                _animator?.SetBool(hash, value);
            else
                _animationMixer.AnimatorPlayable.SetBool(hash, value);
        }
        
        private void SetAnimatorFloat(int hash, float value)
        {
            if(_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator)
                _animator?.SetFloat(hash, value);
            else
                _animationMixer.AnimatorPlayable.SetFloat(hash, value);
        }
        
        private void SetAnimatorInt(int hash, int value)
        {
            if(_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator)
                _animator?.SetInteger(hash, value);
            else
                _animationMixer.AnimatorPlayable.SetInteger(hash, value);
        }

        private void SetAnimatorTrigger(int hash)
        {
            if(_runtimeAnimatorType == RuntimeAnimatorType.RegularAnimator)
                _animator?.SetTrigger(hash);
            else
                _animationMixer.AnimatorPlayable.SetTrigger(hash);
        }
        
        #endregion
    }
}
