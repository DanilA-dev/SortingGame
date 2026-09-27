using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using D_Dev.CustomEventManager;
using D_Dev.Singleton;
using UnityEngine;

namespace D_Dev.MenuHandler
{
    public class MenuController : BaseSingleton<MenuController>
    {
        #region Fields

        [SerializeField] private bool _createMenusOnEnable;
        [SerializeField] private RectTransform _overlayCanvas;
        [SerializeField] private RectTransform _cameraCanvas;
        [SerializeField] private List<MenuInfo> _menuInfos = new();

        private Dictionary<MenuInfo,BaseMenu> _createdMenus = new();

        #endregion

        #region Monobehaviour

        protected override void Awake()
        {
            base.Awake();
            
            EventManager.AddListener<MenuInfo>(EventNameConstants.MenuOpen.ToString(), OpenMenu);
            EventManager.AddListener<MenuInfo>(EventNameConstants.MenuClose.ToString(), CloseMenu);
        }

        private void OnEnable()
        {
            if(_createMenusOnEnable)
                CreateMenus();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<MenuInfo>(EventNameConstants.MenuOpen.ToString(), OpenMenu);
            EventManager.RemoveListener<MenuInfo>(EventNameConstants.MenuClose.ToString(), CloseMenu);
        }

        #endregion

        #region Public

        public async void CreateMenus()
        {
            if (_menuInfos.Count <= 0)
            {
                Debug.LogError($"No menu infos found");
                return;
            }

            foreach (var menuInfo in _menuInfos)
            {
                var newMenu = await CreateMenu(menuInfo);
                if(menuInfo.OpenOnCreate)
                    newMenu.Open();
                
                _createdMenus.Add(menuInfo,newMenu);
            }
        }

        public async void OpenMenu(MenuInfo menuInfo)
        {
            if(_createdMenus.TryGetValue(menuInfo, out var menu))
                menu.Open();
            else
            {
                var newMenu = await CreateMenu(menuInfo);
                _createdMenus.TryAdd(menuInfo,newMenu);
                newMenu.Open();
            }
        }

        public void CloseMenu(MenuInfo menuInfo)
        {
            if(_createdMenus.TryGetValue(menuInfo, out var menu))
                menu.Close();
        }

        public void CloseAllMenus()
        {
            if(_createdMenus.Count <= 0)
                return;

            foreach (var keyValuePair in _createdMenus)
                keyValuePair.Value.Close();
        }

        #endregion

        #region Static

        public static bool IsMenuOpen(MenuInfo menuInfo)
        {
            if(_instance._createdMenus.TryGetValue(menuInfo, out var menu))
                return menu.IsOpen;
            
            return false;
        }
        
        public static bool IsMenuClosed(MenuInfo menuInfo)
        {
            if(_instance._createdMenus.TryGetValue(menuInfo, out var menu))
                return !menu.IsOpen;
            
            return false;
        }
        
        public static async UniTask WaitForMenuOpen(MenuInfo menuInfo)
        {
            try
            {
                if (_instance._createdMenus.TryGetValue(menuInfo, out var menu))
                    await UniTask.WaitUntil(() => menu.IsOpen,
                        cancellationToken: _instance.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException e) {}
        }
        
        public static async UniTask WaitForMenuClose(MenuInfo menuInfo)
        {
            try
            {
                if (_instance._createdMenus.TryGetValue(menuInfo, out var menu))
                    await UniTask.WaitUntil(() => !menu.IsOpen,
                        cancellationToken: _instance.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException e) {}
        }

        #endregion

        #region Private

        private async UniTask<BaseMenu> CreateMenu(MenuInfo menuInfo)
        {
            var menuParent = menuInfo.Canvas == 
                             MenuInfo.CanvasType.Overlay ? _overlayCanvas : _cameraCanvas;
            var newMenu  = await menuInfo.MenuPrefab.InstantiateAsync();
            newMenu.transform.SetParent(menuParent, false);
            newMenu.gameObject.SetActive(false);
            return newMenu.GetComponent<BaseMenu>();
        }

        #endregion
    }
}
