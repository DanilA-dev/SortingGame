using System.Collections.Generic;
using D_Dev.ColliderChecker;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ColliderEvents
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseColliderObservable : MonoBehaviour
    {
        #region Fields

        [Title("Dimension")]
        [SerializeField] protected CollisionDimension _collisionDimension;
        [SerializeField] protected ColliderChecker.ColliderChecker _colliderChecker;
        [Space]
        [SerializeField] protected bool _checkEnter;
        [SerializeField] protected bool _checkExit;
        [Space]
        [ShowIf("@this._collisionDimension == CollisionDimension.Collider3d && this._checkEnter")]
        [SerializeField] protected UnityEvent<Collider> _onEnter;
        [ShowIf("@this._collisionDimension == CollisionDimension.Collider3d && this._checkExit")]
        [SerializeField] protected UnityEvent<Collider> _onExit;
        [ShowIf("@this._collisionDimension == CollisionDimension.Collider2d && this._checkEnter")]
        [SerializeField] protected UnityEvent<Collider2D> _onEnter2D;
        [ShowIf("@this._collisionDimension == CollisionDimension.Collider2d && this._checkExit")]
        [SerializeField] protected UnityEvent<Collider2D> _onExit2D;

        #endregion

        #region Properties

        public CollisionDimension Dimension
        {
            get => _collisionDimension;
            set => _collisionDimension = value;
        }

        public HashSet<Collider> Colliders { get; private set; } = new();
        public HashSet<Collider2D> Colliders2D { get; private set; } = new();

        public UnityEvent<Collider> OnEnter
        {
            get => _onEnter;
            set => _onEnter = value;
        }

        public UnityEvent<Collider> OnExit
        {
            get => _onExit;
            set => _onExit = value;
        }

        public UnityEvent<Collider2D> OnEnter2D
        {
            get => _onEnter2D;
            set => _onEnter2D = value;
        }

        public UnityEvent<Collider2D> OnExit2D
        {
            get => _onExit2D;
            set => _onExit2D = value;
        }

        public ColliderChecker.ColliderChecker CollisionChecker
        {
            get => _colliderChecker;
            set => _colliderChecker = value;
        }

        public bool CheckEnter
        {
            get => _checkEnter;
            set => _checkEnter = value;
        }

        public bool CheckExit
        {
            get => _checkExit;
            set => _checkExit = value;
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _colliderChecker.Init();
            InitColliderEvents();
        }

        #endregion

        #region Abstract

        protected virtual void InitColliderEvents() {}

        #endregion

        #region Public

        public void ForceClearColliders()
        {
            Colliders.Clear();
            Colliders2D.Clear();
        }

        #endregion

    }
}
