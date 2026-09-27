using D_Dev.Base;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class WaitUIButtonCondition : ICondition
    {
        #region Fields

        [SerializeField] private Button _button;
        [SerializeField] private bool _resetOnMet = true;

        private bool _isMet;
        private bool _isSubscribed;

        #endregion

        #region ICondition

        public bool IsConditionMet()
        {
            if (_button == null)
                return false;

            if (!_isSubscribed)
            {
                _button.onClick.AddListener(OnButtonClicked);
                _isSubscribed = true;
            }

            return _isMet;
        }

        public void Reset()
        {
            _isMet = false;
            if (_isSubscribed && _button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
                _isSubscribed = false;
            }
        }

        #endregion

        #region Listeners

        private void OnButtonClicked()
        {
            _isMet = true;
            if (_resetOnMet && _button != null)
                _button.onClick.RemoveListener(OnButtonClicked);
        }

        #endregion
    }
}
