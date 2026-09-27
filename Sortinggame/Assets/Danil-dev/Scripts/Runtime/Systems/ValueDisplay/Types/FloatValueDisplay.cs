using D_Dev.TweenAnimations;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public class FloatValueDisplay : BaseValueDisplay<float, FloatTweenAnimation>
    {
        #region Fields

        [SerializeField] private string _format = "f0";
        
        #endregion

        #region Overrides
        protected override string FormatValue(float value) => value.ToString(_format);
        
        protected override bool TryParseCurrentValue(out float result)
        {
            return float.TryParse(Text.text, out result);
        }

        #endregion
    }
}