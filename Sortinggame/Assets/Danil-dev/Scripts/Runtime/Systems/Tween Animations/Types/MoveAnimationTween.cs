#if DOTWEEN
using System;
using System.Collections.Generic;
using D_Dev.PolymorphicValueSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.TweenAnimations.Types
{
    [System.Serializable]
    public class MoveAnimationTween : BaseAnimationTween
    {
        #region Enums

        public enum MoveObjectType
        {
            Vector,
            Y,
            X,
            Z,
            Transform
        }

        #endregion

        #region Fields

        [SerializeField] private MotionType _motionType;
        [SerializeField] private Transform[] _movedObjects;
        [ShowIf(nameof(_motionType), MotionType.None)]
        [SerializeField] private MoveObjectType _moveObjectType;
        [ShowIf(nameof(_motionType), MotionType.None)]
        [SerializeField] private bool _useInitialPositionAsStart;

        [ShowIf("@_motionType == MotionType.None && !_useInitialPositionAsStart && this._moveObjectType == MoveObjectType.Transform")]
        [SerializeReference] private PolymorphicValue<Transform> _moveStart = new TransformConstantValue();
        [ShowIf("@_motionType == MotionType.None && this._moveObjectType == MoveObjectType.Transform")]
        [SerializeReference] private PolymorphicValue<Transform> _moveEnd = new TransformConstantValue();
        [ShowIf("@_motionType == MotionType.None && !_useInitialPositionAsStart && this._moveObjectType != MoveObjectType.Transform")]
        [SerializeField] private Vector3 _positionStart;
        [ShowIf("@_motionType == MotionType.None && this._moveObjectType != MoveObjectType.Transform")]
        [SerializeField] private Vector3 _positionEnd;
        [ShowIf(nameof(_motionType), MotionType.Shake)]
        [SerializeField] private Vector3 _shakeStrength = Vector3.one;
        [ShowIf(nameof(_motionType), MotionType.Shake)]
        [SerializeField] private int _vibratoShake = 10;
        [ShowIf(nameof(_motionType), MotionType.Shake)]
        [SerializeField] private float _randomnessShake = 90f;
        [ShowIf(nameof(_motionType), MotionType.Shake)]
        [SerializeField] private bool _fadeOutShake = true;
        [ShowIf(nameof(_motionType), MotionType.Punch)]
        [SerializeField] private Vector3 _punch = Vector3.one;
        [ShowIf(nameof(_motionType), MotionType.Punch)]
        [SerializeField] private int _vibratoPunch = 10;
        [ShowIf(nameof(_motionType), MotionType.Punch)]
        [SerializeField] private float _elasticityPunch = 1f;

        private Dictionary<Transform, Vector3> _cachedPositions = new();


        #endregion

        #region Properties

        public MoveObjectType MoveType
        {
            get => _moveObjectType;
            set => _moveObjectType = value;
        }

        public MotionType Motion
        {
            get => _motionType;
            set => _motionType = value;
        }

        public Transform[] MovedObjects
        {
            get => _movedObjects;
            set => _movedObjects = value;
        }

        public bool UseInitialPositionAsStart
        {
            get => _useInitialPositionAsStart;
            set => _useInitialPositionAsStart = value;
        }

        public PolymorphicValue<Transform> MoveStart
        {
            get => _moveStart;
            set => _moveStart = value;
        }

        public PolymorphicValue<Transform> MoveEnd
        {
            get => _moveEnd;
            set => _moveEnd = value;
        }

        public Vector3 PositionStart
        {
            get => _positionStart;
            set => _positionStart = value;
        }

        public Vector3 PositionEnd
        {
            get => _positionEnd;
            set => _positionEnd = value;
        }

        public Vector3 ShakeStrength
        {
            get => _shakeStrength;
            set => _shakeStrength = value;
        }

        public int VibratoShake
        {
            get => _vibratoShake;
            set => _vibratoShake = value;
        }

        public float RandomnessShake
        {
            get => _randomnessShake;
            set => _randomnessShake = value;
        }

        public bool FadeOutShake
        {
            get => _fadeOutShake;
            set => _fadeOutShake = value;
        }

        public Vector3 Punch
        {
            get => _punch;
            set => _punch = value;
        }

        public int VibratoPunch
        {
            get => _vibratoPunch;
            set => _vibratoPunch = value;
        }

        public float ElasticityPunch
        {
            get => _elasticityPunch;
            set => _elasticityPunch = value;
        }

        #endregion

        #region Override

        public override Tween Play()
        {
            if (_movedObjects == null || _movedObjects.Length == 0)
                return null;

            Sequence sequence = DOTween.Sequence();
            
            foreach (var movedObject in _movedObjects)
            {
                if (movedObject == null)
                    continue;

                if (!_cachedPositions.ContainsKey(movedObject))
                {
                    RectTransform rect = movedObject.GetComponent<RectTransform>();
                    _cachedPositions[movedObject] = rect ? rect.anchoredPosition : movedObject.position;
                }

                Tween objectTween = null;
                switch (_motionType)
                {
                    case MotionType.None:
                        objectTween = PlayMoveTween(movedObject);
                        break;
                    case MotionType.Shake:
                        {
                            RectTransform rect = movedObject.GetComponent<RectTransform>();
                            if (rect)
                                rect.anchoredPosition = _cachedPositions[movedObject];
                            else
                                movedObject.position = _cachedPositions[movedObject];
                            objectTween = movedObject.DOShakePosition(Duration, _shakeStrength, _vibratoShake, _randomnessShake, _fadeOutShake)
                                .SetEase(_ease)
                                .SetLoops(_loops, _loopType);
                            break;
                        }
                    case MotionType.Punch:
                        {
                            RectTransform rect = movedObject.GetComponent<RectTransform>();
                            if (rect)
                                rect.anchoredPosition = _cachedPositions[movedObject];
                            else
                                movedObject.position = _cachedPositions[movedObject];
                            objectTween = movedObject.DOPunchPosition(_punch, Duration, _vibratoPunch, _elasticityPunch)
                                .SetEase(_ease)
                                .SetLoops(_loops, _loopType);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (objectTween != null)
                    sequence.Join(objectTween);
            }
            
            SetTarget(_movedObjects[0]?.gameObject);
            SetTweenRaw(sequence);
            return Tween;
        }

        #endregion

        #region Private

        private Tween PlayMoveTween(Transform movedObject)
        {
            RectTransform rect = movedObject.GetComponent<RectTransform>();
            Vector3 initialStartPos = rect ? rect.anchoredPosition : movedObject.position;
            switch (_moveObjectType)
            {
                case MoveObjectType.Vector:
                    return VectorWorldTween(movedObject, initialStartPos);
                case MoveObjectType.Transform:
                    return TransformTween(movedObject, initialStartPos);
                case MoveObjectType.X:
                    return XTween(movedObject, initialStartPos);
                case MoveObjectType.Y:
                    return YTween(movedObject, initialStartPos);
                case MoveObjectType.Z:
                    return ZTween(movedObject, initialStartPos);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Tween TransformTween(Transform movedObject, Vector3 initialStartPos)
        {
            RectTransform rect = movedObject.GetComponent<RectTransform>();
            Tween tween = !rect
                ? movedObject.DOMove(_moveEnd.Value.position, Duration)
                    .From(!_useInitialPositionAsStart? _moveStart.Value.position : initialStartPos)
                : rect.DOAnchorPos(_moveEnd.Value.position, Duration)
                    .From(!_useInitialPositionAsStart? _moveStart.Value.position : initialStartPos);
            
            return tween.SetEase(_ease).SetLoops(_loops, _loopType);
        }

        private Tween YTween(Transform movedObject, Vector3 initialStartPos)
        {
            RectTransform rect = movedObject.GetComponent<RectTransform>();
            Tween tween = !rect
                ? movedObject.DOLocalMoveY(_positionEnd.y, Duration)
                    .From(!_useInitialPositionAsStart? _positionStart.y : initialStartPos.y)
                : rect.DOAnchorPosY(_positionEnd.y, Duration)
                    .From(!_useInitialPositionAsStart? new Vector2(rect.anchoredPosition.x, _positionStart.y) 
                        : new Vector2(rect.anchoredPosition.x, initialStartPos.y));
            
            return tween.SetEase(_ease).SetLoops(_loops, _loopType);
        }
        
        private Tween XTween(Transform movedObject, Vector3 initialStartPos)
        {
            RectTransform rect = movedObject.GetComponent<RectTransform>();
            Tween tween = !rect
                ? movedObject.DOLocalMoveX(_positionEnd.x, Duration)
                    .From(!_useInitialPositionAsStart? _positionStart.x : initialStartPos.x)
                : rect.DOAnchorPosX(_positionEnd.x, Duration)
                    .From(!_useInitialPositionAsStart? new Vector2(_positionStart.x, rect.anchoredPosition.y) 
                        : new Vector2(initialStartPos.x, rect.anchoredPosition.y));
            
            return tween.SetEase(_ease).SetLoops(_loops, _loopType);
        }
        
        private Tween ZTween(Transform movedObject, Vector3 initialStartPos)
        {
            RectTransform rect = movedObject.GetComponent<RectTransform>();
            if(rect)
                Debug.LogError($"Trying to move by z axis, by moved object is RectTransform");
            Tween tween = movedObject.DOLocalMoveZ(_positionEnd.z, Duration)
                .From(!_useInitialPositionAsStart? _positionStart.z : initialStartPos.z);
            
            return tween.SetEase(_ease).SetLoops(_loops, _loopType);
        }
        
        private Tween VectorWorldTween(Transform movedObject, Vector3 initialStartPos)
        {
            RectTransform rect = movedObject.GetComponent<RectTransform>();
            Tween tween = !rect? movedObject.DOMove(_positionEnd, Duration)
                .From(!_useInitialPositionAsStart? _positionStart : initialStartPos)
                    : rect.DOAnchorPos(_positionEnd, Duration)
                        .From(!_useInitialPositionAsStart? _positionStart : initialStartPos);
            
            return tween.SetEase(_ease).SetLoops(_loops, _loopType);
        }

        #endregion

        #region Public

        public override void Pause()
        {
            Tween.Pause();
        }

        #endregion
    }
}
#endif
