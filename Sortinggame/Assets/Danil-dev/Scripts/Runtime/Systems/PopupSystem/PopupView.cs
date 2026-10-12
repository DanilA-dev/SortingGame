#if DOTWEEN
using D_Dev.EntityPool;
using D_Dev.TweenAnimations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.PopupSystem
{
    public class PopupView : PoolableObject
    {
        #region Fields

        [Title("Components")]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _icon;
        [SerializeField, Required] private TweenPlayer _showAnimation;

        private Transform _target;
        private Vector3 _offset;

        #endregion

        #region Monobehaviour

        private void Awake() => _showAnimation.OnComplete.AddListener(Release);

        private void LateUpdate()
        {
            if (_target != null && _target.gameObject.activeInHierarchy)
                transform.position = _target.position + _offset;
        }

        #endregion

        #region Public

        public void Show(PopupData data, Vector3 position)
        {
            _target = null;
            transform.position = position;
            Play(data);
        }

        public void Show(PopupData data, Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
            transform.position = target != null ? target.position + offset : offset;
            Play(data);
        }

        #endregion

        #region Override

        protected override void OnRelease()
        {
            _target = null;
            _showAnimation.Stop();
        }

        #endregion

        #region Private

        private void Play(PopupData data)
        {
            ApplyData(data);
            _showAnimation.Play();
        }

        private void ApplyData(PopupData data)
        {
            if (_text != null)
            {
                var hasText = !string.IsNullOrEmpty(data.Text);
                _text.gameObject.SetActive(hasText);
                _text.text = data.Text;
            }

            if (_icon != null)
            {
                var hasIcon = data.Icon != null;
                _icon.gameObject.SetActive(hasIcon);
                _icon.sprite = data.Icon;
            }
        }

        #endregion
    }
}
#endif
