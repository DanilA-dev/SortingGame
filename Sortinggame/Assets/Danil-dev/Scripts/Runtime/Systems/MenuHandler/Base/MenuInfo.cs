using D_Dev.AddressablesExstensions;
using UnityEngine;

namespace D_Dev.MenuHandler
{
    [CreateAssetMenu(menuName = "D-Dev/Info/MenuInfo")]
    public class MenuInfo : ScriptableObject
    {
        #region Enums

        public enum CanvasType
        {
            Overlay = 0,
            Camera = 1
        }

        #endregion

        #region Fields

        [SerializeField] private CanvasType _canvas;
        [SerializeField] private AddressablesGameObjectLoadData _menuPrefab;
        [SerializeField] private bool _openOnCreate;

        #endregion

        #region Properties

        public CanvasType Canvas => _canvas;
        public AddressablesGameObjectLoadData MenuPrefab => _menuPrefab;
        public bool OpenOnCreate => _openOnCreate;

        #endregion
    }
}
