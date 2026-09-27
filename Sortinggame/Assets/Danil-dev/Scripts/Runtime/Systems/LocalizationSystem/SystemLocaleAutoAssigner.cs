using System;
using System.Globalization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace D_Dev.LocalizationSystem
{
    public class SystemLocaleAutoAssigner : ILocaleAutoAssigner
    {
        public Locale GetLocale()
        {
            string systemLanguageCode = CultureInfo.CurrentCulture.Name;
            string systemLanguageCodeShort = systemLanguageCode.Split('-')[0];

            foreach (Locale availableLocale in LocalizationSettings.AvailableLocales.Locales)
            {
                string localeCode = availableLocale.Identifier.Code;

                if (localeCode.Equals(systemLanguageCode, StringComparison.OrdinalIgnoreCase) ||
                    localeCode.Equals(systemLanguageCodeShort, StringComparison.OrdinalIgnoreCase))
                {
                    return availableLocale;
                }
            }

            return null;
        }
    }
}