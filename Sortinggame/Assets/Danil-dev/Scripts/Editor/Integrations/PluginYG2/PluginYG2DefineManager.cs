using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace D_Dev.PluginYG2.Editor
{
    [InitializeOnLoad]
    public static class PluginYG2DefineManager
    {
        #region Fields

        public const string Define = "D_DEV_YG2";

        private const string PluginFolderName = "PluginYourGames";
        private const string MarkerScriptName = "YG2";

        private static readonly NamedBuildTarget[] _targets =
        {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.WebGL,
            NamedBuildTarget.Android,
            NamedBuildTarget.iOS
        };

        #endregion

        #region Constructors

        static PluginYG2DefineManager() => EditorApplication.delayCall += Refresh;

        #endregion

        #region Public

        [MenuItem("Tools/D_Dev/PluginYG2/Refresh Define")]
        public static void Refresh()
        {
            var isInstalled = IsPluginInstalled();

            foreach (var target in _targets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbols(target)
                    .Split(';')
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .ToList();

                var hasDefine = defines.Contains(Define);
                if (hasDefine == isInstalled)
                    continue;

                if (isInstalled)
                    defines.Add(Define);
                else
                    defines.Remove(Define);

                PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", defines));
                UnityEngine.Debug.Log($"[PluginYG2] {(isInstalled ? "Added" : "Removed")} {Define} for {target.TargetName}");
            }
        }

        #endregion

        #region Private

        private static bool IsPluginInstalled()
        {
            foreach (var guid in AssetDatabase.FindAssets($"{MarkerScriptName} t:MonoScript"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
                if (Path.GetFileNameWithoutExtension(path) == MarkerScriptName && path.Contains(PluginFolderName))
                    return true;
            }

            return false;
        }

        private static bool IsPluginPath(IEnumerable<string> paths) => paths.Any(p => p.Contains(PluginFolderName));

        #endregion

        #region Classes

        private class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (IsPluginPath(importedAssets) || IsPluginPath(deletedAssets) ||
                    IsPluginPath(movedAssets) || IsPluginPath(movedFromAssetPaths))
                    Refresh();
            }
        }

        #endregion
    }
}
