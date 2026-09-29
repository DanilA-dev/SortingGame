using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    public class GameObjectSlot : MonoBehaviour
    {
        #region Fields

        [SerializeField, ReadOnly] private GameObject _item;

        #endregion

        #region Properties

        public bool IsBusy => _item != null;

        public GameObject Item => _item;

        #endregion

        #region Public

        public void FreeSlot()
        {
            if (_item != null)
            {
                _item.transform.parent = null;
                _item = null;
            }
        }

        public bool TryPutItem(GameObject newItem, bool autoParent)
        {
            if (IsBusy)
            {
                Debug.Log($"[GameObjectSlot]_{gameObject.name} is busy!");
                return false;
            }
            _item = newItem;
            
            if(autoParent)
                _item.transform.SetParent(transform);

            return true;
        }

        public bool TrySwapItem(GameObject newItem, out GameObject swappedItem,bool autoParent)
        {
            if (!IsBusy)
            {
                Debug.Log($"[GameObjectSlot]_{gameObject.name} has no item to swap!");
                swappedItem = null;
                return false;
            }

            swappedItem = _item;
            FreeSlot();
            TryPutItem(newItem, autoParent);
            return true;
        }

        #endregion
    }
}