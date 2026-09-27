using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;

namespace D_Dev.CurrencySystem.Extensions
{
    [System.Serializable]
    public class CurrencyInfoEntityVariable : EntityVariable<CurrencyInfo>
    {
        #region Constructors

        public CurrencyInfoEntityVariable() {}
        public CurrencyInfoEntityVariable(StringScriptableVariable variableId,CurrencyInfo currencyInfo) : base(variableId,currencyInfo) {}

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new CurrencyInfoEntityVariable(_variableID, _value);
        }

        #endregion
    }
}