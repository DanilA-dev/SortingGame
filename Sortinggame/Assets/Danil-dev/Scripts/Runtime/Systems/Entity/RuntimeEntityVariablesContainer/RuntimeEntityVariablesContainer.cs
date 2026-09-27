using System.Collections.Generic;
using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.RuntimeEntityVariables
{
    public class RuntimeEntityVariablesContainer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _initLocalVariablesOnAwake;
        [SerializeReference] private List<BaseEntityVariable> _variables = new();

        private Dictionary<StringScriptableVariable, BaseEntityVariable> _variableMap = new();
        
        [FoldoutGroup("Events")]
        public UnityEvent OnInitialized;
            
        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        public bool InitLocalVariablesOnAwake
        {
            get => _initLocalVariablesOnAwake;
            set => _initLocalVariablesOnAwake = value;
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_initLocalVariablesOnAwake)
            {
                foreach (var runtimeVariables in _variables)
                    _variableMap.TryAdd(runtimeVariables.VariableID, runtimeVariables);
                
                IsInitialized = true;
                OnInitialized?.Invoke();
            }
        }

        #endregion

        #region Public

        public void Init(List<BaseEntityVariable> variablesFromInfo)
        {
            foreach (var runtimeVariables in _variables)
                _variableMap.TryAdd(runtimeVariables.VariableID, runtimeVariables);

            if (variablesFromInfo != null && variablesFromInfo.Count > 0)
            {
                foreach (var variable in variablesFromInfo)
                {
                    if (variable == null)
                        continue;

                    var clonedVar = variable.Clone();
                    _variables.Add(clonedVar);
                    _variableMap.TryAdd(variable.VariableID, clonedVar);
                }
            }

            IsInitialized = true;
            OnInitialized?.Invoke();
        }

        public T GetVariable<T>(StringScriptableVariable variableID) where T : BaseEntityVariable
        {
            if (_variableMap.TryGetValue(variableID, out var variable))
            {
                return variable as T;
            }
        
            Debug.LogError($"Variable '{variableID}' not found in RuntimeEntityVariablesContainer on GameObject {gameObject.name}");
            return null;
        }

        public bool TryGetValue<T>(StringScriptableVariable variableID, out T value)
            where T : BaseEntityVariable
        {
            var variable = GetVariable<T>(variableID);
            if (variable == null)
            {
                value = null;
                return false;
            }

            var valueRaw = variable.GetValueRaw() as T;
            if(valueRaw == null)
            {
                value = null;
                return false;
            }
            value = valueRaw;
            return true;
        }
        
        public void SetValue<T>(StringScriptableVariable variableID, T value) where T : BaseEntityVariable
        {
            var variable = GetVariable<T>(variableID);
            if (variable != null)
                variable.SetValueRaw(value);
        }
        
        #endregion
    }
}