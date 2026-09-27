using Cinemachine;
using System.Collections;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Utility
{
    [System.Serializable]
    public class FOVPreset
    {
        public int Index;
        public float MinFov;
        public float MaxFov;

        public FOVPreset(int index, float minFov, float maxFov)
        {
            Index = index;
            MinFov = minFov;
            MaxFov = maxFov;
        }
    }

    public class CinemachineDynamicFOV : MonoBehaviour
    {
        #region Fields

        [SerializeField] private float _minFovValue;
        [SerializeField] private float _maxFovValue;
        [SerializeReference] private PolymorphicValue<float> _fovTransitionDuration = new FloatConstantValue();
        [SerializeField] private CinemachineVirtualCamera _cm;
        [SerializeField] private FOVPreset[] _presets;

        private Coroutine _fovCoroutine;

        #endregion

        #region Monobehaviour

        private void OnDestroy()
        {
            if(_fovCoroutine != null)
                StopCoroutine(_fovCoroutine);
        }

        #endregion
        
        #region Public

        public void SetFOV(float targetFOV, float duration)
        {
            if (_cm == null)
                return;

            var clampedFOV = Mathf.Clamp(targetFOV, _minFovValue, _maxFovValue);

            if (_fovCoroutine != null)
                StopCoroutine(_fovCoroutine);

            _fovCoroutine = StartCoroutine(FOVTransitionCoroutine(clampedFOV, duration));
        }

        public void SetFOV(float targetFOV)
        {
            SetFOV(targetFOV, _fovTransitionDuration.Value);
        }

        public void SetFOVImmediate(float targetFOV)
        {
            if (_cm == null)
                return;

            if (_fovCoroutine != null)
            {
                StopCoroutine(_fovCoroutine);
                _fovCoroutine = null;
            }

            _cm.m_Lens.FieldOfView = Mathf.Clamp(targetFOV, _minFovValue, _maxFovValue);
        }

        public void SetMinFOV(float duration) => SetFOV(_minFovValue, duration);
        public void SetMinFOV() => SetFOV(_minFovValue);
        public void SetMaxFOV(float duration) => SetFOV(_maxFovValue, duration);
        public void SetMaxFOV() => SetFOV(_maxFovValue);

        public void SetFOVByNormalizedMagnitude(Vector3 value)
        {
            var normalizedVec = value.normalized;
            if(normalizedVec.magnitude == 0)
                SetMinFOV();
            else
                SetMaxFOV();
        }
        
        public void SetFOVByNormalizedMagnitude(Vector2 value)
        {
            var normalizedVec = value.normalized;
            if(normalizedVec.magnitude == 0)
                SetMinFOV();
            else
                SetMaxFOV();
        }

        public void SetPresetByIndex(int presetIndex)
        {
            if (presetIndex < 0 || presetIndex >= _presets.Length)
            {
                Debug.LogWarning($"[CinemachineDynamicFOV] Invalid preset index: {presetIndex}");
                return;
            }

            var preset = _presets[presetIndex];
            SetFOVByPreset(preset);
        }

        public void SetPresetByIndex(int presetIndex, float duration)
        {
            if (presetIndex < 0 || presetIndex >= _presets.Length)
            {
                Debug.LogWarning($"[CinemachineDynamicFOV] Invalid preset index: {presetIndex}");
                return;
            }

            var preset = _presets[presetIndex];
            SetFOVByPreset(preset, duration);
        }

        public FOVPreset GetPreset(int index)
        {
            if (index < 0 || index >= _presets.Length)
                return null;

            return _presets[index];
        }

        public FOVPreset[] GetAllPresets() => _presets;

        public int GetPresetsCount() => _presets.Length;

        private void SetFOVByPreset(FOVPreset preset, float duration = 0f)
        {
            if (preset == null)
                return;

            _minFovValue = preset.MinFov;
            _maxFovValue = preset.MaxFov;

            var middleFov = (preset.MinFov + preset.MaxFov) * 0.5f;

            if (duration > 0f)
                SetFOV(middleFov, duration);
            else
                SetFOV(middleFov);
        }

        #endregion

        #region Coroutine

        private IEnumerator FOVTransitionCoroutine(float targetFOV, float duration)
        {
            if (duration <= 0f)
            {
                _cm.m_Lens.FieldOfView = targetFOV;
                _fovCoroutine = null;
                yield break;
            }

            var startFOV = _cm.m_Lens.FieldOfView;
            var time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime / duration;
                time = Mathf.Clamp01(time);
                _cm.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time);
                yield return null;
            }

            _cm.m_Lens.FieldOfView = targetFOV;
            _fovCoroutine = null;
        }

        #endregion
    }
}