using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D_Dev.MeshCombiner
{
    public class MeshCombiner : EditorWindow
    {
        private GameObject _root;
        private bool _combineByMaterial = true;
        private bool _createNewParent = true;
        private bool _deactivateOriginal = true;
        private bool _makeStatic = true;
        private string _savePath = "Assets/CombinedMeshes";

        private Vector2 _scroll;
        private string _lastResult = "";
        private bool _resultOk = true;

        private GUIStyle _titleStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _resultStyle;

        [MenuItem("Tools/D_Dev/Utility/Mesh Combiner")]
        public static void ShowWindow()
        {
            var w = GetWindow<MeshCombiner>("Mesh Combiner");
            w.minSize = new Vector2(380, 460);
        }

        private void OnGUI()
        {
            InitStyles();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawTitle();
            DrawTargetSection();
            DrawOptionsSection();
            DrawOutputSection();
            DrawActionSection();
            DrawResult();

            EditorGUILayout.EndScrollView();
        }

        private void DrawTitle()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("⚙  Mesh Combiner", _titleStyle);
            EditorGUILayout.LabelField("Combines child meshes into one to reduce draw calls",
                EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(10);
            DrawSeparator();
        }

        private void DrawTargetSection()
        {
            EditorGUILayout.LabelField("Target", _sectionStyle);
            EditorGUILayout.Space(2);

            _root = (GameObject)EditorGUILayout.ObjectField(
                new GUIContent("Root Object", "All child MeshRenderers will be combined"),
                _root, typeof(GameObject), true);

            if (_root == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a Root object from the hierarchy or drag it here.",
                    MessageType.Info);
            }
            else
            {
                int count = CountRenderers(_root);
                EditorGUILayout.HelpBox(
                    $"MeshRenderers found: {count}",
                    count > 0 ? MessageType.None : MessageType.Warning);
            }

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawOptionsSection()
        {
            EditorGUILayout.LabelField("Options", _sectionStyle);
            EditorGUILayout.Space(2);

            _combineByMaterial = EditorGUILayout.ToggleLeft(
                new GUIContent("Combine by material",
                    "Creates a separate mesh per unique material — preserves appearance but produces multiple meshes"),
                _combineByMaterial);

            _createNewParent = EditorGUILayout.ToggleLeft(
                new GUIContent("Create new GameObject",
                    "Places the result in a new object next to the Root"),
                _createNewParent);

            _deactivateOriginal = EditorGUILayout.ToggleLeft(
                new GUIContent("Deactivate original objects",
                    "Originals are hidden but not deleted — can be undone"),
                _deactivateOriginal);

            _makeStatic = EditorGUILayout.ToggleLeft(
                new GUIContent("Mark result as Static",
                    "Allows Unity to use Static Batching on the combined mesh"),
                _makeStatic);

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawOutputSection()
        {
            EditorGUILayout.LabelField("Save Mesh", _sectionStyle);
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            _savePath = EditorGUILayout.TextField(
                new GUIContent("Folder", "Mesh is saved as a .asset file"), _savePath);
            if (GUILayout.Button("...", GUILayout.Width(28)))
            {
                string picked = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");
                if (!string.IsNullOrEmpty(picked))
                    _savePath = "Assets" + picked.Substring(Application.dataPath.Length);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawActionSection()
        {
            EditorGUILayout.Space(4);

            GUI.enabled = _root != null && CountRenderers(_root) > 0;
            if (GUILayout.Button("▶  Combine Meshes", GUILayout.Height(36)))
                RunCombine();
            GUI.enabled = true;

            EditorGUILayout.Space(4);
        }

        private void DrawResult()
        {
            if (string.IsNullOrEmpty(_lastResult)) return;
            _resultStyle.normal.textColor = _resultOk
                ? new Color(0.4f, 0.9f, 0.4f)
                : new Color(1f, 0.4f, 0.4f);
            EditorGUILayout.LabelField(_lastResult, _resultStyle);
        }

        private void RunCombine()
        {
            if (_root == null) return;

            MeshRenderer[] renderers = _root
                .GetComponentsInChildren<MeshRenderer>(false)
                .Where(r => r.GetComponent<MeshFilter>() != null)
                .ToArray();

            if (renderers.Length == 0)
            {
                SetResult(false, "No MeshRenderers with a MeshFilter found.");
                return;
            }

            if (!System.IO.Directory.Exists(_savePath))
                System.IO.Directory.CreateDirectory(_savePath);
            AssetDatabase.Refresh();

            GameObject container = _createNewParent
                ? new GameObject(_root.name + "_Combined")
                : _root;

            if (_createNewParent)
            {
                container.transform.SetParent(_root.transform.parent);
                container.transform.localPosition = _root.transform.localPosition;
                container.transform.localRotation = _root.transform.localRotation;
                container.transform.localScale = _root.transform.localScale;
            }

            int totalBefore = renderers.Length;
            int meshesCreated = 0;

            if (_combineByMaterial)
            {
                var groups = new Dictionary<Material, List<CombineInstance>>();

                foreach (var r in renderers)
                {
                    MeshFilter mf = r.GetComponent<MeshFilter>();
                    if (mf == null || mf.sharedMesh == null) continue;

                    Material[] mats = r.sharedMaterials;
                    int subCount = mf.sharedMesh.subMeshCount;

                    for (int i = 0; i < mats.Length && i < subCount; i++)
                    {
                        Material mat = mats[i];
                        if (!groups.ContainsKey(mat))
                            groups[mat] = new List<CombineInstance>();

                        groups[mat].Add(new CombineInstance
                        {
                            mesh = mf.sharedMesh,
                            subMeshIndex = i,
                            transform = mf.transform.localToWorldMatrix
                        });
                    }
                }

                foreach (var pair in groups)
                {
                    Material mat = pair.Key;
                    List<CombineInstance> insts = pair.Value;
                    string mName = mat != null ? mat.name : "NoMaterial";

                    Mesh combined = BuildMeshFromInstances(insts, mName);
                    if (combined == null) continue;

                    SaveMesh(combined, mName);
                    CreateResultObject(combined, mat, container.transform, mName);
                    meshesCreated++;
                }
            }
            else
            {
                var allInstances = new List<CombineInstance>();
                var allMaterials = new List<Material>();

                foreach (var r in renderers)
                {
                    MeshFilter mf = r.GetComponent<MeshFilter>();
                    if (mf == null || mf.sharedMesh == null) continue;

                    int subCount = mf.sharedMesh.subMeshCount;
                    for (int i = 0; i < subCount; i++)
                    {
                        allInstances.Add(new CombineInstance
                        {
                            mesh = mf.sharedMesh,
                            subMeshIndex = i,
                            transform = mf.transform.localToWorldMatrix
                        });
                        if (i < r.sharedMaterials.Length)
                            allMaterials.Add(r.sharedMaterials[i]);
                    }
                }

                Mesh combined = BuildMeshFromInstances(allInstances, _root.name, mergeSubMeshes: false);
                if (combined != null)
                {
                    SaveMesh(combined, _root.name);
                    CreateResultObjectMultiMat(combined, allMaterials.ToArray(), container.transform, _root.name);
                    meshesCreated++;
                }
            }

            if (_deactivateOriginal)
                foreach (var r in renderers)
                    r.gameObject.SetActive(false);

            if (_makeStatic)
                GameObjectUtility.SetStaticEditorFlags(container, StaticEditorFlags.BatchingStatic);

            Undo.RegisterCreatedObjectUndo(container, "Mesh Combine");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetResult(true, $"✓ Done! {totalBefore} objects → {meshesCreated} meshes. Saved to {_savePath}");
        }

        private Mesh BuildMeshFromInstances(List<CombineInstance> instances, string meshName, bool mergeSubMeshes = true)
        {
            if (instances == null || instances.Count == 0) return null;

            var mesh = new Mesh { name = meshName };
            if (instances.Sum(c => c.mesh.vertexCount) > 65535)
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.CombineMeshes(instances.ToArray(), mergeSubMeshes, true);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();
            return mesh;
        }

        private void CreateResultObject(Mesh mesh, Material mat, Transform parent, string name)
        {
            var go = new GameObject(name + "_Mesh");
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        }

        private void CreateResultObjectMultiMat(Mesh mesh, Material[] mats, Transform parent, string name)
        {
            var go = new GameObject(name + "_Mesh");
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshRenderer>().sharedMaterials = mats;
        }

        private void SaveMesh(Mesh mesh, string name)
        {
            string path = $"{_savePath}/{_root.name}_{name}_Combined.asset";
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(mesh, existing);
                AssetDatabase.SaveAssets();
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
            }
        }

        private int CountRenderers(GameObject root) =>
            root.GetComponentsInChildren<MeshRenderer>(false)
                .Count(r => r.GetComponent<MeshFilter>() != null);

        private void SetResult(bool ok, string msg)
        {
            _resultOk = ok;
            _lastResult = msg;
            Repaint();
        }

        private void DrawSeparator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
            EditorGUILayout.Space(4);
        }

        private void InitStyles()
        {
            if (_titleStyle != null) return;

            _titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };

            _sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11
            };

            _resultStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
        }
    }
}
