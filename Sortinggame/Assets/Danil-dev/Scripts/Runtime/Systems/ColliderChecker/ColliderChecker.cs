using System.Collections.Generic;
using D_Dev.TagSystem;
using D_Dev.TagSystem.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ColliderChecker
{

    #region Enums

    public enum CollisionDimension
    {
        Collider3d = 0,
        Collider2d = 1
    }

    #endregion

    [System.Serializable]
    public class ColliderChecker
    {
        #region Fields

        [Title("Dimension")]
        [SerializeField] private CollisionDimension _collisionDimension;
        [Title("Checks")]
        [SerializeField] private bool _checkLayer;
        [ShowIf(nameof(_checkLayer))]
        [SerializeField] private LayerMask _checkLayerMask;
        [SerializeField] private bool _checkTag;
        [ShowIf(nameof(_checkTag))]
        [SerializeField] private Tag[] _checkTags;
        [Space]
        [Title("Ignore")]
        [SerializeField] private bool _ignoreTrigger;
        [SerializeField] private bool _ignoreColliders;
        [ShowIf(nameof(_ignoreColliders))]
        [ShowIf(nameof(_collisionDimension), CollisionDimension.Collider3d)]
        [SerializeField] private Collider[] _ignoredColliders;
        [ShowIf(nameof(_ignoreColliders))]
        [ShowIf(nameof(_collisionDimension), CollisionDimension.Collider2d)]
        [SerializeField] private Collider2D[] _ignoredColliders2D;
        [SerializeField] private bool _ignoreLayer;
        [ShowIf(nameof(_ignoreLayer))]
        [SerializeField] private LayerMask _ignoreLayerMask;
        [SerializeField] private bool _ignoreTag;
        [ShowIf(nameof(_ignoreTag))]
        [SerializeField] private Tag[] _ignoreTags;
        [Title("Debug")]
        [SerializeField] protected bool _debugColliders;

        private HashSet<Collider> _ignoredCollidersSet;
        private HashSet<Collider2D> _ignoredColliders2DSet;

        private int _checkLayerMaskValue;
        private int _ignoreLayerMaskValue;

        #endregion

        #region Properties

        public bool CheckLayer
        {
            get => _checkLayer;
            set => _checkLayer = value;
        }

        public LayerMask CheckLayerMask
        {
            get => _checkLayerMask;
            set => _checkLayerMask = value;
        }

        public bool CheckTag
        {
            get => _checkTag;
            set => _checkTag = value;
        }

        public Tag[] CheckTags
        {
            get => _checkTags;
            set => _checkTags = value;
        }

        public bool IgnoreTrigger
        {
            get => _ignoreTrigger;
            set => _ignoreTrigger = value;
        }

        public bool IgnoreColliders
        {
            get => _ignoreColliders;
            set => _ignoreColliders = value;
        }

        public Collider[] IgnoredColliders
        {
            get => _ignoredColliders;
            set
            {
                _ignoredColliders = value;
                RebuildIgnoredCollidersSet();
            }
        }

        public Collider2D[] IgnoredColliders2D
        {
            get => _ignoredColliders2D;
            set
            {
                _ignoredColliders2D = value;
                RebuildIgnoredColliders2DSet();
            }
        }

        public CollisionDimension CollisionDimension
        {
            get => _collisionDimension;
            set => _collisionDimension = value;
        }

        public bool IgnoreLayer
        {
            get => _ignoreLayer;
            set => _ignoreLayer = value;
        }

        public LayerMask IgnoreLayerMask
        {
            get => _ignoreLayerMask;
            set => _ignoreLayerMask = value;
        }

        public bool IgnoreTag
        {
            get => _ignoreTag;
            set => _ignoreTag = value;
        }

        public Tag[] IgnoreTags
        {
            get => _ignoreTags;
            set => _ignoreTags = value;
        }

        #endregion

        #region Public

        public void Init()
        {
            _checkLayerMaskValue = _checkLayerMask.value;
            _ignoreLayerMaskValue = _ignoreLayerMask.value;

            RebuildIgnoredCollidersSet();
            RebuildIgnoredColliders2DSet();
        }

        public bool IsColliderPassed(Collider collider)
        {
            if (collider == null || collider.gameObject == null)
                return false;

            var go = collider.gameObject;
            var tagComp = (_checkTag || _ignoreTag) ? go.GetTagComponent() : null;

            bool passed = IsCheckPassed(go, tagComp)
                       && IsIgnorePassed(go, collider.isTrigger, collider, tagComp);

            DebugCollider(collider, passed);
            return passed;
        }

        public bool IsColliderPassed(Collider2D collider2D)
        {
            if (collider2D == null || collider2D.gameObject == null)
                return false;

            var go = collider2D.gameObject;
            var tagComp = (_checkTag || _ignoreTag) ? go.GetTagComponent() : null;

            bool passed = IsCheckPassed(go, tagComp)
                       && IsIgnorePassed(go, collider2D.isTrigger, collider2D, tagComp);

            DebugCollider(collider2D, passed);
            return passed;
        }

        #endregion

        #region Private

        private bool IsCheckPassed(GameObject go, TagComponent tagComp)
        {
            if (_checkLayer && ((_checkLayerMaskValue & (1 << go.layer)) == 0))
                return false;

            if (_checkTag && (tagComp == null || !tagComp.HasAnyTags(_checkTags)))
                return false;

            return true;
        }

        private bool IsIgnorePassedCommon(GameObject go, bool isTrigger, TagComponent tagComp)
        {
            if (_ignoreTrigger && isTrigger)
                return false;

            if (_ignoreLayer && ((_ignoreLayerMaskValue & (1 << go.layer)) != 0))
                return false;

            if (_ignoreTag && tagComp != null && tagComp.HasAnyTags(_ignoreTags))
                return false;

            return true;
        }

        private bool IsIgnorePassed(GameObject go, bool isTrigger, Collider collider, TagComponent tagComp)
        {
            if (!IsIgnorePassedCommon(go, isTrigger, tagComp))
                return false;

            if (_ignoreColliders && _ignoredCollidersSet != null && _ignoredCollidersSet.Contains(collider))
                return false;

            return true;
        }

        private bool IsIgnorePassed(GameObject go, bool isTrigger, Collider2D collider2D, TagComponent tagComp)
        {
            if (!IsIgnorePassedCommon(go, isTrigger, tagComp))
                return false;

            if (_ignoreColliders && _ignoredColliders2DSet != null && _ignoredColliders2DSet.Contains(collider2D))
                return false;

            return true;
        }

        private void RebuildIgnoredCollidersSet()
        {
            if (_ignoredColliders == null || _ignoredColliders.Length == 0)
            {
                _ignoredCollidersSet = null;
                return;
            }

            _ignoredCollidersSet ??= new HashSet<Collider>(_ignoredColliders.Length);
            _ignoredCollidersSet.Clear();

            foreach (var c in _ignoredColliders)
                if (c != null) _ignoredCollidersSet.Add(c);
        }

        private void RebuildIgnoredColliders2DSet()
        {
            if (_ignoredColliders2D == null || _ignoredColliders2D.Length == 0)
            {
                _ignoredColliders2DSet = null;
                return;
            }

            _ignoredColliders2DSet ??= new HashSet<Collider2D>(_ignoredColliders2D.Length);
            _ignoredColliders2DSet.Clear();

            foreach (var c in _ignoredColliders2D)
                if (c != null) _ignoredColliders2DSet.Add(c);
        }

        #endregion

        #region Debug

        private void DebugCollider(Collider collider, bool isPassed)
        {
            if (!_debugColliders) return;

            string color = isPassed ? "green" : "red";
            string result = isPassed ? "passed" : "not passed";
            Debug.Log($"{collider.name}, collider <color={color}>{result}</color>");
        }

        private void DebugCollider(Collider2D collider2D, bool isPassed)
        {
            if (!_debugColliders) return;

            string color = isPassed ? "green" : "red";
            string result = isPassed ? "passed" : "not passed";
            Debug.Log($"{collider2D.name}, collider2D <color={color}>{result}</color>");
        }

        #endregion
    }
}