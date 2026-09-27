using D_Dev.TweenAnimations;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public class IntValueDisplay : BaseValueDisplay<int, IntTweenAnimation>
    {
        #region Fields

        [SerializeField] private string _format = "n0";

        #endregion

        #region Overrides

        protected override string FormatValue(int value) => value.ToString(_format);
        
        protected override bool TryParseCurrentValue(out int result)
        {
            return int.TryParse(Text.text, out result);
        }

        #endregion
    }
}