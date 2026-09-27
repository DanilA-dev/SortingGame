using D_Dev.TweenAnimations;

namespace D_Dev.ValueViewProvider
{
    public class StringValueDisplay : BaseValueDisplay<string, StringTweenAnimation>
    {
        #region Overrides

        protected override string FormatValue(string value) => value;
        protected override bool TryParseCurrentValue(out string result)
        {
            result = Text.text;
            return true;
        }

        #endregion
    }
}