using D_Dev.CustomEventManager;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.CurrencySystem.Extensions
{
    public class EntityFlyingAnimationInvoker : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<string> _eventName = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<Transform> _from = new TransformConstantValue();
        [SerializeReference] private PolymorphicValue<Transform> _to = new TransformConstantValue();
        [SerializeReference] private PolymorphicValue<int> _amount = new IntConstantValue();

        #endregion

        #region Public

        [Button]
        public void Invoke()
        {
            EventManager.Invoke(_eventName.Value,_from.Value, _to.Value, _amount.Value);
        }

        #endregion
    }
}