using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace D_Dev.MenuHandler
{
    [CreateAssetMenu(menuName = "D-Dev/Info/Menu/MenuInfosContainer")]
    public class MenuInfosContainer : ScriptableObject
    {
        #region Fields

        [SerializeField] private List<MenuInfo> _projectMenus = new();

        #endregion

        #region Properties

        public List<MenuInfo> ProjectMenus => _projectMenus;

        #endregion
        
#if UNITY_EDITOR
        #region Editor

        [Button]
        private void UpdateMenus()
        {
            AutoPopulateMenus(this);
        }

        public static void AutoPopulateMenus(MenuInfosContainer container)
        {
            if (container == null)
                return;

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(MenuInfo)}");
            
            container._projectMenus.Clear();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MenuInfo menuInfo = AssetDatabase.LoadAssetAtPath<MenuInfo>(path);
                
                if (menuInfo != null && !container._projectMenus.Contains(menuInfo))
                    container._projectMenus.Add(menuInfo);
            }

            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets(); 
        }
        
        private static MenuInfosContainer FindContainer()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(MenuInfosContainer)}");
            
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<MenuInfosContainer>(path);
            }

            Debug.LogError($"MenuInfosContainer cannot be found");
            return null;
        }
        
        public class MenuInfoAssetProcessor : AssetPostprocessor
        {
            public static void OnPostprocessAllAssets(
                string[] importedAssets, 
                string[] deletedAssets, 
                string[] movedAssets, 
                string[] movedFromAssetPaths)
            {
                bool needsUpdate = false;

                foreach (string asset in importedAssets)
                {
                    if (asset.EndsWith(".asset") && AssetDatabase.GetMainAssetTypeAtPath(asset) == typeof(MenuInfo))
                    {
                        needsUpdate = true;
                        break;
                    }
                }

                if (!needsUpdate)
                {
                    foreach (string asset in deletedAssets)
                    {
                        if (asset.EndsWith(".asset")) 
                        {
                            needsUpdate = true;
                            break;
                        }
                    }
                }
                
                if (!needsUpdate)
                {
                    foreach (string asset in movedAssets)
                    {
                        if (asset.EndsWith(".asset") && AssetDatabase.GetMainAssetTypeAtPath(asset) == typeof(MenuInfo))
                        {
                            needsUpdate = true;
                            break;
                        }
                    }
                }

                if (needsUpdate)
                {
                    var container = FindContainer();
                    if (container != null)
                    {
                        AutoPopulateMenus(container);
                    }
                }
            }
        }
        
        #endregion
#endif
    }
}