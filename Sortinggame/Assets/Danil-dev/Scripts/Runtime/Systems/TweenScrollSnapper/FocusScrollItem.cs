using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.TweenScrollSnapper
{
    public class FocusScrollItem : MonoBehaviour
    {
        #region Fields

        [FoldoutGroup("Events")] 
        public UnityEvent OnScrollFocus;
        [FoldoutGroup("Events")]
        public UnityEvent OnScrollResetFocus;

        #endregion

        #region Properties

        public bool IsFocused { get; private set; }


        #endregion

        #region Public

        public void SetFocus(bool value)
        {
            IsFocused = value;
            
            if(IsFocused)
                OnScrollFocus?.Invoke();
            else
                OnScrollResetFocus?.Invoke();
        }

        #endregion
    }
}
