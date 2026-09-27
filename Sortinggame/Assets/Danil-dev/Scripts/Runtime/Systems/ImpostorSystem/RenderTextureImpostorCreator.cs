#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace D_Dev.ImpostorSystem
{
    public class RenderTextureImpostorCreator : MonoBehaviour
    {
        #region Fields
        
        [Title("Settings")]
        [SerializeField] private Renderer _mainRenderer;
        [SerializeField] private Renderer[] _affectedRenderers;
        [SerializeField] private bool _disableAffectedRenderersOnImpostorCreation = true;
        [SerializeField] private bool _useLodGroup;
        [SerializeField] private Vector2Int _textureSize = new Vector2Int(512, 512);
        [Required,SerializeField, FolderPath] private string _impostorPath;
        [SerializeField] private Shader _defaultImpostorShader;
        [Space]
        [SerializeField] private bool _useOrthographicPerspective;
        [ShowIf(nameof(_useOrthographicPerspective))]
        [GUIColor(nameof(_camGizmoColor))]
        [SerializeField] private float _cameraOrthographicSize;
        [GUIColor(nameof(_camGizmoColor))]
        [SerializeField] private Vector3 _cameraBoundsOffset = new Vector3(0, 0, -10);
        [Space]
        [FoldoutGroup("Preview")]
        [PreviewField( 150)]
        [SerializeField, ReadOnly] private Texture _previewTexture;
        [FoldoutGroup("Preview")]
        [SerializeField] private Mesh _camGizmoMesh;
        [FoldoutGroup("Preview")]
        [SerializeField] private Color _camGizmoColor = Color.green;
        [FoldoutGroup("Preview")]
        [SerializeField] private Vector3 _camGizmoScale = new Vector3(5, 5, 5);

        private Dictionary<GameObject, string> _objectsLayers;

        private readonly string _defaultShaderPath =
            "Assets/JMO Assets/Toony Colors Pro/Shaders/Hybrid 2/TCP2 Hybrid Shader 2.tcp2shader";
        #endregion

        #region Monobehaviour

        private void Reset()
        {
            TryGetComponent(out _mainRenderer);
            _affectedRenderers = gameObject.GetComponentsInChildren<Renderer>();

            if (File.Exists(_defaultShaderPath) && _defaultImpostorShader == null)
            {
                Shader defaultShader = AssetDatabase.LoadAssetAtPath<Shader>(_defaultShaderPath);
                _defaultImpostorShader = defaultShader;
            }
        }

        #endregion
        
        #region Editor

        [Button]
        private void CreateImpostor()
        {
            var path = Path.Combine(_impostorPath, $"{gameObject.name}");
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var fullTexturePath = Path.Combine(path, $"{gameObject.name}_{_textureSize.x}_Impostor.png");
            var fullMaterialPath = Path.Combine(path, $"{gameObject.name}_{_textureSize.x}_impostorMat.mat");
            
            var cam = CreateCam();
            var renderTexture = CameraRenderTexture(cam);
            var texture = CreateTexture(renderTexture);
            
            SaveTextureAndReset(texture, fullTexturePath, cam);
            SetTextureType(fullTexturePath);
            var newMaterial = CreateMaterial(fullTexturePath, fullMaterialPath);
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            var impostorGameObject = CreateImpostorQuad(newMaterial);
            TrySetLODGroup(impostorGameObject);

            if(_objectsLayers.Count > 0)
                foreach (var keyValuePair in _objectsLayers)
                    keyValuePair.Key.layer = LayerMask.NameToLayer(keyValuePair.Value);
            
            _objectsLayers.Clear();
        }

        [Button]
        private void PreviewImpostorTexture()
        {
            var cam = CreateCam();
            var renderTexture = CameraRenderTexture(cam);
            var texture = CreateTexture(renderTexture);
            _previewTexture = texture;
            RenderTexture.active = null;
            cam.targetTexture = null;
            DestroyImmediate(cam.gameObject);
            
            if(_objectsLayers.Count > 0)
                foreach (var keyValuePair in _objectsLayers)
                    keyValuePair.Key.layer = LayerMask.NameToLayer(keyValuePair.Value);
            
            _objectsLayers.Clear();
        }

        #endregion

        #region Private

        private void TrySetLODGroup(GameObject impostorGameObject)
        {
            if(!_useLodGroup)
                return;

            var lodGroup = gameObject.AddComponent<LODGroup>();
            var mainRenderers = gameObject.GetComponentsInChildren<Renderer>();
            var impostorRenderer = impostorGameObject.GetComponent<Renderer>();
            foreach (var renderer in mainRenderers)
                if (renderer.gameObject.name == impostorRenderer.gameObject.name)
                    mainRenderers.ToList().Remove(renderer);
            
            var mainLOD = new LOD(0.5f, mainRenderers);
            var impostorLOD = new LOD(0.25f, new[] { impostorRenderer });
            lodGroup.SetLODs(new[] {mainLOD, impostorLOD});
        }
        
        private GameObject CreateImpostorQuad(Material newMaterial)
        {
            if(_disableAffectedRenderersOnImpostorCreation)
                foreach (var allRenderer in _affectedRenderers)
                    allRenderer.enabled = false;
            
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = $"Impostor(x{_textureSize.x}/{_textureSize.y})";
            quad.transform.SetParent(_mainRenderer.transform);
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localRotation = Quaternion.identity;
            quad.transform.localScale = new Vector3(20, 20, 1);
            var quadCollider = quad.GetComponentInChildren<MeshCollider>();
            var quadRenderer = quad.GetComponent<MeshRenderer>();
            quadRenderer.sharedMaterial = newMaterial;
            DestroyImmediate(quadCollider);
            Selection.activeGameObject = quad;
            SceneView.FrameLastActiveSceneView();
            return quad;
        }

        private Material CreateMaterial(string fullTexturePath, string fullMaterialPath)
        {
            Material newMaterial = new Material(_defaultImpostorShader);
            Texture finalTexture = AssetDatabase.LoadAssetAtPath<Texture>(fullTexturePath);
            _previewTexture = finalTexture;
            newMaterial.SetTexture("_BaseMap", finalTexture);
            newMaterial.SetFloat("_Surface", 1);

            if (newMaterial.HasFloat("_UseAlphaTest") && newMaterial.HasFloat("_Cutoff"))
            {
                newMaterial.SetFloat("_UseAlphaTest", 1);
                newMaterial.SetFloat("_Cutoff", 0.1f);
            }
            AssetDatabase.CreateAsset(newMaterial, fullMaterialPath);
            return newMaterial;
        }

        private void SetTextureType(string fullTexturePath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(fullTexturePath) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }

        private void SaveTextureAndReset(Texture2D texture, string fullTexturePath, Camera cam)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(fullTexturePath, bytes);
            RenderTexture.active = null;
            cam.targetTexture = null;
            DestroyImmediate(cam.gameObject);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private Texture2D CreateTexture(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(_textureSize.x, _textureSize.y, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, _textureSize.x, _textureSize.y), 0, 0);
            texture.Apply();
            return texture;
        }

        private RenderTexture CameraRenderTexture(Camera cam)
        {
            _objectsLayers = new Dictionary<GameObject, string>();
            RenderTexture renderTexture = new RenderTexture(_textureSize.x, _textureSize.y, 0, RenderTextureFormat.ARGB32);
            cam.targetTexture = renderTexture;
            
            var rendererBounds = _mainRenderer.bounds;
            foreach (var r in _affectedRenderers)
            {
                _objectsLayers.Add(r.gameObject, LayerMask.LayerToName(r.gameObject.layer));
                r.gameObject.layer = LayerMask.NameToLayer("RenderImpostor");
            }
            cam.transform.parent = transform;
            cam.transform.position = rendererBounds.center + new Vector3(_cameraBoundsOffset.x, _cameraBoundsOffset.y,-rendererBounds.extents.z + _cameraBoundsOffset.z);
            cam.transform.LookAt(rendererBounds.center);
            cam.Render();
            return renderTexture;
        }

        private Camera CreateCam()
        {
            Camera _cam = new GameObject("ImpostorCamera", typeof(Camera)).GetComponent<Camera>();
            _cam.cullingMask = LayerMask.GetMask("RenderImpostor");
            _cam.clearFlags = CameraClearFlags.Nothing;
            _cam.orthographic = _useOrthographicPerspective;
            _cam.orthographicSize = _cameraOrthographicSize;
            _cam.backgroundColor = Color.black;
            return _cam;
        }

        #endregion
        
        #region Gizmo

        private void OnDrawGizmosSelected()
        {
            if(_mainRenderer == null)
                return;
            
            if(_camGizmoMesh == null)
                return;
            
            var rendererBounds = _mainRenderer.bounds;
            var pos = rendererBounds.center + new Vector3(_cameraBoundsOffset.x, _cameraBoundsOffset.y,
                -rendererBounds.extents.z + _cameraBoundsOffset.z);
            var dir = (_mainRenderer.transform.position - pos).normalized;
            var rot = Quaternion.LookRotation(dir);

            Gizmos.color = _camGizmoColor;
            Gizmos.DrawMesh(_camGizmoMesh, pos, rot, _camGizmoScale);
        }

        #endregion
        
    }
}
#endif
