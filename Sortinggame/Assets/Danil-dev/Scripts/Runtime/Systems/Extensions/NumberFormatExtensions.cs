using System;
using System.Globalization;

namespace D_Dev.Extensions
{
    public static class NumberFormatExtensions
    {
        #region Fields

        private const double THOUSAND = 1000d;
        private const double FLOOR_EPSILON = 1e-9;
        private const int ALPHABET_LENGTH = 26;

        private static readonly string[] Suffixes = { "", "K", "M", "B", "T" };

        #endregion

        #region Public

        public static string ToShortString(this int value, int decimals = 1) => ToShortString((double)value, decimals);

        public static string ToShortString(this long value, int decimals = 1) => ToShortString((double)value, decimals);

        public static string ToShortString(this float value, int decimals = 1) => ToShortString((double)value, decimals);

        public static string ToShortString(this double value, int decimals = 1)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value.ToString(CultureInfo.InvariantCulture);

            decimals = Math.Max(0, decimals);

            var absValue = Math.Abs(value);
            var tier = 0;
            while (absValue >= THOUSAND)
            {
                absValue /= THOUSAND;
                tier++;
            }

            var factor = Math.Pow(10, decimals);
            var truncated = Math.Floor(absValue * factor + FLOOR_EPSILON) / factor;
            var format = decimals > 0 ? "0." + new string('#', decimals) : "0";
            var number = truncated.ToString(format, CultureInfo.InvariantCulture);
            var sign = value < 0 && truncated > 0 ? "-" : string.Empty;

            return sign + number + GetSuffix(tier);
        }

        #endregion

        #region Private

        private static string GetSuffix(int tier)
        {
            if (tier < Suffixes.Length)
                return Suffixes[tier];

            var index = tier - Suffixes.Length;
            var first = (char)('a' + index / ALPHABET_LENGTH);
            var second = (char)('a' + index % ALPHABET_LENGTH);
            return new string(new[] { first, second });
        }

        #endregion
    }
}
