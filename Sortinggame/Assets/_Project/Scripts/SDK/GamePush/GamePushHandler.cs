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
            GP_Init.OnReady += OnSdkReady;
            GP_Init.OnError += OnSdkError;
        }

        private void OnDestroy() => Unsubscribe();

        #endregion

        #region Public

        public void CallGameReady() => GP_Game.GameReady();
        public void GameplayStart() => GP_Game.GameplayStart();
        public void GameplayStop() => GP_Game.GameplayStop();

        #endregion

        #region Private

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
