using System;
using System.Globalization;
using D_Dev.TweenAnimations;
using UnityEngine;

namespace D_Dev.ValueViewProvider
{
    public class DoubleValueViewProvider : PolymorphicValueViewProvider<double, DoubleTweenAnimation>
    {
        #region Fields

        [SerializeField] private bool _formatAsTime;

        #endregion

        #region Overrides

        protected override string FormatValue(double value)
        {
            if (!_formatAsTime)
                return base.FormatValue(value);

            var time = TimeSpan.FromSeconds(Math.Max(0, value));

            if (string.IsNullOrEmpty(_format))
                return time.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);

            if (_format.Contains("{0"))
                return string.Format(CultureInfo.InvariantCulture, _format, time);

            return time.ToString(_format, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
