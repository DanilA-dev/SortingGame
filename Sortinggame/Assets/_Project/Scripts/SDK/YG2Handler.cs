using UnityEngine;
using YG;

namespace _Project.Scripts.SDK
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
