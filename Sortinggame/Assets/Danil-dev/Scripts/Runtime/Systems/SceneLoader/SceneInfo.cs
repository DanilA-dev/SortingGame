using D_Dev.AddressablesExstensions;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace D_Dev.SceneLoader
{
    [CreateAssetMenu(menuName = "D-Dev/Info/SceneInfo")]
    public class SceneInfo : ScriptableObject
    {
        #region Fields

#if UNITY_EDITOR
        [SerializeField, OnValueChanged(nameof(GetSceneName))] private SceneAsset _sceneAsset;
#endif
        [SerializeField, ReadOnly] private string _sceneName;
        [SerializeField] private bool _addSceneOnStartup;
        [SerializeField] private bool _isUnloadable;
        [SerializeField] private AddressablesSceneLoadData _sceneLoadData;

        #endregion

        #region Properties

        public string SceneName => _sceneName;
        public bool IsUnloadable => _isUnloadable;
        public bool AddSceneOnStartup => _addSceneOnStartup;
        public AddressablesSceneLoadData SceneLoadData => _sceneLoadData;

        #endregion

#if UNITY_EDITOR
        #region Editor

        public void ApplySceneAsset(SceneAsset sceneAsset)
        {
            _sceneAsset = sceneAsset;
            GetSceneName();
        }

        [Button("Update Scene Name")]
        private void GetSceneName()
        {
            if (_sceneAsset != null)
            {
                _sceneName = _sceneAsset.name;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        #endregion
#endif
    }
}