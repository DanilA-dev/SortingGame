#if DOTWEEN
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace D_Dev.TweenScrollSnapper
{
    [RequireComponent(typeof(ScrollRect))]
    public class TweenScrollRectSnapper : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
    {
        #region Enums

        public enum ScrollMovementDirection
        {
            Horizontal,
            Vertical
        }

        #endregion

        #region Fields

        [SerializeField] private bool _autoFindFocusScrollItem;
        [SerializeField] private bool _enableArrows;
        [ShowIf(nameof(_enableArrows))]
        [SerializeField] private Button _positveDirectionArrow;
        [ShowIf(nameof(_enableArrows))]
        [SerializeField] private Button _negativeDirectionArrow;
        [Title("Tween settings")]
        [SerializeField] private ScrollMovementDirection _scrollMovementDirection;
        [SerializeField] private float _snapTime;
        [SerializeField] private Ease _snapEase;
        [SerializeField] private bool _ignoreTimescale;
        [Space]
        [FoldoutGroup("Events")] 
        public UnityEvent OnElementFocus;
        [Space]
        [OnValueChanged(nameof(ToggleAutoFindInSettings))]
        [Title("Debug")]
        [SerializeField, ReadOnly] private float _scrollValue;
        [SerializeField, ReadOnly] private Vector2 _scrollDelta;
        [Space]
        [PropertyOrder(100)]
        [SerializeField] private List<FocusRectSettings> _focusRectSettings = new();

        private bool _isContentMoving;
        private Vector2 _startContentPos;
        private Vector2 _scrollBeginPos;
        private Vector2 _currentScrollPosition;
        private RectTransform _elementRect;
        private RectTransform _contentRect;
        private FocusRectSettings _currentFocusSettings;
        private FocusScrollItem _currentFocusScrollItem;
        private FocusScrollItem _prevFocusScrollItem;
        private FocusScrollItem[] _focusScrollItems;
        private ScrollRect _scrollRect;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _contentRect = _scrollRect.content;
            _scrollRect.horizontal = _scrollMovementDirection == ScrollMovementDirection.Horizontal;
            _scrollRect.vertical = _scrollMovementDirection == ScrollMovementDirection.Vertical;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
            
            if(_positveDirectionArrow != null)
                _positveDirectionArrow.onClick.AddListener((() => ArrowScroll(-1)));
            
            if(_negativeDirectionArrow != null)
                _negativeDirectionArrow.onClick.AddListener((() => ArrowScroll(1)));
        }

        private void Start()
        {
            InitScrollItems();
        }

        private void OnDisable()
        {
            if(_positveDirectionArrow != null)
                _positveDirectionArrow.onClick.RemoveListener((() => ArrowScroll(-1)));
            
            if(_negativeDirectionArrow != null)
                _negativeDirectionArrow.onClick.RemoveListener((() => ArrowScroll(1)));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startContentPos = _contentRect.anchoredPosition;
            _scrollBeginPos = eventData.position;
            _scrollRect.enabled = false;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            var dir = 0;
            var differ = 0.5f;
            
            _scrollDelta = (_scrollBeginPos - eventData.position).normalized;
            if (_scrollDelta.x > differ || _scrollDelta.y > differ)
                dir = -1;
            else if (_scrollDelta.x < differ || _scrollDelta.y < differ)
                dir = 1;
            _scrollValue = GetScrollValue(dir);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            CheckFocusRange();
            MoveContentScroll();
            _scrollDelta = Vector2.zero;
            _scrollRect.enabled = true;
        }

        #endregion

        #region Public

        public void ScrollAndFocus(RectTransform rectTransform, float scrollTime = 0)
        {
            var focusSettings =
                _focusRectSettings.FirstOrDefault(f =>
                    f.ElementRect.anchoredPosition == rectTransform.anchoredPosition);
            
            _positveDirectionArrow.gameObject.SetActive(false);
            _negativeDirectionArrow.gameObject.SetActive(false);
            MoveContentRect(focusSettings, scrollTime);
            SetFocusScrollRect(focusSettings);
        }

        #endregion

        #region Private

         private void InitScrollItems()
        {
            TryFindFocusElementsFromContent();
            SetElementsRelativePositions();
        }
        
        private void CheckFocusRange()
        {
            for (int i = 0; i < _focusRectSettings.Count; i++)
            {
                if (_focusRectSettings[i].InRange(_contentRect.anchoredPosition))
                    SetFocusScrollRect(_focusRectSettings[i]);
            }
            
            if (_negativeDirectionArrow != null)
            {
                var enable = _currentFocusSettings.Index != 0;
                _negativeDirectionArrow.gameObject.SetActive(enable);
            }

            if (_positveDirectionArrow != null)
            {
                var enable = _currentFocusSettings.Index != _focusRectSettings.Count - 1;
                _positveDirectionArrow.gameObject.SetActive(enable);
            }
        }
        private void ArrowScroll(int dir)
        {
            _startContentPos = _contentRect.anchoredPosition;
            _scrollValue = GetScrollValue(dir);
            MoveContentScroll();
        }
        
        private float GetScrollValue(int dir)
        {
            return _scrollMovementDirection == ScrollMovementDirection.Horizontal
                ? Mathf.Clamp(_startContentPos.x + (dir * _elementRect.rect.width), -_contentRect.sizeDelta.x, 0)
                : Mathf.Clamp(_startContentPos.y + (dir * _elementRect.rect.height), -_contentRect.sizeDelta.y, 0);
        }

        private void SetElementsRelativePositions()
        {
            for (var i = 0; i < _focusRectSettings.Count; i++)
                _focusRectSettings[i].GetRelativePosition(i, _scrollMovementDirection == ScrollMovementDirection.Horizontal,
                    _contentRect);
        }

        private void TryFindFocusElementsFromContent()
        {
            if (_autoFindFocusScrollItem)
            {
                _focusScrollItems = _contentRect.GetComponentsInChildren<FocusScrollItem>();
                _elementRect = _focusScrollItems[0].GetComponent<RectTransform>();
                if (_focusScrollItems != null)
                {
                    for (int i = 0; i < _focusScrollItems.Length; i++)
                    {
                        var focusRectSettings = new FocusRectSettings{FocusScrollItem = _focusScrollItems[i]};
                        _focusRectSettings.Add(focusRectSettings);
                    }
                }
            }
        }
        private void MoveContentScroll()
        {
            if (_isContentMoving)
                return;

            _currentScrollPosition = new Vector2(_scrollMovementDirection == ScrollMovementDirection.Horizontal? _scrollValue : _contentRect.anchoredPosition.x,
                _scrollMovementDirection == ScrollMovementDirection.Vertical? _scrollValue : _contentRect.anchoredPosition.y);
            
            _isContentMoving = true;
            _contentRect.DOAnchorPos(_currentScrollPosition, _snapTime)
                .SetEase(_snapEase)
                .SetUpdate(_ignoreTimescale)
                .SetAutoKill(true)
                .OnComplete((() =>
                {
                    _isContentMoving = false;
                    CheckFocusRange();
                }));
        }
        private void MoveContentRect(FocusRectSettings focusRectSettings, float customScrollTime = 0)
        {
            if (_isContentMoving)
                return;
            
            _currentScrollPosition = focusRectSettings.RelativePosition * -1;
            var scrollTime = customScrollTime == 0 ? _snapTime : customScrollTime;
            _isContentMoving = true;
            _contentRect.DOAnchorPos(_currentScrollPosition, scrollTime)
                .SetEase(_snapEase)
                .SetUpdate(_ignoreTimescale)
                .SetAutoKill(true)
                .OnComplete((() =>
                {
                    _isContentMoving = false;
                    CheckFocusRange();
                }));
        }
        
        private void SetFocusScrollRect(FocusRectSettings focusRectSettings)
        {
            if (_currentFocusScrollItem != null)
            {
                if(_currentFocusScrollItem == focusRectSettings.FocusScrollItem)
                    return;
                
                _prevFocusScrollItem = _currentFocusScrollItem;
                _prevFocusScrollItem.SetFocus(false);
            }

            _currentFocusSettings = focusRectSettings;
            _currentFocusScrollItem = _currentFocusSettings.FocusScrollItem;
            _currentFocusScrollItem.SetFocus(true);
            OnElementFocus?.Invoke();
        }

        [Button]
        private void MoveToElementIndex(int index)
        {
            var focusItem = 
                _focusRectSettings.FirstOrDefault(f => f.Index == index);
            
            ScrollAndFocus(focusItem.FocusScrollItem.GetComponent<RectTransform>());
        }
        
        private void ToggleAutoFindInSettings()
        {
            if(_focusRectSettings == null)
                return;

            foreach (var focusRectSettings in _focusRectSettings)
                focusRectSettings.ToggleAutoFindItem();
        }

        #endregion
    }

    [System.Serializable]
    public class FocusRectSettings
    {
        #region Fields

        [SerializeField, ReadOnly] private Vector2 _relativePosition;
        [ShowIf(nameof(_autoFindFocusScrollItem))]
        [SerializeField] private FocusScrollItem _focusScrollItem;

        private bool _autoFindFocusScrollItem;
        private RectTransform _elementRect;
        private int _index;
        private float _orientation;

        #endregion

        #region Properties

        public FocusScrollItem FocusScrollItem
        {
            get => _focusScrollItem;
            set => _focusScrollItem = value;
        }
        public int Index => _index;
        public Vector2 RelativePosition => _relativePosition;
        public RectTransform ElementRect => _elementRect;

        #endregion

        #region Public

        public void ToggleAutoFindItem() => _autoFindFocusScrollItem = !_autoFindFocusScrollItem;

        public void GetRelativePosition(int index, bool isHorizontal, RectTransform content)
        {
            if(_focusScrollItem != null)
                _elementRect = _focusScrollItem.GetComponent<RectTransform>();

            _index = index;
            _orientation = isHorizontal
                ? _elementRect.sizeDelta.x
                : _elementRect.sizeDelta.y;

            _relativePosition = new Vector2(Mathf.Abs(isHorizontal? _orientation * _index : content.anchoredPosition.x),
                Mathf.Abs(!isHorizontal? _orientation * _index : content.anchoredPosition.y));
        }

        public bool InRange(Vector2 pos)
        {
            return _relativePosition == new Vector2(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
        }

        #endregion
    }

}
#endif
