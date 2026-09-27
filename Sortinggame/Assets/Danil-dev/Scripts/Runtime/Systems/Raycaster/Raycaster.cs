using System.Collections.Generic;
using D_Dev.PositionRotationConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Raycaster
{

    [System.Serializable]
    public class Raycaster
    {
        #region Enums

        public enum CastType
        {
            Ray = 0,
            Sphere = 1,
            Box = 2
        }

        #endregion
        
        #region Fields

        [Title("Cast settings")]
        [SerializeField] private CastType _castType;
        [ValidateInput("@this._distance > 0", "Distance must be greater than 0")]
        [SerializeField] private float _distance = 1f;
        [ValidateInput("@this._collidersBuffer > 0", "Colliders buffer must be greater than 0")]
        [SerializeField] private int _collidersBuffer = 10;
        [ShowIf("@this._castType == CastType.Sphere", Animate = false)]
        [ValidateInput("@this._castType != CastType.Sphere || this._radius > 0", "Radius must be greater than 0 for Sphere cast")]
        [SerializeField] private float _radius = 0.5f;
        [ShowIf("@this._castType == CastType.Box", Animate = false)]
        [SerializeField] private Vector3 _halfExtents = Vector3.one * 0.5f;
        [SerializeReference] private BasePositionSettings _origin = new();
        [SerializeReference] private BasePositionSettings _direction = new();
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction;
        [Title("Collider checker")]
        [ValidateInput("@this._colliderChecker != null", "ColliderChecker cannot be null")]
        [SerializeField] private ColliderChecker.ColliderChecker _colliderChecker;
        [Space]
        [Title("Gizmos")]
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private Color _debugColor = Color.green;

        private Ray _ray = new();
        private RaycastHit[] _hits;

        #endregion

        #region Properties

        public BasePositionSettings Origin
        {
            get => _origin;
            set => _origin = value;
        }

        public BasePositionSettings Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public float Distance
        {
            get => _distance;
            set => _distance = value;
        }

        public int CollidersBuffer
        {
            get => _collidersBuffer;
            set => _collidersBuffer = value;
        }

        #endregion

        #region Private

        private int PerformCast(Vector3 origin, Vector3 direction, out int hitCount)
        {
            if (_hits == null)
                _hits = new RaycastHit[_collidersBuffer];

            var layerMask = _colliderChecker.CheckLayer ? _colliderChecker.CheckLayerMask.value : ~0;

            switch (_castType)
            {
                case CastType.Ray:
                    _ray.origin = origin;
                    _ray.direction = direction;
                    hitCount = Physics.RaycastNonAlloc(_ray, _hits, _distance, layerMask, _queryTriggerInteraction);
                    break;

                case CastType.Sphere:
                    hitCount = Physics.SphereCastNonAlloc(origin, _radius, direction, _hits, _distance, layerMask, _queryTriggerInteraction);
                    break;

                case CastType.Box:
                    hitCount = Physics.BoxCastNonAlloc(origin, _halfExtents, direction, _hits, Quaternion.identity, _distance, layerMask, _queryTriggerInteraction);
                    break;

                default:
                    hitCount = 0;
                    break;
            }

            return hitCount;
        }

        #endregion

        #region Public

        public bool IsHit()
        {
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                        return true;
                }
            }
            return false;
        }
        
        public bool IsHit(Vector3 origin, Vector3 direction)
        {
            PerformCast(origin, direction, out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                        return true;
                }
            }
            return false;
        }

        public bool IsHit(Vector3 origin, Vector3 direction, out Collider collider, out float distance)
        {
            PerformCast(origin, direction, out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        collider = _hits[i].collider;
                        distance = _hits[i].distance;
                        return true;
                    }
                }
            }
            collider = null;
            distance = 0f;
            return false;
        }

        public bool IsHit(Vector3 origin, Vector3 direction, out RaycastHit hit)
        {
            PerformCast(origin, direction, out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        hit = _hits[i];
                        return true;
                    }
                }
            }
            hit = default;
            return false;
        }

        public bool IsHit(Vector3 origin, Vector3 direction, out Collider collider)
        {
            PerformCast(origin, direction, out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        collider = _hits[i].collider;
                        return true;
                    }
                }
            }
            collider = null;
            return false;
        }

        public bool IsHit(out Collider collider, out float distance)
        {
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        collider = _hits[i].collider;
                        distance = _hits[i].distance;
                        return true;
                    }
                }
            }
            collider = null;
            distance = 0f;
            return false;
        }

        public bool IsHit(out RaycastHit hit)
        {
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        hit = _hits[i];
                        return true;
                    }
                }
            }
            hit = default;
            return false;
        }

        public bool IsHit(out Collider collider)
        {
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        collider = _hits[i].collider;
                        return true;
                    }
                }
            }
            collider = null;
            return false;
        }

        public void GetAllValidColliders(out List<Collider> colliders)
        {
            colliders = new();
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        colliders.Add(_hits[i].collider);
                    }
                }
            }
        }

        public bool GetFirstHit(out RaycastHit hit)
        {
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if(_hits[i].collider == null)
                        continue;

                    if(_colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        hit = _hits[i];
                        return true;
                    }
                }
            }
            hit = default;
            return false;
        }

        public bool GetClosestHit(out RaycastHit hit)
        {
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                RaycastHit closestHit = new RaycastHit { distance = float.MaxValue };
                bool found = false;

                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider)
                        && _hits[i].distance < closestHit.distance)
                    {
                        closestHit = _hits[i];
                        found = true;
                    }
                }

                if (found)
                {
                    hit = closestHit;
                    return true;
                }
            }
            hit = default;
            return false;
        }

        public void GetAllHits(out List<RaycastHit> hits, bool sortByDistance = false)
        {
            hits = new();
            PerformCast(_origin.GetPosition(), _direction.GetPosition(), out int hitsAmount);
            if (hitsAmount > 0)
            {
                for (var i = 0; i < hitsAmount; i++)
                {
                    if (_hits[i].collider != null
                        && _colliderChecker.IsColliderPassed(_hits[i].collider))
                    {
                        hits.Add(_hits[i]);
                    }
                }

                if (sortByDistance)
                {
                    hits.Sort((a, b) => a.distance.CompareTo(b.distance));
                }
            }
        }

        #endregion

        #region Gizmos

        public void OnGizmos()
        {
            if(!_drawGizmos)
                return;
            
            Gizmos.color = _debugColor;
            var originPoint = _origin.GetPosition();
            var directionVector = _direction.GetPosition();
            
            switch (_castType)
            {
                case CastType.Ray:
                    Gizmos.DrawRay(originPoint, directionVector * _distance);
                    break;

                case CastType.Sphere:
                    var endPoint = originPoint + directionVector * _distance;
                    Gizmos.DrawWireSphere(endPoint, _radius);
                    Gizmos.DrawLine(originPoint, endPoint);
                    break;

                case CastType.Box:
                    var boxEndPoint = originPoint + directionVector * _distance;
                    Gizmos.DrawWireCube(boxEndPoint, _halfExtents * 2f);
                    Gizmos.DrawLine(originPoint, boxEndPoint);
                    break;
            }
        }

        #endregion
    }
}
