using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using D_Dev.CustomEventManager;
using D_Dev.EntityPool;
using D_Dev.EntitySpawner;
using D_Dev.PolymorphicValueSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.CurrencySystem.Extensions
{
    public class EntityFlyingAnimationHandler : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class FlyingEntityAnimationConfig
        {
            #region Fields

            [FoldoutGroup("General")] 
            [SerializeField] private EntitySpawnSettings _entitySpawnSettings;
            [FoldoutGroup("General")] 
            [SerializeField] private int _maxAmount;
            [FoldoutGroup("General")] 
            [SerializeField] private Vector3 _localScale = new(1, 1, 1);

            [FoldoutGroup("UI Settings")]
            [SerializeField] private Vector2 _fromOffset;
            [FoldoutGroup("UI Settings")]
            [SerializeField] private Vector2 _toOffset;
            [FoldoutGroup("UI Settings")]
            [SerializeReference] private PolymorphicValue<Canvas> _canvas;

            [FoldoutGroup("World Settings")]
            [SerializeField] private Vector3 _fromOffsetWorld;
            [FoldoutGroup("World Settings")]
            [SerializeField] private Vector3 _toOffsetWorld;
            [FoldoutGroup("World Settings")]
            [SerializeField] private float _jumpPower = 2f;
            [FoldoutGroup("World Settings")]
            [SerializeField] private int _numJumps = 1;

            [FoldoutGroup("Animation Settings")]
            [SerializeField] private float _delayBetweenSpawn;
            [FoldoutGroup("Animation Settings")]
            [SerializeField] private float _moveTime;
            [FoldoutGroup("Animation Settings")]
            [SerializeField] private Ease _ease;
            [FoldoutGroup("Animation Settings")]
            [SerializeField] private bool _ignoreTimeScale;

            [FoldoutGroup("Events")] 
            public UnityEvent OnSingleAnimationEnd;
            [FoldoutGroup("Events")] 
            public UnityEvent OnAllAnimationEnd;

            #endregion

            #region Properties

            public EntitySpawnSettings EntitySpawnSettings => _entitySpawnSettings;

            public PolymorphicValue<Canvas> Canvas => _canvas;

            public float MoveTime => _moveTime;

            public Ease Ease => _ease;

            public bool IgnoreTimeScale => _ignoreTimeScale;

            public int MaxAmount => _maxAmount;

            public Vector3 LocalScale
            {
                get => _localScale;
                set => _localScale = value;
            }

            public Vector2 FromOffset => _fromOffset;

            public Vector2 ToOffset => _toOffset;

            public Vector3 FromOffsetWorld => _fromOffsetWorld;

            public Vector3 ToOffsetWorld => _toOffsetWorld;

            public float DelayBetweenSpawn => _delayBetweenSpawn;

            public float JumpPower => _jumpPower;

            public int NumJumps => _numJumps;

            #endregion
        }

        #endregion
        
        #region Fields

        [Title("Event Name")] 
        [SerializeReference] private PolymorphicValue<string> _flyingEntityEventName = new StringConstantValue();

        [Title("Animation")] 
        [SerializeField] private FlyingEntityAnimationConfig _flyingEntityAnimationConfig;

        private Camera _camera;
        private CancellationTokenSource _cancellationTokenSource;
        
        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _camera = Camera.main;
        }

        private async void Start()
        {
            await _flyingEntityAnimationConfig.EntitySpawnSettings.Init();
        }

        private void OnEnable()
        {
            EventManager.AddListener<Transform, Transform, int>(_flyingEntityEventName.Value, OnEntityUpdate);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<Transform, Transform, int>(_flyingEntityEventName.Value, OnEntityUpdate);
        }

        private void OnDestroy()
        {
            _flyingEntityAnimationConfig?.EntitySpawnSettings?.DisposePool();
            StopAnimation();
        }


        #endregion

        #region Public

        public void StopAnimation()
        {
            CancelAndDisposeCts();
        }

        #endregion
        
        #region Listeners

        private void OnEntityUpdate(Transform from, Transform to, int amount)
        {
            CancelAndDisposeCts();
            _cancellationTokenSource = new CancellationTokenSource();
            StartFlyingEntitySequence(from, to, amount).Forget();
        }

        #endregion

        #region Private

        private async UniTaskVoid StartFlyingEntitySequence(Transform from, Transform to, int amount)
        {
            
            int createdAmount = Mathf.Min(amount, _flyingEntityAnimationConfig.MaxAmount);
            bool fromIsUI = from is RectTransform;
            bool toIsUI = to is RectTransform;
            bool isWorldMode = !fromIsUI && !toIsUI;

            for (int i = 0; i < createdAmount; i++)
            {
                var entityGo = await _flyingEntityAnimationConfig.EntitySpawnSettings.Get();
                if (entityGo == null)
                    continue;

                if (isWorldMode)
                {
                    Transform entityTransform = entityGo.transform;
                    entityTransform.position = from.position + _flyingEntityAnimationConfig.FromOffsetWorld;
                    entityTransform.localScale = _flyingEntityAnimationConfig.LocalScale;

                    var worldTween = entityTransform.DOJump(
                        to.position + _flyingEntityAnimationConfig.ToOffsetWorld, 
                        _flyingEntityAnimationConfig.JumpPower, 
                        _flyingEntityAnimationConfig.NumJumps, 
                        _flyingEntityAnimationConfig.MoveTime)
                        .SetEase(_flyingEntityAnimationConfig.Ease)
                        .SetUpdate(_flyingEntityAnimationConfig.IgnoreTimeScale)
                        .OnComplete(() =>
                        {
                            _flyingEntityAnimationConfig.OnSingleAnimationEnd?.Invoke();
                                                        
                            if (_flyingEntityAnimationConfig.EntitySpawnSettings.UsePool 
                                && entityGo.TryGetComponent(out PoolableObject poolable))
                                poolable?.Release();
                            else
                                Destroy(entityGo);
                        });
                    
                }
                else
                {
                    var canvasRect = _flyingEntityAnimationConfig.Canvas.Value.GetComponent<RectTransform>();

                    Vector2 fromScreenPos = fromIsUI 
                        ? ((RectTransform)from).position 
                        : _camera.WorldToScreenPoint(from.position);

                    Vector2 toScreenPos = toIsUI 
                        ? ((RectTransform)to).position 
                        : _camera.WorldToScreenPoint(to.position);

                    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, fromScreenPos, null, out Vector2 fromLocalPos))
                        continue;

                    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, toScreenPos, null, out Vector2 toLocalPos))
                        continue;

                    RectTransform entityRect = entityGo.GetComponent<RectTransform>();
                    entityRect.SetParent(canvasRect, false);
                    entityRect.anchoredPosition = fromLocalPos + _flyingEntityAnimationConfig.FromOffset;
                    entityRect.localScale = _flyingEntityAnimationConfig.LocalScale;

                    var uiTween = entityRect.DOAnchorPos(toLocalPos + _flyingEntityAnimationConfig.ToOffset, _flyingEntityAnimationConfig.MoveTime)
                        .SetEase(_flyingEntityAnimationConfig.Ease)
                        .SetUpdate(_flyingEntityAnimationConfig.IgnoreTimeScale)
                        .OnComplete(() =>
                        {
                            _flyingEntityAnimationConfig.OnSingleAnimationEnd?.Invoke();
                            
                            if (_flyingEntityAnimationConfig.EntitySpawnSettings.UsePool 
                                && entityGo.TryGetComponent(out PoolableObject poolable))
                                poolable?.Release();
                            else
                                Destroy(entityGo);
                        });
                }

                await UniTask.Delay((int)(_flyingEntityAnimationConfig.DelayBetweenSpawn * 1000),
                    DelayType.Realtime, cancellationToken: _cancellationTokenSource.Token);
            }
            _flyingEntityAnimationConfig.OnAllAnimationEnd?.Invoke();
        }

        private void CancelAndDisposeCts()                  
        {
            if (_cancellationTokenSource == null)
                return;
            
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException e) {}
            _cancellationTokenSource.Dispose();             
        }     
        
        #endregion
    }
}