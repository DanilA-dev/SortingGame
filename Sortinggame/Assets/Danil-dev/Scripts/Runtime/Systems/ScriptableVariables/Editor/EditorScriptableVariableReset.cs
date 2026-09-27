#if UNITY_EDITOR
using System;
using System.Linq;
using D_Dev.ScriptableVariables;
using UnityEditor;
using UnityEngine;


namespace D_Dev.ScriptableVariables.Editor
{
    [InitializeOnLoad]
    public class EditorScriptableVariableReset
    {
        static EditorScriptableVariableReset()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                var allVariables = Resources.FindObjectsOfTypeAll<ScriptableObject>()
                    .Where(obj => obj.GetType().IsSubclassOfGeneric(typeof(BaseScriptableVariable<>)));

                foreach (var variable in allVariables)
                {
                    var resetFlag = variable.GetType().GetProperty("ResetOnEnterRuntime")?.GetValue(variable);
                
                    if (resetFlag != null && (bool)resetFlag)
                    {
                        var method = variable.GetType().GetMethod("ResetValue");
                        method?.Invoke(variable, null);
                        Debug.Log($"Reset value for: {variable.name}");
                    }
                }
            }
        }
    }
    
    public static class TypeExtensions
    {
        public static bool IsSubclassOfGeneric(this Type current, Type genericBase)
        {
            do
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == genericBase)
                    return true;
            }
            while ((current = current.BaseType) != null);
            return false;
        }
    }
}
#endif
