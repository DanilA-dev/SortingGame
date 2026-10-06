using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.UI;

namespace D_Dev.InputSystem
{
    public class OnScreenSwipeZone : OnScreenControl, IPointerDownHandler
    {
        #region Fields

        [InputControl(layout = "Vector2")]
        [SerializeField] private string _controlPath = "<Mouse>/delta";
        [SerializeField] private float _sensitivity = 1f;
        [SerializeField] private float _referenceHeight = 1080f;
        [SerializeField, Range(0f, 0.95f)] private float _smoothing = 0.5f;

        private Pointer _pointer;
        private int _touchId = -1;
        private Vector2 _smoothedDelta;
        private bool _hasValue;

        #endregion

        #region Properties

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        #endregion

        #region Monobehaviour

        private void Update()
        {
            if (!TryReadDelta(out var delta))
            {
                Release();
                return;
            }

            if (Screen.height > 0)
                delta *= _referenceHeight / Screen.height;

            _smoothedDelta = Vector2.Lerp(delta, _smoothedDelta, _smoothing);
            SendValueToControl(_smoothedDelta * _sensitivity);
            _hasValue = true;
        }

        protected override void OnDisable()
        {
            Release();
            base.OnDisable();
        }

        #endregion

        #region IPointerDownHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_pointer != null)
                return;

            if (eventData is not ExtendedPointerEventData extended || extended.device is not Pointer pointer)
                return;

            _pointer = pointer;
            _touchId = extended.pointerType == UIPointerType.Touch ? extended.touchId : -1;
            _smoothedDelta = Vector2.zero;
        }

        #endregion

        #region Private

        private bool TryReadDelta(out Vector2 delta)
        {
            delta = Vector2.zero;
            if (_pointer == null || !_pointer.added)
                return false;

            if (_pointer is Touchscreen touchscreen)
            {
                foreach (var touch in touchscreen.touches)
                {
                    if (touch.touchId.ReadValue() != _touchId)
                        continue;

                    if (!touch.isInProgress)
                        return false;

                    delta = touch.delta.ReadValue();
                    return true;
                }

                return false;
            }

            if (!_pointer.press.isPressed)
                return false;

            delta = _pointer.delta.ReadValue();
            return true;
        }

        private void Release()
        {
            _pointer = null;
            _touchId = -1;
            _smoothedDelta = Vector2.zero;

            if (!_hasValue)
                return;

            SendValueToControl(Vector2.zero);
            _hasValue = false;
        }

        #endregion
    }
}
