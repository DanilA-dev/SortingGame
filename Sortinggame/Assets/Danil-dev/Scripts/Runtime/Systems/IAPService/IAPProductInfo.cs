using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.IAPService
{
    [CreateAssetMenu(menuName = "D-Dev/IAP/Product Info")]
    public class IAPProductInfo : ScriptableObject
    {
        #region Fields

        [Title("Product")]
        [Tooltip("Product id configured in the store console.")]
        [SerializeField] private string _productId;
        [SerializeField] private IAPProductType _type;
        [Title("Display")]
        [Tooltip("Shown when the store does not provide a localized price, e.g. in the editor.")]
        [SerializeReference] private PolymorphicValue<string> _fallbackPrice = new StringConstantValue();

        #endregion

        #region Properties

        public string ProductId => _productId;
        public IAPProductType Type => _type;
        public PolymorphicValue<string> FallbackPrice => _fallbackPrice;

        #endregion
    }
}
