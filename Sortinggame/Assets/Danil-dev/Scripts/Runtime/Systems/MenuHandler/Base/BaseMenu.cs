using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
#if DOTWEEN
using D_Dev.TweenAnimations;
#endif

namespace D_Dev.MenuHandler
{
    public class BaseMenu : MonoBehaviour
    {
        #region Fields

        [OnValueChanged(nameof(ActiveColor))]
        [GUIColor(nameof(ActiveColor))]
        [SerializeField, ReadOnly] private bool _isOpen;
        [Title("Animations")]
        [SerializeField] private bool _hasOpenAnimation;
        [SerializeField] private bool _hasCloseAniation;
        [ShowIf(nameof(_hasCloseAniation))]
        [SerializeField] private bool _disableObjectOnComplete;
#if DOTWEEN
        [ShowIf(nameof(_hasOpenAnimation))]
        [SerializeField] private TweenPlayable _openAnimation;

        [ShowIf(nameof(_hasCloseAniation))]
        [SerializeField] private TweenPlayable _closeAnimation;
#endif
        [FoldoutGroup("Events")]
        public UnityEvent OnBeforeOpenEvent;
        [FoldoutGroup("Events")]
        public UnityEvent OnOpenEvent;
        [FoldoutGroup("Events")]
        public UnityEvent OnBeforeCloseEvent;
        [FoldoutGroup("Events")]
        public UnityEvent OnCloseEvent;

        private bool _isTargetOpen;
        private int _transitionId;
        private Action _cancelTransition;

        #endregion

        #region Properties
        public bool IsOpen => _isOpen;
        public bool IsOpening => _isTargetOpen && !_isOpen;
        public bool IsClosing => !_isTargetOpen && _isOpen;

        #endregion

        #region Monobehaviour

        protected void Awake() => ForceClose();

        #endregion

        #region Public

        public async void Open()
        {
            if(_isTargetOpen)
                return;

            var wasOpen = _isOpen;
            var transitionId = BeginTransition(true);
            OnBeforeOpenEvent?.Invoke();
            gameObject.SetActive(true);
#if DOTWEEN
            _closeAnimation?.Kill();
            if (_hasOpenAnimation && _openAnimation != null)
            {
                if (!await PlayAnimation(_openAnimation, transitionId))
                    return;
            }
#endif
            _isOpen = true;
            if (!wasOpen)
                OnOpenEvent?.Invoke();
        }

        public async void Close()
        {
            if(!_isTargetOpen)
                return;

            var wasOpen = _isOpen;
            var transitionId = BeginTransition(false);
            OnBeforeCloseEvent?.Invoke();
#if DOTWEEN
            _openAnimation?.Kill();
            if (_hasCloseAniation && _closeAnimation != null)
            {
                if (!await PlayAnimation(_closeAnimation, transitionId))
                    return;

                _isOpen = false;
                gameObject.SetActive(!_disableObjectOnComplete);
                if (wasOpen)
                    OnCloseEvent?.Invoke();
                return;
            }
#endif
            ForceClose();
        }

        public void ForceOpen()
        {
            BeginTransition(true);
            KillAnimations();
            _isOpen = true;
            gameObject.SetActive(IsOpen);
        }

        public void ForceClose()
        {
            BeginTransition(false);
            KillAnimations();
            _isOpen = false;
            gameObject.SetActive(IsOpen);
            OnCloseEvent?.Invoke();
        }

        #endregion

        #region Virtual
        protected virtual void OnOpen() {}
        protected virtual void OnClose() {}

        #endregion

        #region Private

        private int BeginTransition(bool isTargetOpen)
        {
            _isTargetOpen = isTargetOpen;
            _transitionId++;

            var cancelTransition = _cancelTransition;
            _cancelTransition = null;
            cancelTransition?.Invoke();

            return _transitionId;
        }

        private void KillAnimations()
        {
#if DOTWEEN
            _openAnimation?.Kill();
            _closeAnimation?.Kill();
#endif
        }

#if DOTWEEN
        private async UniTask<bool> PlayAnimation(TweenPlayable animation, int transitionId)
        {
            var tcs = new UniTaskCompletionSource();

            void OnComplete() => tcs.TrySetResult();

            animation.OnComplete += OnComplete;
            _cancelTransition = OnComplete;
            animation.Play();

            await tcs.Task;
            animation.OnComplete -= OnComplete;

            return transitionId == _transitionId;
        }
#endif

        private Color ActiveColor() => _isOpen ? Color.green : Color.red;

        #endregion
    }
}
