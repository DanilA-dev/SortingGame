using System.Linq;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class StringValue : PolymorphicValue<string> { }

    [System.Serializable]
    public sealed class StringConstantValue : ConstantValue<string>
    {
        #region Cloning

        public override PolymorphicValue<string> Clone()
        {
            return new StringConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class StringFromGameObjectName : StringValue
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<GameObject> _gameObject = new GameObjectConstantValue();

        #endregion

        #region Properties

        public override string Value
        {
            get => _gameObject.Value.name;
            set {}
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<string> Clone()
        {
            return new StringFromGameObjectName { Value = Value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class StringScriptableVariableValue : ScriptableVariableValue<StringScriptableVariable,string>
    {
        #region Cloning

        public override PolymorphicValue<string> Clone()
        {
            return new StringScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class StringConcatValue : StringValue
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<string>[] _values;

        #endregion
        
        #region Properties
        public override string Value
        {
            get => string.Concat(_values.Select(x => x.Value));
            set {}
        }

        public override PolymorphicValue<string> Clone()
        {
            return new StringConcatValue() { Value = Value };
        }

        #endregion
    }
}
