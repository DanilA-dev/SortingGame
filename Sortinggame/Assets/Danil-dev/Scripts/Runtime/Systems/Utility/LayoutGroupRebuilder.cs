using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.Utility
{
    public class LayoutGroupRebuilder : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _immediate;
        [SerializeField] private RectTransform _layout;
        [Space]
        [SerializeField] private bool _rebuildOnStart;
        [ShowIf(nameof(_layout))]
        [SerializeField] private float _startDelay;

        #endregion

        #region Monobehaviour

        private IEnumerator Start()
        {
            if(!_rebuildOnStart)
                yield break;
            
            yield return new WaitForSecondsRealtime(_startDelay);
            Rebuild();
        }

        #endregion

        #region Public

        public void Rebuild()
        {
            if(!_immediate)
                LayoutRebuilder.MarkLayoutForRebuild(_layout);
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
            }
        }

        #endregion
    }
}