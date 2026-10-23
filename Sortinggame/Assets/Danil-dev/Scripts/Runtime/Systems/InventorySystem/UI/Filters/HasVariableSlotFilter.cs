using System;
using D_Dev.EntityVariable;
using D_Dev.Entity.Extensions;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InventorySystem.UI.Filters
{
    [Serializable]
    public class HasVariableSlotFilter : InventorySlotFilter
    {
        #region Fields

        [SerializeField, Required] private StringScriptableVariable _variableId;
        [SerializeField] private bool _invert;

        #endregion

        #region Overrides

        public override bool IsValid(InventoryItemEntityInfo info)
        {
            var hasVariable = info != null && info.GetVariable<BaseEntityVariable>(_variableId) != null;
            return hasVariable != _invert;
        }

        #endregion
    }
}
