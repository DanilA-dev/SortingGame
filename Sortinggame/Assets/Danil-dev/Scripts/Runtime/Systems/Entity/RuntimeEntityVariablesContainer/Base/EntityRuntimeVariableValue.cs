using D_Dev.EntityVariable;

namespace D_Dev.RuntimeEntityVariables
{
    [System.Serializable]
    public abstract class EntityRuntimeVariableValue<TVariable, T> : RuntimeVariableValue<TVariable, T>
        where TVariable : EntityVariable<T>
    {
        #region Properties

        public override T Value
        {
            get
            {
                var variable = Variable;
                return variable != null ? variable.Value : DefaultValue;
            }
            set
            {
                var variable = Variable;
                if (variable != null)
                    variable.Value = value;
            }
        }

        #endregion

        #region Overrides

        protected override void SubscribeToVariable(TVariable variable)
        {
            variable.OnVariableChange += RaiseValueChanged;
        }

        protected override void UnsubscribeFromVariable(TVariable variable)
        {
            variable.OnVariableChange -= RaiseValueChanged;
        }

        #endregion
    }
}
