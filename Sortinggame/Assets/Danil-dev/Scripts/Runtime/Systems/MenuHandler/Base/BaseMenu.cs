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
        public UnityEvent OnOpenEvent;
        [FoldoutGroup("Events")]
        public UnityEvent OnCloseEvent;
        
        #endregion

        #region Properties
        public bool IsOpen => _isOpen;

        #endregion

        #region Monobehaviour

        protected void Awake() => ForceClose();

        #endregion

        #region Public

public async void Open()
        {
            if(IsOpen)
                return;
            
            gameObject.SetActive(true);
#if DOTWEEN
            if (_hasOpenAnimation && _openAnimation != null)
            {
                var tcs = new UniTaskCompletionSource();
                
                void OnComplete()
                {
                    _openAnimation.OnComplete -= OnComplete;
                    tcs.TrySetResult();
                }
                
                _openAnimation.OnComplete += OnComplete;
                _openAnimation.Play();
                
                await tcs.Task;
                _isOpen = true;
                OnOpenEvent?.Invoke();
                return;
            }
#endif
            _isOpen = true;
            OnOpenEvent?.Invoke();
        }

        public async void Close()
        {
            if(!IsOpen)
                return;
            
#if DOTWEEN
            if (_hasCloseAniation && _closeAnimation != null)
            {
                var tcs = new UniTaskCompletionSource();
                
                void OnComplete()
                {
                    _closeAnimation.OnComplete -= OnComplete;
                    tcs.TrySetResult();
                }
                
                _closeAnimation.OnComplete += OnComplete;
                _closeAnimation.Play();
                
                await tcs.Task;
                _isOpen = false;
                gameObject.SetActive(!_disableObjectOnComplete);
                OnCloseEvent?.Invoke();
                return;
            }
#endif
            ForceClose();
        }

        public void ForceOpen()
        {
            _isOpen = true;
            gameObject.SetActive(IsOpen);
        }

        public void ForceClose()
        {
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

        private Color ActiveColor() => _isOpen ? Color.green : Color.red;

        #endregion
    }
}
