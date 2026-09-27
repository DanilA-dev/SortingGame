using D_Dev.Base;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Actions
{
    [System.Serializable]
    public class WaitUntilGameObjectValueExists : BaseAction
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<GameObject> _value = new GameObjectConstantValue();

        #endregion

        #region Overrides

        public override void Execute()
        {
            IsFinished = _value != null && _value.Value != null;
        }

        #endregion
    }
}