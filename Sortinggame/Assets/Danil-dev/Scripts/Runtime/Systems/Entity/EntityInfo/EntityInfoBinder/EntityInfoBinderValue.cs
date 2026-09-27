using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.EntityInfoBinder
{
    [System.Serializable]
    public class EntityInfoBinderValue : EntityInfoValue
    {
        #region Fields

        [SerializeField] private EntityInfoBinder _binder;

        #endregion

        #region Properties

        public override EntityInfo Value
        {
            get => _binder != null ? _binder.Info : null;
            set { }
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<EntityInfo> Clone()
        {
            return new EntityInfoBinderValue { _binder = _binder };
        }

        #endregion
    }
}
