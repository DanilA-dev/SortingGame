using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;

namespace D_Dev.DebugConsole.Editor
{
    [InitializeOnLoad]
    internal static class IngameDebugConsoleDefineSetter
    {
        #region Fields

        private const string Define = "D_DEV_INGAME_DEBUG_CONSOLE";
        private const string AssemblyName = "IngameDebugConsole.Runtime";

        #endregion

        static IngameDebugConsoleDefineSetter() => Refresh();

        public static void Refresh()
        {
            bool hasAsset = !string.IsNullOrEmpty(
                CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(AssemblyName));

            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            PlayerSettings.GetScriptingDefineSymbols(target, out var defines);
            bool hasDefine = defines.Contains(Define);

            if (hasAsset == hasDefine)
                return;

            defines = hasAsset
                ? defines.Append(Define).ToArray()
                : defines.Where(d => d != Define).ToArray();

            PlayerSettings.SetScriptingDefineSymbols(target, defines);
        }

        // Runs in the old domain even when the removed asset left compile errors behind
        private class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
            {
                if (imported.Length > 0 || deleted.Length > 0)
                    Refresh();
            }
        }
    }
}
