using GamePush;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.SDK
{
    public class GamePushHandler : MonoBehaviour
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private bool _callGameReadyOnInit = true;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onReady;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onError;

        private bool _isSubscribed;
        private bool _isGameplayStarted;
        private bool _isGameplayActive;

        #endregion

        #region Properties

        public bool IsReady => GP_Init.isReady;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            if (GP_Init.isReady)
            {
                OnSdkReady();
                return;
            }

            _isSubscribed = true;
            
            GP_Ads.ShowSticky();
            GP_Init.OnReady += OnSdkReady;
            GP_Init.OnError += OnSdkError;
        }

        private void OnDestroy() => Unsubscribe();

        #endregion

        #region Public

        public void CallGameReady() => GP_Game.GameReady();
        public void GameplayStart()
        {
            _isGameplayStarted = true;
            SetGameplayActive(true);
        }

        public void GameplayResume()
        {
            if (_isGameplayStarted)
                SetGameplayActive(true);
        }

        public void GameplayStop() => SetGameplayActive(false);

        #endregion

        #region Private

        private void SetGameplayActive(bool isActive)
        {
            if (_isGameplayActive == isActive)
                return;

            _isGameplayActive = isActive;

            if (isActive)
                GP_Game.GameplayStart();
            else
                GP_Game.GameplayStop();
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
                return;

            _isSubscribed = false;
            GP_Init.OnReady -= OnSdkReady;
            GP_Init.OnError -= OnSdkError;
        }

        #endregion

        #region Listeners

        private void OnSdkReady()
        {
            Unsubscribe();

            if (_callGameReadyOnInit)
                CallGameReady();

            _onReady?.Invoke();
        }

        private void OnSdkError()
        {
            Unsubscribe();
            Debug.LogWarning("[GamePushHandler] GamePush SDK initialization failed");
            _onError?.Invoke();
        }

        #endregion
    }
}
