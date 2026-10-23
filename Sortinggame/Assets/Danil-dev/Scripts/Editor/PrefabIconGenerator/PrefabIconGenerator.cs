using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D_Dev.PrefabIconGenerator
{
    public class PrefabIconGenerator : EditorWindow
    {
        private static readonly GUIContent[] SizeLabels =
        {
            new GUIContent("64"),
            new GUIContent("128"),
            new GUIContent("256"),
            new GUIContent("512"),
            new GUIContent("1024"),
            new GUIContent("2048")
        };

        private static readonly int[] SizeValues = { 64, 128, 256, 512, 1024, 2048 };

        private GameObject _prefab;
        private int _size = 512;
        private float _yaw = 150f;
        private float _pitch = 15f;
        private bool _orthographic;
        private float _fov = 30f;
        private float _padding = 1.05f;
        private bool _transparent = true;
        private Color _background = new Color(0.2f, 0.2f, 0.2f, 1f);
        private Color _ambient = new Color(0.45f, 0.45f, 0.45f, 1f);
        private float _lightIntensity = 1.2f;
        private string _savePath = "Assets/Icons";
        private string _suffix = "_icon";
        private bool _autoPreview = true;

        private Texture2D _preview;
        private Vector2 _scroll;
        private string _lastResult = "";
        private bool _resultOk = true;

        private GUIStyle _titleStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _resultStyle;

        [MenuItem("Tools/D_Dev/Utility/Icon Generator")]
        public static void ShowWindow()
        {
            var w = GetWindow<PrefabIconGenerator>("Icon Generator");
            w.minSize = new Vector2(380, 640);
        }

        [MenuItem("Assets/D_Dev/Generate Icon From Prefab")]
        public static void ShowFromSelection()
        {
            var w = GetWindow<PrefabIconGenerator>("Icon Generator");
            w.minSize = new Vector2(380, 640);
            w._prefab = Selection.activeObject as GameObject;
            w.UpdatePreview();
        }

        [MenuItem("Assets/D_Dev/Generate Icon From Prefab", true)]
        private static bool ShowFromSelectionValidate() =>
            Selection.activeObject is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go);

        private void OnDisable()
        {
            if (_preview != null)
                DestroyImmediate(_preview);
        }

        private void OnGUI()
        {
            InitStyles();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUI.BeginChangeCheck();

            DrawTitle();
            DrawTargetSection();
            DrawCameraSection();
            DrawLookSection();

            bool changed = EditorGUI.EndChangeCheck();

            DrawOutputSection();

            if (changed && _autoPreview)
                UpdatePreview();

            DrawPreview();
            DrawActionSection();
            DrawResult();

            EditorGUILayout.EndScrollView();
        }

        private void DrawTitle()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("🖼  Icon Generator", _titleStyle);
            EditorGUILayout.LabelField("Renders a prefab into a PNG icon",
                EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(10);
            DrawSeparator();
        }

        private void DrawTargetSection()
        {
            EditorGUILayout.LabelField("Target", _sectionStyle);
            EditorGUILayout.Space(2);

            _prefab = (GameObject)EditorGUILayout.ObjectField(
                new GUIContent("Prefab", "Prefab asset to render"),
                _prefab, typeof(GameObject), false);

            if (_prefab == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a prefab from the project or drag it here.",
                    MessageType.Info);
            }
            else
            {
                int count = CountRenderers(_prefab);
                EditorGUILayout.HelpBox(
                    $"Renderers found: {count}",
                    count > 0 ? MessageType.None : MessageType.Warning);
            }

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawCameraSection()
        {
            EditorGUILayout.LabelField("Camera", _sectionStyle);
            EditorGUILayout.Space(2);

            _size = EditorGUILayout.IntPopup(
                new GUIContent("Size", "Icon resolution in pixels"),
                _size,
                SizeLabels,
                SizeValues);

            _yaw = EditorGUILayout.Slider(new GUIContent("Yaw", "Horizontal angle"), _yaw, 0f, 360f);
            _pitch = EditorGUILayout.Slider(new GUIContent("Pitch", "Vertical angle"), _pitch, -89f, 89f);

            _orthographic = EditorGUILayout.ToggleLeft(
                new GUIContent("Orthographic", "Removes perspective distortion"),
                _orthographic);

            if (!_orthographic)
                _fov = EditorGUILayout.Slider(new GUIContent("FOV"), _fov, 5f, 90f);

            _padding = EditorGUILayout.Slider(
                new GUIContent("Padding", "Extra space around the object"),
                _padding, 1f, 1.6f);

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawLookSection()
        {
            EditorGUILayout.LabelField("Look", _sectionStyle);
            EditorGUILayout.Space(2);

            _transparent = EditorGUILayout.ToggleLeft(
                new GUIContent("Transparent background", "Saves PNG with alpha channel"),
                _transparent);

            if (!_transparent)
                _background = EditorGUILayout.ColorField(new GUIContent("Background"), _background);

            _ambient = EditorGUILayout.ColorField(new GUIContent("Ambient"), _ambient);
            _lightIntensity = EditorGUILayout.Slider(
                new GUIContent("Light intensity"), _lightIntensity, 0f, 3f);

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawOutputSection()
        {
            EditorGUILayout.LabelField("Save Icon", _sectionStyle);
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            _savePath = EditorGUILayout.TextField(
                new GUIContent("Folder", "Icon is saved as a .png file"), _savePath);
            if (GUILayout.Button("...", GUILayout.Width(28)))
            {
                string picked = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");
                if (!string.IsNullOrEmpty(picked))
                    _savePath = "Assets" + picked.Substring(Application.dataPath.Length);
            }
            EditorGUILayout.EndHorizontal();

            _suffix = EditorGUILayout.TextField(
                new GUIContent("Name suffix", "File name is Prefab name + suffix"), _suffix);

            _autoPreview = EditorGUILayout.ToggleLeft(
                new GUIContent("Auto preview", "Re-renders preview on every change"),
                _autoPreview);

            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawPreview()
        {
            if (_preview == null) return;

            Rect rect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(true));
            EditorGUI.DrawTextureTransparent(rect, _preview, ScaleMode.ScaleToFit);
            EditorGUILayout.Space(6);
            DrawSeparator();
        }

        private void DrawActionSection()
        {
            EditorGUILayout.Space(4);

            if (GUILayout.Button("↻  Update Preview", GUILayout.Height(24)))
                UpdatePreview();

            EditorGUILayout.Space(2);

            GUI.enabled = _prefab != null && CountRenderers(_prefab) > 0;
            if (GUILayout.Button("▶  Save Icon", GUILayout.Height(36)))
                RunSingle();
            GUI.enabled = true;

            EditorGUILayout.Space(2);

            GUI.enabled = GetSelectedPrefabs().Length > 0;
            if (GUILayout.Button("▶  Save For All Selected Prefabs", GUILayout.Height(28)))
                RunBatch();
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

        private void RunSingle()
        {
            if (_prefab == null) return;

            string path = SaveIcon(_prefab);
            if (path == null)
            {
                SetResult(false, "No renderers found on the prefab.");
                return;
            }

            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
            SetResult(true, $"✓ Done! Saved to {path}");
        }

        private void RunBatch()
        {
            GameObject[] prefabs = GetSelectedPrefabs();
            int saved = 0;

            foreach (var prefab in prefabs)
                if (SaveIcon(prefab) != null)
                    saved++;

            AssetDatabase.Refresh();
            SetResult(saved > 0, $"✓ Done! {saved}/{prefabs.Length} icons saved to {_savePath}");
        }

        private string SaveIcon(GameObject prefab)
        {
            Texture2D tex = Render(prefab);
            if (tex == null) return null;

            if (!Directory.Exists(_savePath))
                Directory.CreateDirectory(_savePath);

            string path = $"{_savePath}/{prefab.name}{_suffix}.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());
            DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }

            return path;
        }

        private void UpdatePreview()
        {
            if (_preview != null)
                DestroyImmediate(_preview);

            _preview = _prefab != null ? Render(_prefab) : null;
            Repaint();
        }

        private Texture2D Render(GameObject prefab)
        {
            var pru = new PreviewRenderUtility();
            GameObject instance = null;

            try
            {
                instance = Instantiate(prefab);
                instance.hideFlags = HideFlags.HideAndDontSave;
                instance.transform.position = Vector3.zero;
                pru.AddSingleGO(instance);

                if (!TryGetBounds(instance, out Bounds bounds)) return null;

                Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
                Vector3 right = rot * Vector3.right;
                Vector3 up = rot * Vector3.up;
                Vector3 fwd = rot * Vector3.forward;

                float tanHalf = Mathf.Tan(_fov * 0.5f * Mathf.Deg2Rad);
                float radius = bounds.extents.magnitude;
                float orthoSize = 0f;
                float dist = 0f;

                for (int i = 0; i < 8; i++)
                {
                    Vector3 rel = Vector3.Scale(bounds.extents, new Vector3(
                        (i & 1) == 0 ? -1f : 1f,
                        (i & 2) == 0 ? -1f : 1f,
                        (i & 4) == 0 ? -1f : 1f));

                    float x = Mathf.Abs(Vector3.Dot(rel, right));
                    float y = Mathf.Abs(Vector3.Dot(rel, up));
                    float z = Vector3.Dot(rel, fwd);

                    orthoSize = Mathf.Max(orthoSize, x, y);
                    dist = Mathf.Max(dist, x / tanHalf - z, y / tanHalf - z);
                }

                Camera cam = pru.camera;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.black;
                cam.orthographic = _orthographic;
                cam.fieldOfView = _fov;

                if (_orthographic)
                {
                    cam.orthographicSize = Mathf.Max(orthoSize * _padding, 0.001f);
                    dist = radius * 2f;
                }
                else
                {
                    dist = Mathf.Max(dist * _padding, radius * 0.5f);
                }

                cam.transform.rotation = rot;
                cam.transform.position = bounds.center - fwd * dist;
                cam.nearClipPlane = 0.01f;
                cam.farClipPlane = dist + radius * 4f;

                pru.ambientColor = _ambient;
                pru.lights[0].intensity = _lightIntensity;
                pru.lights[0].transform.rotation = rot * Quaternion.Euler(35f, 30f, 0f);
                pru.lights[1].intensity = _lightIntensity * 0.4f;
                pru.lights[1].transform.rotation = rot * Quaternion.Euler(-20f, -150f, 0f);

                if (!_transparent)
                    return RenderPass(pru, _background);

                Texture2D onBlack = RenderPass(pru, Color.black);
                Texture2D onWhite = RenderPass(pru, Color.white);
                return ExtractAlpha(onBlack, onWhite);
            }
            finally
            {
                if (instance != null)
                    DestroyImmediate(instance);
                pru.Cleanup();
            }
        }

        private Texture2D RenderPass(PreviewRenderUtility pru, Color background)
        {
            pru.camera.backgroundColor = background;
            pru.BeginStaticPreview(new Rect(0, 0, _size, _size));
            pru.Render(true, false);
            return pru.EndStaticPreview();
        }

        private static Texture2D ExtractAlpha(Texture2D onBlack, Texture2D onWhite)
        {
            Color32[] black = onBlack.GetPixels32();
            Color32[] white = onWhite.GetPixels32();
            var result = new Color32[black.Length];

            for (int i = 0; i < black.Length; i++)
            {
                float diff = ((white[i].r - black[i].r)
                              + (white[i].g - black[i].g)
                              + (white[i].b - black[i].b)) / (3f * 255f);
                float alpha = Mathf.Clamp01(1f - diff);

                if (alpha < 0.003f)
                {
                    result[i] = new Color32(0, 0, 0, 0);
                    continue;
                }

                result[i] = new Color32(
                    (byte)Mathf.Clamp(black[i].r / alpha, 0f, 255f),
                    (byte)Mathf.Clamp(black[i].g / alpha, 0f, 255f),
                    (byte)Mathf.Clamp(black[i].b / alpha, 0f, 255f),
                    (byte)Mathf.RoundToInt(alpha * 255f));
            }

            var tex = new Texture2D(onBlack.width, onBlack.height, TextureFormat.RGBA32, false);
            tex.SetPixels32(result);
            tex.Apply();

            DestroyImmediate(onBlack);
            DestroyImmediate(onWhite);
            return tex;
        }

        private static bool TryGetBounds(GameObject go, out Bounds bounds)
        {
            bounds = default;
            bool found = false;

            foreach (var r in go.GetComponentsInChildren<Renderer>())
            {
                if (r is ParticleSystemRenderer) continue;

                if (!found)
                {
                    bounds = r.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(r.bounds);
                }
            }

            return found;
        }

        private GameObject[] GetSelectedPrefabs() =>
            Selection.GetFiltered<GameObject>(SelectionMode.Assets)
                .Where(go => PrefabUtility.IsPartOfPrefabAsset(go))
                .ToArray();

        private int CountRenderers(GameObject root) =>
            root.GetComponentsInChildren<Renderer>()
                .Count(r => !(r is ParticleSystemRenderer));

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