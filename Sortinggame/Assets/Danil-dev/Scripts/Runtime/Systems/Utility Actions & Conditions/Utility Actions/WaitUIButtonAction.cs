using D_Dev.Base;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.Actions
{
    [System.Serializable]
    public class WaitUIButtonAction : BaseAction
    {
        #region Fields

        [SerializeField] private Button _button;

        private bool _isSubscribed;

        #endregion

        #region IAction

        public override void Execute()
        {
            if (_button == null)
                return;

            if (!_isSubscribed)
            {
                _button.onClick.AddListener(OnButtonClicked);
                _isSubscribed = true;
            }
        }

        #endregion

        #region Listeners

        private void OnButtonClicked()
        {
            IsFinished = true;
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClicked);
            _isSubscribed = false;
        }

        #endregion
    }
}
