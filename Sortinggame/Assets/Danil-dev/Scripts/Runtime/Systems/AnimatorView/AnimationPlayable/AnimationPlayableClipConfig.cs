using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    [System.Serializable]
    public class AnimationPlayableClipConfig
    {
        #region Fields

        [SerializeField] private int _layer;
        [SerializeField] private bool _applyRootMotion;
        [SerializeField, Range(0, 1f)] private float _targetWeight;
        [SerializeField] private bool _canPlayIfSameClip;
        [SerializeField] private bool _isRandomizeClip;
        [HideIf(nameof(_isRandomizeClip))] 
        [SerializeField] private AnimationClip _animationClip;
        [ShowIf(nameof(_isRandomizeClip))]
        [SerializeField] private AnimationClip[] _animationClips;
        [SerializeField] private bool _isLooping;
        [SerializeField] private AvatarMask _avatarMask;
        [SerializeField] private bool _isAdditive;
        [SerializeField] private bool _useFootIK;
        [SerializeField] private bool _useAutoFadeTimeBasedOnClipLength;
        [HideIf(nameof(_useAutoFadeTimeBasedOnClipLength))]
        [SerializeField] private float _crossFadeTime;
        [HideIf(nameof(_useAutoFadeTimeBasedOnClipLength))]
        [SerializeField] private float _fadeDelay;
        [Range(0, 10)]
        [SerializeField] private float _animationSpeed = 1;

        #endregion
        
        #region Properties

        public bool IsRandomizeClip
        {
            get => _isRandomizeClip;
            set => _isRandomizeClip = value;
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

        public AvatarMask Mask
        {
            get => _avatarMask;
            set => _avatarMask = value;
        }

        public bool IsLooping
        {
            get => _isLooping;
            set => _isLooping = value;
        }

        public bool IsAdditive
        {
            get => _isAdditive;
            set => _isAdditive = value;
        }

        public float TargetWeight
        {
            get => _targetWeight;
            set => _targetWeight = value;
        }

        public bool UseFootIK
        {
            get => _useFootIK;
            set => _useFootIK = value;
        }

        public float FadeDelay
        {
            get => _fadeDelay;
            set => _fadeDelay = value;
        }

        public bool UseAutoFadeTimeBasedOnClipLength
        {
            get => _useAutoFadeTimeBasedOnClipLength;
            set => _useAutoFadeTimeBasedOnClipLength = value;
        }

        public bool ApplyRootMotion
        {
            get => _applyRootMotion;
            set => _applyRootMotion = value;
        }

        public bool CanPlayIfSameClip
        {
            get => _canPlayIfSameClip;
            set => _canPlayIfSameClip = value;
        }

        #endregion
        
        #region Public

        public AnimationClip GetAnimationClip() => _isRandomizeClip 
            ? _animationClips[Random.Range(0, _animationClips.Length)] 
            : _animationClip;

        #endregion
    }
}
