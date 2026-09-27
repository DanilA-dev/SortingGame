using System.Collections.Generic;
using D_Dev.CustomEventManager;
using D_Dev.PolymorphicValueSystem;
using D_Dev.UpdateManagerSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ScreenPointers
{
    public class ScreenPointersHandler : MonoBehaviour, ILateTickable
    {
        #region Classes

        [System.Serializable]
        public class PointerConfig
        {
            #region Fields

            [SerializeReference] public PolymorphicValue<string> PointerName = new StringConstantValue();
            [SerializeReference] public PolymorphicValue<Transform> Target = new TransformConstantValue();
            public ScreenTargetPointer PointerPrefab;

            #endregion
        }

        #endregion
        
        #region Fields

        [Title("Data")] 
        [SerializeReference] private PolymorphicValue<string> _activatePointerEventName = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<string> _dectivatePointerEventName = new StringConstantValue();

        [Space] 
        [SerializeField] private bool _createPointersOnStart;
        [SerializeField] private PointerConfig[] _pointerConfigs;
        [SerializeReference] private PolymorphicValue<Canvas> _pointersCanvas;
        [SerializeReference] private PolymorphicValue<Transform> _origin;
        [Space]
        [Title("Pointers Settings")]
        [SerializeField] private Vector2 _screenOffset;
        [SerializeField] private float _pointerRotateSpeed;
        [SerializeReference] private PolymorphicValue<float> _updateInterval = new FloatConstantValue(){Value = 1};

        private Dictionary<string, ScreenTargetPointer> _activePings;
        private float _timeSinceLastUpdate;

        #endregion

        #region Monobehavior

        private void Awake()
        {
            _activePings = new();
            if(_createPointersOnStart)
                CreatePointers();
        }

        private void OnEnable()
        {
            LateUpdateManager.Add(this);
            
            EventManager.AddListener<string>(_activatePointerEventName.Value, ActivatePointer);
            EventManager.AddListener<string>(_dectivatePointerEventName.Value, DeactivatePointer);
        }
        
        private void OnDisable()
        {
            EventManager.RemoveListener<string>(_activatePointerEventName.Value, ActivatePointer);
            EventManager.RemoveListener<string>(_dectivatePointerEventName.Value, DeactivatePointer);
            LateUpdateManager.Remove(this);
        }

        private void OnDestroy()
        {
            DestroyPointers();
        }

        #endregion

        #region Public
        
        public void CreatePointers()
        {
            if(_pointerConfigs.Length <= 0)
                return;

            foreach (var pointerConfig in _pointerConfigs)
            {
                var newPointer = Instantiate(pointerConfig.PointerPrefab, _pointersCanvas.Value.transform);
                newPointer.Init(pointerConfig.Target.Value, _screenOffset, _pointerRotateSpeed,
                    _pointersCanvas.Value, _origin.Value);
                newPointer.gameObject.SetActive(false);
                _activePings.TryAdd(pointerConfig.PointerName.Value, newPointer);
            }
        }

        public void DeactivatePointer(string pointerName)
        {
            if(_activePings.Count <= 0)
                return;
            
            _activePings.TryGetValue(pointerName, out var pointer);
            {
                pointer?.DeactivatePointer();
            }
        }

        public void DeactivateAll()
        {
            if(_activePings.Count <= 0)
                return;

            foreach (var (pointerName, pointer) in _activePings)
                pointer?.DeactivatePointer();
        }

        #endregion
        
        #region Private

        private void ActivatePointer(string pointerName)
        {
            if(_activePings.Count <= 0)
                return;

            _activePings.TryGetValue(pointerName, out var pointer);
            {
                pointer?.ActivatePointer();
            }
        }

        private void DestroyPointers()
        {
            if(_activePings.Count <= 0)
                return;

            foreach (var screenTargetPointer in _activePings)
            {
                if(screenTargetPointer.Value == null)
                    continue;
                
                Destroy(screenTargetPointer.Value.gameObject);
            }
        }
        

        #endregion

        #region Tickable

        public void LateTick()
        {
            if(_activePings.Count <= 0)
                return;

            foreach (var (pointerName, pointer) in _activePings)
                pointer.SmoothUpdate();

            _timeSinceLastUpdate += Time.deltaTime;
            
            if(_timeSinceLastUpdate >= _updateInterval.Value)
            {
                _timeSinceLastUpdate = 0;
                
                foreach (var (pointerName, pointer) in _activePings)
                    pointer.UpdateTargetPosition();
            }
        }

        #endregion
    }
}
