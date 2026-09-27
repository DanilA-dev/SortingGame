namespace D_Dev.ScriptableVariables.Setters
{
    public class IntScriptableVariableSetter : BaseScriptableVariableSetter<int, IntScriptableVariable>
    {
        #region Public
        public void AddValue(int value) => _variable.Value += value;

        public void Subtract(int value) => _variable.Value -= value;
        #endregion
    }
}
