namespace D_Dev.ScriptableVariables.Setters
{
    public class FloatScriptableVariableSetter : BaseScriptableVariableSetter<float, FloatScriptableVariable>
    {
        #region Public
        public void AddValue(float value) => _variable.Value += value;

        public void Subtract(float value) => _variable.Value -= value;

        #endregion
    }
}
