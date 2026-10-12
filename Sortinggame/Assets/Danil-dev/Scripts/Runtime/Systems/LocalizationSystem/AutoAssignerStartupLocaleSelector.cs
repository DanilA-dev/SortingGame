using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace D_Dev.LocalizationSystem
{
    [Serializable]
    public abstract class AutoAssignerStartupLocaleSelector<T> : IStartupLocaleSelector where T : ILocaleAutoAssigner, new()
    {
        private readonly T _assigner = new T();

        public Locale GetStartupLocale(ILocalesProvider availableLocales) => _assigner.GetLocale();
    }
}
