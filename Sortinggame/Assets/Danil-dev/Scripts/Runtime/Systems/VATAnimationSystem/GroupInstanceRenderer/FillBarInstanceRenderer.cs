using D_Dev.PolymorphicValueSystem;
using D_Dev.UpdateManagerSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.VATAnimationSystem.GroupInstanceRenderer
{
    public class FillBarInstanceRenderer : MonoBehaviour, ITickable
    {
        #region Fields

        [Title("Disabled Components")] 
        [SerializeField] private Renderer _renderer;
        
        [Title("Base")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Material _material;
        [SerializeField] private bool _hasMaxValue;
        [ShowIf(nameof(_hasMaxValue))]
        [SerializeReference] private PolymorphicValue<float> _maxValue = new FloatConstantValue();
        
        [Title("Settings")]
        [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);
        [SerializeField] private float _lerpSpeed = 5f;

        private GroupInstancesRenderer.FillBarInstance _instance;
        private float _targetFill = 1f;

        #endregion

        #region Monobehaviour

        private void Reset()
        {
            if (TryGetComponent(out _renderer))
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
            
            _instance = GroupInstancesRenderer.Instance.AddFillBarInstance(
                _meshFilter.sharedMesh, _material, transform.localToWorldMatrix);
        }

        private void OnEnable()
        {
            if (_instance == null || GroupInstancesRenderer.Instance == null)
                return;

            GroupInstancesRenderer.Instance.AddExistingFillBarInstance(
                _meshFilter.sharedMesh, _material, _instance);
        }

        private void OnDisable()
        {
            if (GroupInstancesRenderer.Instance != null && _instance != null)
                GroupInstancesRenderer.Instance.RemoveFillBarInstance(
                    _meshFilter.sharedMesh, _material, _instance);
        }

        private void OnDestroy()
        {
            UpdateManager.Remove(this);
            
            if (GroupInstancesRenderer.Instance != null && _instance != null)
                GroupInstancesRenderer.Instance.RemoveFillBarInstance(
                    _meshFilter.sharedMesh, _material, _instance);
        }

        #endregion

        #region Public

        public void UpdateFill(float value)
        {
            _targetFill = Mathf.Clamp01(value);
            if (_instance != null)
                _instance.Fill = _targetFill;
        }

        public void UpdateFillByMax(float value)
        {
            _targetFill = _hasMaxValue
                ? value / _maxValue.Value 
                : Mathf.Clamp01(value);
            
            if (_instance != null)
                _instance.Fill = _targetFill;
        }

        #endregion

        #region ITickable

        public void Tick()
        {
            if (this == null || _instance == null)
                return;
            
            if (_instance.DelayedFill > _targetFill)
            {
                _instance.DelayedFill = Mathf.MoveTowards(
                    _instance.DelayedFill, 
                    _targetFill, 
                    Time.deltaTime * _lerpSpeed
                );
            }
            else
            {
                _instance.DelayedFill = _targetFill;
            }
            
            _instance.Matrix = Matrix4x4.TRS(
                transform.position + _offset, 
                transform.rotation, 
                transform.localScale
            );
        }

        #endregion
    }
}