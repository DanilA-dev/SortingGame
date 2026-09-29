using D_Dev.CustomEventManager;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace _Project.Scripts
{
    public class LevelFinishHandler : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<int> _currentSortedItems = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _currentSortedShelves = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _maxSortedItems = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _maxSortedShelves = new IntConstantValue();

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _currentSortedItems.OnValueChanged += OnItemsSortedUpdate;
            _currentSortedShelves.OnValueChanged += OnShelvesSortedUpdate;
        }

        private void OnDisable()
        {
            _currentSortedItems.OnValueChanged -= OnItemsSortedUpdate;
            _currentSortedShelves.OnValueChanged -= OnShelvesSortedUpdate;
        }

        #endregion

        #region Private

        private void TryFinishLevel()
        {
            if (_currentSortedItems.Value == _maxSortedItems.Value &&
                _currentSortedShelves.Value == _maxSortedShelves.Value)
            {
                EventManager.Invoke(EventNameConstants.GameFinish.ToString());
            }
        }

        #endregion

        #region Listeners

        private void OnItemsSortedUpdate(int sortedItems)
        {
            TryFinishLevel();
        }

        private void OnShelvesSortedUpdate(int sortedShelves)
        {
            TryFinishLevel();
        }

        #endregion
        
        
    }
}