using System;
using D_Dev.LocalizationSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YG;

namespace _Project.Scripts.SDK
{
    public class YG2LocaleAutoAssigner : ILocaleAutoAssigner
    {
        public Locale GetLocale()
        {
            string languageCode = YG2.envir.language;
            if (string.IsNullOrEmpty(languageCode))
                return null;

            foreach (Locale availableLocale in LocalizationSettings.AvailableLocales.Locales)
            {
                string localeCode = availableLocale.Identifier.Code;

                if (localeCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase) ||
                    localeCode.Split('-')[0].Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    return availableLocale;
                }
            }

            return null;
        }
    }
}
