using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace D_Dev.Utility
{
    [RequireComponent(typeof(Collider))]
    public class ColliderPointerClickEvent : MonoBehaviour, IPointerClickHandler
    {
        #region Fields

        [Title("Settings")]
        [SerializeReference] private PolymorphicValue<bool> _isInteractable = new BoolConstantValue() { Value = true };
        [SerializeField] private PointerEventData.InputButton _button = PointerEventData.InputButton.Left;
        [Space]
        [FoldoutGroup("Events")]
        public UnityEvent OnClick;
        [FoldoutGroup("Events")]
        public UnityEvent<PointerEventData> OnClickWithData;

        #endregion

        #region IPointerClickHandler

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isInteractable != null && !_isInteractable.Value)
                return;

            if (eventData.button != _button)
                return;

            OnClick?.Invoke();
            OnClickWithData?.Invoke(eventData);
        }

        #endregion
    }
}
