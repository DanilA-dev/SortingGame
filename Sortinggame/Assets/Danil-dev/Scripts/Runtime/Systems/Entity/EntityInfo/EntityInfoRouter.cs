using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Entity
{
    [DisallowMultipleComponent]
    public class EntityInfoRouter : MonoBehaviour
    {
        #region Fields

        [SerializeField, ReadOnly] private EntityInfo _info;

        #endregion

        #region Properties

        public EntityInfo Info => _info;

        #endregion

        #region Public

        public void Bind(EntityInfo info) => _info = info;

        #endregion
    }
}
