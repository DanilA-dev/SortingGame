#if D_DEV_YG2
using UnityEngine;
using YG;

namespace D_Dev.PluginYG2
{
    public class YG2Handler : MonoBehaviour
    {
        #region Monobehaviour

        private void Start() => CallGameReady();

        #endregion
        
        #region Public

        public void CallGameReady() => YG2.GameReadyAPI();

        #endregion
    }
}
#endif
