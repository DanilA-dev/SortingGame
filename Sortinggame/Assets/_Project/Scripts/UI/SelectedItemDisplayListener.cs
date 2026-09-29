using D_Dev.CustomEventManager;
using D_Dev.MenuHandler;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class SelectedItemDisplayListener : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<GameObject> _selectedItemToDisplay = new GameObjectConstantValue();
        [SerializeField] private MenuInfo _selectedMenuInfo;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _selectedItemToDisplay.OnValueChanged += OnSelectedItemUpdate;
        }

        private void OnDisable()
        {
            _selectedItemToDisplay.OnValueChanged -= OnSelectedItemUpdate;
        }

        #endregion

        #region Listeners

        private void OnSelectedItemUpdate(GameObject item)
        {
            if (item == null)
                EventManager.Invoke(EventNameConstants.MenuClose.ToString(), _selectedMenuInfo);
            else
                EventManager.Invoke(EventNameConstants.MenuOpen.ToString(), _selectedMenuInfo);
        }

        #endregion
    }
}