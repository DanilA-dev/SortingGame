using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.CurrencySystem
{
    public class CurrencyInfoIconAssigner : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currencyInfo;

        [SerializeField] private bool _setSpriteRenderer;
        [ShowIf(nameof(_setSpriteRenderer))]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private bool _setImage;
        [ShowIf(nameof(_setImage))]
        [SerializeField] private Image _image;

        #endregion

        #region Monobehaviour

        private void Start() => AssignIcon();

        #endregion

        #region Public

        [Button]
        public void AssignIcon()
        {
            if (_currencyInfo?.Value == null)
                return;

            var icon = _currencyInfo.Value.CurrencyIcon;

            if (_setSpriteRenderer && _spriteRenderer != null)
                _spriteRenderer.sprite = icon;

            if (_setImage && _image != null)
                _image.sprite = icon;
        }

        #endregion
    }
}
