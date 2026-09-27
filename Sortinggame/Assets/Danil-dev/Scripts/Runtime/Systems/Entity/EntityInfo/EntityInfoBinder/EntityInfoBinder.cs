using D_Dev.Entity;
using D_Dev.RuntimeEntityVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.EntityInfoBinder
{
    public class EntityInfoBinder : MonoBehaviour
    {
        #region Fields

        [SerializeField] private EntityInfo _entityInfo;
        [Space]
        [SerializeField] private bool _initRuntimeContainer;
        [ShowIf(nameof(_initRuntimeContainer))]
        [OnValueChanged(nameof(TryDisableLocalRuntimeContainerInit))]
        [SerializeField] private RuntimeEntityVariablesContainer _runtimeEntityVariablesContainer;

        #endregion

        #region Properties

        public EntityInfo Info => _entityInfo;

        #endregion

        #region Monobehaviour

        private void OnValidate()
        {
            TryDisableLocalRuntimeContainerInit();
        }

        private void Awake()
        {
            TryDisableLocalRuntimeContainerInit();
            TryInitRuntimeContainer();
        }

        #endregion

        #region Private

        private void TryInitRuntimeContainer()
        {
            if(!_initRuntimeContainer || _entityInfo == null || _runtimeEntityVariablesContainer == null)
                return;
            
            _runtimeEntityVariablesContainer.Init(_entityInfo.Variables);
        }

        private void TryDisableLocalRuntimeContainerInit()
        {
            if(_runtimeEntityVariablesContainer == null)
                return;

            if (!_runtimeEntityVariablesContainer.InitLocalVariablesOnAwake)
                return;
            
            _runtimeEntityVariablesContainer.InitLocalVariablesOnAwake = false;
            Debug.Log($"[EntityInfoBinder] Local init from RuntimeEntityContainer - {_runtimeEntityVariablesContainer.gameObject.name} DISABLED " +
                      $"due to EntityInfoBinder - {this.gameObject.name} Init");
        }

        #endregion
    }
}