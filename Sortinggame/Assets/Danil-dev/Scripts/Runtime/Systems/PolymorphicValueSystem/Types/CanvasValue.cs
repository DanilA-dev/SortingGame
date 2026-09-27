using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class CanvasValue : PolymorphicValue<Canvas> {}
    
    [System.Serializable]
    public sealed class CanvasConstantValue : ConstantValue<Canvas>
    {
        #region Cloning

        public override PolymorphicValue<Canvas> Clone()
        {
            return new CanvasConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class CanvasScriptableVariableValue : ScriptableVariableValue<CanvasScriptableVariable,Canvas>
    {
        #region Cloning

        public override PolymorphicValue<Canvas> Clone()
        {
            return new CanvasScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}