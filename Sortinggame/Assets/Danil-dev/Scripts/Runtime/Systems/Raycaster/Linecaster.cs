using D_Dev.PositionRotationConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Raycaster
{
    [System.Serializable]
    public class Linecaster
    {
        #region Fields

        [Title("Ray settings")] 
        [SerializeReference] private BasePositionSettings _origin = new();
        [SerializeReference] private BasePositionSettings _direction = new();
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction;
        [Title("Collider checker")]
        [SerializeField] private ColliderChecker.ColliderChecker _colliderChecker;
        [Title("Gizmos")]
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private Color _debugColor = Color.yellow;
        
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

        #endregion

        #region Public

        public bool IsIntersect()
        {
            return Physics.Linecast(_origin.GetPosition(), _direction.GetPosition(), out RaycastHit hit,  _colliderChecker.CheckLayer 
                       ? _colliderChecker.CheckLayerMask 
                       : ~0, _queryTriggerInteraction) 
                   && _colliderChecker.IsColliderPassed(hit.collider);
        }
        
        public bool IsIntersect(Vector3 origin, Vector3 direction)
        {
            return Physics.Linecast(origin, direction, out RaycastHit hit, _colliderChecker.CheckLayer 
                       ? _colliderChecker.CheckLayerMask 
                       : ~0, _queryTriggerInteraction) 
                   && _colliderChecker.IsColliderPassed(hit.collider);
        }

        #endregion
        
        #region Gizmos

        public void OnGizmos()
        {
            if(!_drawGizmos)
                return;
            
            var originPoint = _origin.GetPosition();
            var directionVector = _direction.GetPosition();
            
            Gizmos.color = _debugColor;
            Gizmos.DrawRay(originPoint, directionVector);
        }

        #endregion
    }
}