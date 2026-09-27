using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.Utility
{
    public class ImageFillUpdater : MonoBehaviour
    {
        #region Fields

        [Title("Components")]
        [SerializeField] protected Image _fillImage;

        [FoldoutGroup("Values")] 
        [SerializeReference] protected PolymorphicValue<float> _valueToListen = new FloatConstantValue();
        [FoldoutGroup("Values")] 
        [SerializeReference] protected PolymorphicValue<float> _maxValue = new FloatConstantValue();
        
        #endregion

        #region Monobehaviour

        private void Reset()
        {
            if (_fillImage == null)
                TryGetComponent(out _fillImage);
        }

        private void OnEnable()
        {
            if(_valueToListen != null)
                _valueToListen.OnValueChanged += UpdateFillAmount;
        }

        private void OnDisable()
        {
            if(_valueToListen != null)
                _valueToListen.OnValueChanged -= UpdateFillAmount;
        }

        #endregion
        
        #region Public

        public virtual void UpdateFillAmount(float current)
        {
            if (_fillImage == null || _maxValue == null || _maxValue.Value <= 0)
                return;
            
            _fillImage.fillAmount = current / _maxValue.Value;
        }
        
        public virtual void UpdateFillAmountResult(float progress)
        {
            if (_fillImage == null)
                return;
            
            _fillImage.fillAmount = progress;
        }
        
        public virtual void UpdateFillAmount(float current, float max)
        {
            if (_fillImage == null || max <= 0)
                return;
            
            _fillImage.fillAmount = current / max;
        }

        public void ResetFill()
        {
            if (_fillImage == null)
                return;
            
            _fillImage.fillAmount = 0;
        }

        #endregion
    }
}