using System.Collections.Generic;
using D_Dev.AddressablesExstensions;
using D_Dev.EntityVariable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Entity
{
    [CreateAssetMenu(menuName = "D-Dev/Info/EntityInfo")]
    public class EntityInfo : ScriptableObject
    {
        #region Fields

        [InlineButton(nameof(CreateID)), PropertyOrder(-1)]
        [SerializeField] protected string _id;
        [PropertySpace(10)]
        [SerializeField] protected AddressablesGameObjectLoadData _entityPrefab;
        [SerializeReference] protected List<BaseEntityVariable> _variables = new();

        #endregion

        #region Properties

        public string ID => _id;
        public AddressablesGameObjectLoadData EntityPrefab => _entityPrefab;

        public List<BaseEntityVariable> Variables => _variables;

        #endregion

        #region Private

        private void CreateID() => _id = System.Guid.NewGuid().ToString();

        #endregion
    }
}
