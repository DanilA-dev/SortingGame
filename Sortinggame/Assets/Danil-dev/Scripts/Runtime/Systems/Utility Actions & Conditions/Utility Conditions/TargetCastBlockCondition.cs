using D_Dev.Base;
using D_Dev.PositionRotationConfig;
using D_Dev.Raycaster;
using UnityEngine;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class TargetCastBlockCondition : ICondition
    {
        #region Fields

        [SerializeReference] private BasePositionSettings _from = new();
        [SerializeReference] private BasePositionSettings _to  = new();
        [Space]
        [SerializeField] private Linecaster _linecaster;
        [SerializeField] private bool _inverse;

        #endregion

        #region Public

        public bool IsConditionMet()
        {
            if (!_inverse)
                return _linecaster.IsIntersect(_from.GetPosition(), _to.GetPosition());
            else
                return!_linecaster.IsIntersect(_from.GetPosition(), _to.GetPosition());
        }

        public void Reset() {}

        #endregion
    }
}