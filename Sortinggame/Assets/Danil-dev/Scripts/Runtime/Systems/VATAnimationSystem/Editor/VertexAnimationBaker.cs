using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace D_Dev.VATAnimationSystem
{
    [System.Serializable]
    public class ClipEntry
    {
        public AnimationClip Clip;
        public string Name;
        public int SampleFPS = 20;
    }

    [System.Serializable]
    public class BoneEntry
    {
        public string Name;
        public Transform Bone;
    }

    public class VertexAnimationBaker : EditorWindow
    {
        #region Fields

        private SkinnedMeshRenderer _skinnedMesh;
        private Animator _animator;
        private List<BoneEntry> _bones = new();
        private List<ClipEntry> _clips = new();
        private Vector2 _scroll;
        private string _outputFolder = "Assets/VAT";

        #endregion

        #region Menu

        [MenuItem("Tools/D_Dev/Utility/VAT Baker")]
        public static void Open() => GetWindow<VertexAnimationBaker>("VAT Baker");

        #endregion

        #region GUI

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);

            _skinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                "Skinned Mesh Renderer", _skinnedMesh, typeof(SkinnedMeshRenderer), true);

            _animator = (Animator)EditorGUILayout.ObjectField(
                "Animator", _animator, typeof(Animator), true);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Bone Attachments", EditorStyles.boldLabel);

            for (int i = 0; i < _bones.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _bones[i].Name = EditorGUILayout.TextField(_bones[i].Name, GUILayout.Width(100));
                _bones[i].Bone = (Transform)EditorGUILayout.ObjectField(
                    _bones[i].Bone, typeof(Transform), true);
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    _bones.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Hand R"))
                _bones.Add(new BoneEntry { Name = "hand_r" });
            if (GUILayout.Button("+ Hand L"))
                _bones.Add(new BoneEntry { Name = "hand_l" });
            if (GUILayout.Button("+ Head"))
                _bones.Add(new BoneEntry { Name = "head" });
            if (GUILayout.Button("+ Custom"))
                _bones.Add(new BoneEntry { Name = "bone" });
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);

            for (int i = 0; i < _clips.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _clips[i].Clip = (AnimationClip)EditorGUILayout.ObjectField(
                    _clips[i].Clip, typeof(AnimationClip), false);
                _clips[i].Name = EditorGUILayout.TextField(_clips[i].Name, GUILayout.Width(100));
                _clips[i].SampleFPS = EditorGUILayout.IntField(_clips[i].SampleFPS, GUILayout.Width(40));
                EditorGUILayout.LabelField("fps", GUILayout.Width(24));
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    _clips.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Clip"))
                _clips.Add(new ClipEntry { Name = "clip", SampleFPS = 20 });

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            _outputFolder = EditorGUILayout.TextField("Folder", _outputFolder);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string absolute = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
                if (!string.IsNullOrEmpty(absolute))
                {
                    if (absolute.StartsWith(Application.dataPath))
                        _outputFolder = "Assets" + absolute.Substring(Application.dataPath.Length);
                    else
                        EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the Assets directory.", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            GUI.enabled = CanBake();
            if (GUILayout.Button("Bake", GUILayout.Height(36)))
                Bake();
            GUI.enabled = true;

            if (!CanBake())
                EditorGUILayout.HelpBox("Set Skinned Mesh Renderer, Animator and at least one clip.", MessageType.Warning);

            EditorGUILayout.EndScrollView();
        }

        private bool CanBake() =>
            _skinnedMesh != null && _animator != null && _clips.Count > 0;

        #endregion

        #region Bake

        private void Bake()
        {
            if (!Directory.Exists(_outputFolder))
                Directory.CreateDirectory(_outputFolder);

            string modelName = _skinnedMesh.name;
            var frameRanges = new List<VertexAnimationClipInfo>();
            int totalFrames = 0;

            foreach (var entry in _clips)
            {
                if (entry.Clip == null) continue;
                int frameCount = Mathf.Max(1, Mathf.RoundToInt(entry.Clip.length * entry.SampleFPS));
                frameRanges.Add(new VertexAnimationClipInfo
                {
                    Name = string.IsNullOrEmpty(entry.Name) ? entry.Clip.name : entry.Name,
                    StartFrame = totalFrames,
                    FrameCount = frameCount,
                    FPS = entry.SampleFPS,
                    Duration = entry.Clip.length
                });
                totalFrames += frameCount;
            }

            int vertexCount = _skinnedMesh.sharedMesh.vertexCount;

            var vatTex = new Texture2D(vertexCount, totalFrames, TextureFormat.RGBAHalf, false);
            vatTex.filterMode = FilterMode.Point;
            vatTex.wrapMode = TextureWrapMode.Clamp;

            var validBones = _bones.FindAll(b => b.Bone != null);
            Texture2D boneTex = null;
            if (validBones.Count > 0)
            {
                boneTex = new Texture2D(validBones.Count * 2, totalFrames, TextureFormat.RGBAHalf, false);
                boneTex.filterMode = FilterMode.Bilinear;
                boneTex.wrapMode = TextureWrapMode.Clamp;
            }

            var bakedMesh = new Mesh();
            _skinnedMesh.BakeMesh(bakedMesh);

            var uv1 = new Vector2[vertexCount];
            for (int v = 0; v < vertexCount; v++)
                uv1[v] = new Vector2((v + 0.5f) / vertexCount, 0f);
            bakedMesh.uv2 = uv1;

            var bounds = new Bounds();
            int currentFrame = 0;

            foreach (var entry in _clips)
            {
                if (entry.Clip == null) continue;
                int frameCount = Mathf.RoundToInt(entry.Clip.length * entry.SampleFPS);

                for (int f = 0; f < frameCount; f++)
                {
                    float t = frameCount <= 1 ? 0f : (float)f / (frameCount - 1);
                    SampleAnimation(entry.Clip, t);

                    var tempMesh = new Mesh();
                    _skinnedMesh.BakeMesh(tempMesh);
                    var verts = tempMesh.vertices;
                    DestroyImmediate(tempMesh);

                    foreach (var v in verts)
                        bounds.Encapsulate(v);

                    var pixels = new Color[vertexCount];
                    for (int v = 0; v < vertexCount; v++)
                        pixels[v] = new Color(verts[v].x, verts[v].y, verts[v].z, 1f);

                    vatTex.SetPixels(0, currentFrame, vertexCount, 1, pixels);

                    if (boneTex != null)
                    {
                        for (int b = 0; b < validBones.Count; b++)
                        {
                            var pos = validBones[b].Bone.position;
                            var rot = validBones[b].Bone.rotation;
                            boneTex.SetPixel(b * 2, currentFrame, new Color(pos.x, pos.y, pos.z, 1f));
                            boneTex.SetPixel(b * 2 + 1, currentFrame, new Color(rot.x, rot.y, rot.z, rot.w));
                        }
                    }

                    currentFrame++;
                }
            }

            vatTex.Apply();
            boneTex?.Apply();

            SaveAsset(vatTex, $"{_outputFolder}/{modelName}_VAT.asset");
            if (boneTex != null)
                SaveAsset(boneTex, $"{_outputFolder}/{modelName}_BoneTex.asset");
            SaveAsset(bakedMesh, $"{_outputFolder}/{modelName}_Baked.asset");

            var boneNames = new string[validBones.Count];
            for (int b = 0; b < validBones.Count; b++)
                boneNames[b] = validBones[b].Name;

            var data = ScriptableObject.CreateInstance<VertexAnimationData>();
            data.ModelName = modelName;
            data.TotalFrames = totalFrames;
            data.VertexCount = vertexCount;
            data.BoundsMin = bounds.min;
            data.BoundsSize = bounds.size;
            data.Clips = frameRanges.ToArray();
            data.VATTexture = vatTex;
            data.BoneTexture = boneTex;
            data.BoneNames = boneNames;
            data.BaseMesh = bakedMesh;

            SaveAsset(data, $"{_outputFolder}/{modelName}_VATData.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[VATBaker] Done: {totalFrames} frames, {vertexCount} vertices, {validBones.Count} bones -> {_outputFolder}");
            EditorUtility.DisplayDialog("VAT Baker", $"Done!\n{totalFrames} frames, {vertexCount} vertices, {validBones.Count} bones\n-> {_outputFolder}", "OK");
        }

        private void SampleAnimation(AnimationClip clip, float normalizedTime)
        {
            clip.SampleAnimation(_animator.gameObject, normalizedTime * clip.length);
        }

        private static void SaveAsset(Object asset, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (existing != null)
                EditorUtility.CopySerialized(asset, existing);
            else
                AssetDatabase.CreateAsset(asset, path);
        }

        #endregion
    }
}