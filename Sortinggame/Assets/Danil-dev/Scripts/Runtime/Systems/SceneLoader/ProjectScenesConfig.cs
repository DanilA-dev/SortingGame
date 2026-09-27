using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace D_Dev.SceneLoader
{
    [CreateAssetMenu(menuName = "D-Dev/ProjectScenesConfig")]
    public class ProjectScenesConfig : ScriptableObject
    {
        #region Fields

        [SerializeField] private string _sceneInfosPath;
        [InlineEditor]
        [SerializeField] private List<SceneInfo> _scenes = new();

        #endregion

        #region Properties

        public List<SceneInfo> Scenes => _scenes;

        #endregion

#if UNITY_EDITOR
        #region Editor

        [Button]
        private void UpdateProjectScenes()
        {
            _scenes.Clear();
            
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            if (sceneGuids.Length == 0) return;
            var currentBuildScenes = EditorBuildSettings.scenes.ToList();

            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                if(!scenePath.StartsWith("Assets/"))
                    continue;
                
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (sceneAsset == null)
                    continue;
                
                bool inBuild = currentBuildScenes.Any(bs => bs.path == scenePath);
                if (!inBuild)
                    currentBuildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                
                if (!string.IsNullOrEmpty(_sceneInfosPath))
                {
                    string infoPath = _sceneInfosPath + "/" + sceneAsset.name + ".asset";
                    SceneInfo sceneInfo = AssetDatabase.LoadAssetAtPath<SceneInfo>(infoPath);
                    if (sceneInfo == null)
                    {
                        sceneInfo = CreateInstance<SceneInfo>();
                        AssetDatabase.CreateAsset(sceneInfo, infoPath);
                        sceneInfo.ApplySceneAsset(sceneAsset);
                        EditorUtility.SetDirty(sceneInfo);
                    }
                    _scenes.Add(sceneInfo);
                }
            }
            EditorBuildSettings.scenes = currentBuildScenes.ToArray();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        
        [MenuItem("Tools/D_Dev/Setup/Project Scenes Config")]
        private static void OpenScenesConfig()
        {
            ProjectScenesConfig projectScenesConfig = Resources.Load<ProjectScenesConfig>("ProjectScenesConfig");
            var resourcesDirFullPath = Path.Combine(Application.dataPath, "_Project", "Resources");
            var assetRelativePath = "Assets/_Project/Resources/ProjectScenesConfig.asset";

            if (projectScenesConfig == null)
            {
                if (!Directory.Exists(resourcesDirFullPath))
                    Directory.CreateDirectory(resourcesDirFullPath);

                projectScenesConfig = ScriptableObject.CreateInstance<ProjectScenesConfig>();
                AssetDatabase.CreateAsset(projectScenesConfig, assetRelativePath);
                AssetDatabase.Refresh();
            }
            EditorUtility.OpenPropertyEditor(projectScenesConfig);
        }

        private void FindScenes()
        {
            var sceneInfosGuids = AssetDatabase.FindAssets("t:SceneInfo");
            foreach (var guid in sceneInfosGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var sceneInfo = AssetDatabase.LoadAssetAtPath<SceneInfo>(path);
                if (sceneInfo != null && !_scenes.Contains(sceneInfo))
                    _scenes.Add(sceneInfo);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        
        #endregion
#endif
    }
}
