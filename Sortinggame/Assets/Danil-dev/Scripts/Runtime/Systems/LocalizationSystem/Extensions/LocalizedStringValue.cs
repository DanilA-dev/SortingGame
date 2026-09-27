using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace D_Dev.LocalizationSystem.Extensions
{
    [System.Serializable]
    public sealed class LocalizedStringValue : StringValue
    {
        #region Fields

        [SerializeField] private LocalizedString _localizedString = new();

        [NonSerialized] private string _cachedValue;
        [NonSerialized] private bool _isSubscribed;

        #endregion

        #region Properties

        public override string Value
        {
            get
            {
                if (_localizedString == null || _localizedString.IsEmpty)
                    return string.Empty;

                if (_isSubscribed)
                    return _cachedValue ?? string.Empty;

                var operation = _localizedString.GetLocalizedStringAsync();
                if (operation.IsDone)
                {
                    _cachedValue = operation.Result;
                    return _cachedValue ?? string.Empty;
                }

                operation.Completed += OnLoadCompleted;
                return _cachedValue ?? string.Empty;
            }
            set {}
        }

        #endregion

        #region Overrides

        protected override void SubscribeToSource()
        {
            if (_localizedString == null || _localizedString.IsEmpty)
                return;

            _isSubscribed = true;
            _localizedString.StringChanged += OnStringChanged;
        }

        protected override void UnsubscribeFromSource()
        {
            if (!_isSubscribed)
                return;

            _localizedString.StringChanged -= OnStringChanged;
            _isSubscribed = false;
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<string> Clone()
        {
            return new LocalizedStringValue
            {
                _localizedString = new LocalizedString(_localizedString.TableReference, _localizedString.TableEntryReference)
            };
        }

        #endregion

        #region Private

        private void OnLoadCompleted(AsyncOperationHandle<string> operation)
        {
            if (operation.Status != AsyncOperationStatus.Succeeded)
                return;

            _cachedValue = operation.Result;
            RaiseValueChanged(_cachedValue);
        }

        private void OnStringChanged(string value)
        {
            _cachedValue = value;
            RaiseValueChanged(value);
        }

        #endregion
    }
}
