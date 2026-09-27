using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.AnimatorView
{
    [System.Serializable]
    public class AnimationClipConfig
    {
        #region Fields

        [InfoBox("Name should be the same as state in Animator Controller board")] 
        [SerializeField]  private string _animationState;
        [SerializeField] private bool _isRandomizeClip;
        [SerializeField] private bool _isAnimationClipOverride;

        [ShowIf(nameof(_isAnimationClipOverride))] 
        [SerializeField] private AnimationClip _overrideAnimationClip;

        [HideIf(nameof(_isRandomizeClip))] 
        [SerializeField] private AnimationClip _animationClip;

        [ShowIf(nameof(_isRandomizeClip))]
        [SerializeField] private AnimationClip[] _animationClips;

        [SerializeField] private int _layer;
        [SerializeField] private float _crossFadeTime;
        [Range(0, 10)]
        [SerializeField] private float _animationSpeed = 1;

        #endregion

        #region Properties

        public string AnimationState
        {
            get => _animationState;
            set => _animationState = value;
        }

        public bool IsRandomizeClip
        {
            get => _isRandomizeClip;
            set => _isRandomizeClip = value;
        }

        public bool IsAnimationClipOverride
        {
            get => _isAnimationClipOverride;
            set => _isAnimationClipOverride = value;
        }

        public AnimationClip OverrideAnimationClip
        {
            get => _overrideAnimationClip;
            set => _overrideAnimationClip = value;
        }

        public AnimationClip AnimationClip
        {
            get => _animationClip;
            set => _animationClip = value;
        }

        public AnimationClip[] AnimationClips
        {
            get => _animationClips;
            set => _animationClips = value;
        }

        public int Layer
        {
            get => _layer;
            set => _layer = value;
        }

        public float CrossFadeTime
        {
            get => _crossFadeTime;
            set => _crossFadeTime = value;
        }

        public float AnimationSpeed
        {
            get => _animationSpeed;
            set => _animationSpeed = value;
        }

        #endregion

        #region Public

        public AnimationClip GetAnimationClip() => _isRandomizeClip 
            ? _animationClips[Random.Range(0, _animationClips.Length)] 
            : _animationClip;

        #endregion

        #region Editor

#if UNITY_EDITOR

        [Button(ButtonStyle.Box)]
        private void UpdateStateNameByClip(Animator animator)
        {
            if(animator == null)
                return;
            
            var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    if (state.state.motion is AnimationClip clip)
                    {
                        if(_isAnimationClipOverride && clip.name == _overrideAnimationClip.name)
                            _animationState = state.state.name;
                        else if(!_isAnimationClipOverride && clip.name == _animationClip.name)
                        {
                            _animationState = state.state.name;
                        }
                        else
                        {
                            var c = _isAnimationClipOverride ? _overrideAnimationClip : _animationClip;
                            Debug.Log($"<color=yellow> Animation state with clip {c.name} " +
                                      $" doesn't exists in Animator {animator.gameObject.name}</color>");
                        }
                    }
                }

                foreach (var subMachine in layer.stateMachine.stateMachines)
                {
                    foreach (var state in subMachine.stateMachine.states)
                    {
                        if (state.state.motion is AnimationClip clip)
                        {
                            if(_isAnimationClipOverride && clip.name == _overrideAnimationClip.name)
                                _animationState = state.state.name;
                            else if(!_isAnimationClipOverride && clip.name == _animationClip.name)
                            {
                                _animationState = state.state.name;
                            }
                            else
                            {
                                var c = _isAnimationClipOverride ? _overrideAnimationClip : _animationClip;
                                Debug.Log($"<color=yellow> Animation state with clip {c.name} " +
                                          $" doesn't exists in Animator {animator.gameObject.name}</color>");
                            }
                        }
                    }
                }
                    
            }
        }
        
#endif

        #endregion
    }
}
