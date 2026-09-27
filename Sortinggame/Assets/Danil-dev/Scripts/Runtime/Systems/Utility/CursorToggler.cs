using UnityEngine;

namespace D_Dev.Utility
{
    public class CursorToggler : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class CursorState
        {
            [field: SerializeField] public CursorLockMode LockMode { get; set; }
            [field: SerializeField] public bool Visible { get; set; }
            
            public CursorState(CursorLockMode lockMode, bool visible)
            {
                LockMode = lockMode;
                Visible = visible;
            }
        }
        

        #endregion
        
        #region Fields

        [SerializeField] private int _setCursorStateIndexOnStart;
        [SerializeField] private CursorState[] _cursorStates;

        #endregion

        #region Monobehavior

        private void Start()
        {
            ApplyCursorStatePreset(_setCursorStateIndexOnStart);
        }

        #endregion

        #region Public

        public void ToggleCursor(CursorState cursorState)
        {
            Cursor.lockState = cursorState.LockMode;
            Cursor.visible = cursorState.Visible;
        }
        
        public void ApplyCursorStatePreset(int index)
        {
            if(_cursorStates.Length <= 0)
                return;
            
            var cursorState = _cursorStates[index];
            if (cursorState == null)
            {
                Debug.LogError($"[CursorToggler] Cursor state with index {index} is null");
                return;
            }
            ToggleCursor(cursorState);
        }

        #endregion
    }
}
