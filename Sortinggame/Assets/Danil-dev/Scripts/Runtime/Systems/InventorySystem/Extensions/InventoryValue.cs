using D_Dev.PolymorphicValueSystem;

namespace D_Dev.InventorySystem.Extensions
{
    [System.Serializable]
    public abstract class InventoryValue : PolymorphicValue<Inventory> { }

    [System.Serializable]
    public sealed class InventoryConstantValue : ConstantValue<Inventory>
    {
        #region Cloning

        public override PolymorphicValue<Inventory> Clone()
        {
            return new InventoryConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class InventoryScriptableVariableValue : ScriptableVariableValue<InventoryScriptableVariable, Inventory>
    {
        #region Cloning

        public override PolymorphicValue<Inventory> Clone()
        {
            return new InventoryScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
