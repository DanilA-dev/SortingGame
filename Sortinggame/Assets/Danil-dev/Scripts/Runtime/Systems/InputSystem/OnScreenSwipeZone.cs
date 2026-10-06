using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace D_Dev.InputSystem
{
    public class OnScreenSwipeZone : OnScreenControl, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Fields

        [InputControl(layout = "Vector2")]
        [SerializeField] private string _controlPath = "<Mouse>/delta";
        [SerializeField] private float _sensitivity = 1f;

        private int _pointerId = int.MinValue;
        private int _lastDragFrame = -1;
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
            if (_hasValue && _lastDragFrame != Time.frameCount)
                ResetValue();
        }

        protected override void OnDisable()
        {
            ResetValue();
            _pointerId = int.MinValue;
            base.OnDisable();
        }

        #endregion

        #region IDragHandler

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_pointerId != int.MinValue)
                return;

            _pointerId = eventData.pointerId;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId)
                return;

            SendValueToControl(eventData.delta * _sensitivity);
            _lastDragFrame = Time.frameCount;
            _hasValue = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId)
                return;

            _pointerId = int.MinValue;
            ResetValue();
        }

        #endregion

        #region Private

        private void ResetValue()
        {
            if (!_hasValue)
                return;

            SendValueToControl(Vector2.zero);
            _hasValue = false;
        }

        #endregion
    }
}
