using D_Dev.Base;
using TMPro;
using UnityEngine;

namespace _Project.Scripts
{
    public class TimedBonusView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TimedBonusInfo _bonus;
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _timeText;

        private long _lastSeconds = -1;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _lastSeconds = -1;
            Refresh();
        }

        private void Update() => Refresh();

        #endregion

        #region Private

        private void Refresh()
        {
            if (_bonus == null)
                return;

            var isActive = _bonus.IsActive;
            if (_root != null && _root.activeSelf != isActive)
                _root.SetActive(isActive);

            if (!isActive || _timeText == null)
                return;

            var seconds = (long)Mathf.Ceil(_bonus.RemainingTime);
            if (seconds == _lastSeconds)
                return;

            _lastSeconds = seconds;
            _timeText.SetText(TimeFormatter.Format(seconds));
        }

        #endregion
    }
}
