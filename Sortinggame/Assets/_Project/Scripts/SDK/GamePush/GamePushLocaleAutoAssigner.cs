using System;
using D_Dev.LocalizationSystem;
using GamePush;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace _Project.Scripts.SDK
{
    [Serializable]
    public class GamePushLocaleAutoAssigner : ILocaleAutoAssigner
    {
        public Locale GetLocale()
        {
            if (!GP_Init.isReady)
                return null;

            string languageCode = GP_Language.CurrentISO();
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
