using D_Dev.Base;
using UnityEngine;

namespace D_Dev.AddressablesExstensions.Extensions
{
    [System.Serializable]
    public class AddressablesPreloaderLoadAction : BaseAction
    {
        #region Fields

        [SerializeField] private AddressablesPreloader _addressablesPreloader;

        private bool _isPreloadStart;
        
        #endregion

        #region Overrides

        public override void Execute()
        {
            if (_addressablesPreloader == null)
            {
                IsFinished = true;
                return;
            }

            if (!_isPreloadStart)
            {
                _isPreloadStart = true;
                _addressablesPreloader.Preload();
            }

            IsFinished = _addressablesPreloader.IsLoaded;
        }

        #endregion
    }
}