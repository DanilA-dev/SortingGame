using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

namespace D_Dev.InputSystem
{
    public class ZoneUIStickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler
    {
        #region Fields

        [Title("Zone")]
        [SerializeField] private RectTransform _containerRectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [Title("Stick")]
        [SerializeField] private RectTransform _stickRect;
        [SerializeField] private OnScreenStick _onScreenStick;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _onScreenStick.behaviour = OnScreenStick.Behaviour.RelativePositionWithStaticOrigin;
            _stickRect.anchorMax = new Vector2(0.5f, 0.5f);
            _stickRect.anchorMin = new Vector2(0.5f, 0.5f);
            _stickRect.pivot = new Vector2(0.5f, 0.5f);
        }

        #endregion

        #region IPointerHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveStickToPosition(eventData.position, eventData.pressEventCamera);
            _canvasGroup.alpha = 1;
            _onScreenStick.OnPointerDown(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _onScreenStick.OnDrag(eventData);
            _canvasGroup.alpha = 1;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _onScreenStick.OnPointerUp(eventData);
            _canvasGroup.alpha = 0;
        }

        #endregion

        #region Private

        private void MoveStickToPosition(Vector2 screenPosition, Camera eventCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _containerRectTransform,
                screenPosition,
                eventCamera,
                out Vector2 localPoint);

            _stickRect.anchoredPosition = localPoint;
        }

        #endregion
    }
}