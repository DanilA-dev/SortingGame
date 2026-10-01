using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using D_Dev.Entity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace D_Dev.EntityInfoGenerator
{
    public class EntityInfoGenerator : EditorWindow
    {
        #region Fields

        [SerializeField] private Object _source;
        [SerializeField] private string _nameContains = "Item_Prefab";
        [SerializeField] private string _excludeName = "Base";
        [SerializeField] private DefaultAsset _outputFolder;
        [SerializeField] private bool _copyVariablesFromTemplate;
        [SerializeField] private EntityInfo _templateEntityInfo;

        private const float LabelWidth = 170f;
        private string _lastResult;

        #endregion

        #region Menu

        [MenuItem("Tools/D_Dev/Utility/Entity/EntityInfo Generator")]
        private static void Open() => GetWindow<EntityInfoGenerator>("EntityInfo Generator");

        #endregion

        #region Unity

        private void OnEnable() => minSize = new Vector2(360, 300);

        private void OnGUI()
        {
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;

            EditorGUILayout.Space(6);
            DrawSourceSection();
            DrawOutputSection();
            DrawTemplateSection();
            DrawGenerateSection();

            EditorGUIUtility.labelWidth = prevLabelWidth;
        }

        #endregion

        #region Draw

        private void DrawSourceSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
                _source = EditorGUILayout.ObjectField(
                    new GUIContent("Prefab / Folder", "Prefab or folder with prefabs"), _source, typeof(Object), false);
                _nameContains = EditorGUILayout.TextField(
                    new GUIContent("Name Contains", "Prefab name filter for folder search. Also removed from the generated EntityInfo name"),
                    _nameContains);
                _excludeName = EditorGUILayout.TextField(
                    new GUIContent("Exclude Name", "Prefabs whose name contains any of these values are skipped in folder search. Comma separated"),
                    _excludeName);

                if (_source != null && !(_source is GameObject) && !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(_source)))
                    EditorGUILayout.HelpBox("Source must be a prefab or a folder", MessageType.Warning);
            }
        }

        private void DrawOutputSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
                _outputFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                    new GUIContent("Output Folder", "If empty, EntityInfo is created next to its prefab"),
                    _outputFolder, typeof(DefaultAsset), false);
            }
        }

        private void DrawTemplateSection()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Template", EditorStyles.boldLabel);
                _copyVariablesFromTemplate = EditorGUILayout.ToggleLeft(
                    new GUIContent("Copy Variables From Template", "Clone template EntityInfo with all its variables"),
                    _copyVariablesFromTemplate);

                if (!_copyVariablesFromTemplate)
                    return;

                EditorGUI.indentLevel++;
                _templateEntityInfo = (EntityInfo)EditorGUILayout.ObjectField(
                    "Template Entity Info", _templateEntityInfo, typeof(EntityInfo), false);
                EditorGUI.indentLevel--;

                if (_templateEntityInfo == null)
                    EditorGUILayout.HelpBox("Template is not set, empty EntityInfo will be created", MessageType.Warning);
            }
        }

        private void DrawGenerateSection()
        {
            EditorGUILayout.Space(6);
            using (new EditorGUI.DisabledScope(_source == null))
            {
                if (GUILayout.Button("Generate", GUILayout.Height(30)))
                    Generate();
            }

            if (!string.IsNullOrEmpty(_lastResult))
                EditorGUILayout.HelpBox(_lastResult, MessageType.Info);
        }

        #endregion

        #region Private

        private void Generate()
        {
            var referenced = CollectReferencedPrefabs();
            int created = 0;
            int skipped = 0;

            foreach (var prefab in CollectPrefabs())
            {
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
                if (referenced.Contains(guid))
                {
                    skipped++;
                    continue;
                }

                CreateInfo(prefab);
                created++;
            }

            AssetDatabase.SaveAssets();
            _lastResult = $"Created: {created}, already have EntityInfo: {skipped}";
            Debug.Log($"[EntityInfoGenerator] {_lastResult}");
        }

        private List<GameObject> CollectPrefabs()
        {
            var result = new List<GameObject>();

            if (_source is GameObject prefab)
            {
                result.Add(prefab);
                return result;
            }

            var sourcePath = AssetDatabase.GetAssetPath(_source);
            if (!AssetDatabase.IsValidFolder(sourcePath))
                return result;

            var excluded = (_excludeName ?? string.Empty)
                .Split(',')
                .Select(e => e.Trim())
                .Where(e => e.Length > 0)
                .ToArray();

            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { sourcePath }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(path);

                if (!string.IsNullOrEmpty(_nameContains) && !name.Contains(_nameContains))
                    continue;
                if (excluded.Any(name.Contains))
                    continue;

                result.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
            }
            return result;
        }

        private static HashSet<string> CollectReferencedPrefabs()
        {
            var referenced = new HashSet<string>();
            foreach (var guid in AssetDatabase.FindAssets($"t:{nameof(EntityInfo)}"))
            {
                var info = AssetDatabase.LoadAssetAtPath<EntityInfo>(AssetDatabase.GUIDToAssetPath(guid));
                var prefabData = info != null ? info.EntityPrefab : null;
                if (prefabData == null)
                    continue;

                if (prefabData.Asset != null)
                    referenced.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefabData.Asset)));
                if (prefabData.AssetReference != null && !string.IsNullOrEmpty(prefabData.AssetReference.AssetGUID))
                    referenced.Add(prefabData.AssetReference.AssetGUID);
            }
            return referenced;
        }

        private void CreateInfo(GameObject prefab)
        {
            var infoName = string.IsNullOrEmpty(_nameContains) ? prefab.name : prefab.name.Replace(_nameContains, "");
            if (string.IsNullOrEmpty(infoName))
                infoName = prefab.name;

            var folder = _outputFolder != null
                ? AssetDatabase.GetAssetPath(_outputFolder)
                : Path.GetDirectoryName(AssetDatabase.GetAssetPath(prefab))?.Replace('\\', '/');
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{infoName}_EntityInfo.asset");

            var info = _copyVariablesFromTemplate && _templateEntityInfo != null
                ? Instantiate(_templateEntityInfo)
                : CreateInstance<EntityInfo>();
            AssetDatabase.CreateAsset(info, path);

            var infoSO = new SerializedObject(info);
            infoSO.FindProperty("_id").stringValue = Guid.NewGuid().ToString();
            infoSO.FindProperty("_entityPrefab._makeAddressable").boolValue = false;
            infoSO.FindProperty("_entityPrefab._asset").objectReferenceValue = prefab;
            infoSO.ApplyModifiedPropertiesWithoutUndo();

            Debug.Log($"[EntityInfoGenerator] {info.name} -> {prefab.name}", info);
        }

        #endregion
    }
}
