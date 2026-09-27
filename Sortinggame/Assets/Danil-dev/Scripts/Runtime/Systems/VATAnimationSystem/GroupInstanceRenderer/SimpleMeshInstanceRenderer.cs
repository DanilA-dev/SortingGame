using D_Dev.UpdateManagerSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.VATAnimationSystem.GroupInstanceRenderer
{
    public class SimpleMeshInstanceRenderer : MonoBehaviour, ITickable
    {
        #region Fields

        [Title("Disabled Components")] 
        [SerializeField] private Renderer _renderer;
        [Title("Base")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Material _material;

        private GroupInstancesRenderer.SimpleMeshInstance _instance;

        #endregion

        #region Monobehaviour

        private void Reset()
        {
            if(TryGetComponent(out _renderer))
                _material = _renderer.sharedMaterial;
            
            TryGetComponent(out _meshFilter);
        }

        private void Start()
        {
            UpdateManager.Add(this);

            
            if (GroupInstancesRenderer.Instance == null)
                return;
            
            if (_meshFilter == null || _material == null)
                return;

            _renderer.enabled = false;
            _instance = GroupInstancesRenderer.Instance.AddStaticInstance(
                _meshFilter.sharedMesh, _material, transform.localToWorldMatrix);
        }


        private void OnEnable()
        {
            if (_instance == null || GroupInstancesRenderer.Instance == null)
                return;

            GroupInstancesRenderer.Instance.AddExistingStaticInstance(
                _meshFilter.sharedMesh, _material, _instance);
        }

        private void OnDisable()
        {
            if (GroupInstancesRenderer.Instance != null && _instance != null)
                GroupInstancesRenderer.Instance.RemoveStaticInstance(
                    _meshFilter.sharedMesh, _material, _instance);
        }
        private void OnDestroy()
        {
            UpdateManager.Remove(this);
            
            if (GroupInstancesRenderer.Instance != null && _instance != null)
                GroupInstancesRenderer.Instance.RemoveStaticInstance(
                    _meshFilter.sharedMesh, _material, _instance);
        }

        #endregion

        #region ITickable

        public void Tick()
        {
            if (this == null || _instance == null)
                return;
            
            _instance.Matrix = transform.localToWorldMatrix;
        }

        #endregion
    }
}