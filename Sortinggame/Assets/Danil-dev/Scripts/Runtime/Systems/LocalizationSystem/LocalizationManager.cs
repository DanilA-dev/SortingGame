using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace D_Dev.LocalizationSystem
{
    public class LocalizationManager : MonoBehaviour
    {
        #region Fields

        private const string DEFAULT_LOCALE_CODE = "en";

        [SerializeField] private bool _autoAssignLocale;
        [ShowIf(nameof(_autoAssignLocale))]
        [SerializeField] private bool _assingOnStart;
        [ShowIf(nameof(_autoAssignLocale))]
        [SerializeReference] private ILocaleAutoAssigner _localeAutoAssigner;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            if (_assingOnStart && _autoAssignLocale)
                StartCoroutine(InitializeAndAssignLocale());
        }

        #endregion

        #region Public

        public static void ChangeLocale(Locale locale)
        {
            if (locale != null)
                LocalizationSettings.SelectedLocale = locale;
        }
        
        public void TryAssignAutoLocale()
        {
            Locale locale = _localeAutoAssigner?.GetLocale() ?? GetDefaultLocale();
            
            if (locale != null)
                LocalizationSettings.SelectedLocale = locale;
        }

        public static Locale GetDefaultLocale()
        {
            return LocalizationSettings.AvailableLocales.GetLocale(DEFAULT_LOCALE_CODE);
        }

        #endregion


        #region Coroutine

        private IEnumerator InitializeAndAssignLocale()
        {
            yield return LocalizationSettings.InitializationOperation;
            TryAssignAutoLocale();
        }

        #endregion
    }
}