using _Project.Scripts.Core.Skills;
using D_Dev.Entity;
using D_Dev.EntityInfoBinder;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    public abstract class SelectedItemSkill : BaseSkill
    {
        #region Fields

        [Title("Selected Item")]
        [SerializeReference] private PolymorphicValue<GameObject> _selectedItem = new GameObjectConstantValue();

        #endregion

        #region Properties

        protected GameObject SelectedItem => _selectedItem.Value;
        protected EntityInfo SelectedInfo { get; private set; }

        #endregion

        #region Overrides

        protected override bool CanUse()
        {
            SelectedInfo = SelectedItem != null && SelectedItem.TryGetComponent(out EntityInfoBinder binder)
                ? binder.Info
                : null;

            return SelectedInfo != null;
        }

        #endregion
    }
}
