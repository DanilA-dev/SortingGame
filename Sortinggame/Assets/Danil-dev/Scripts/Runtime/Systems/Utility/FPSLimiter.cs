using UnityEngine;

namespace D_Dev.Utility
{
    public class FPSLimiter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private int _targetFPS = 60;
        [SerializeField] private bool _limitEnabled = true;
        [SerializeField] private bool _showFPS = true;
        [SerializeField] private Color _fpsColor = Color.green;
        [SerializeField] private int _fontSize = 20;

        private float _deltaTime;
        private float _fps;
        private GUIStyle _style;

        #endregion

        #region Properties

        public int TargetFPS => _targetFPS;
        public bool LimitEnabled => _limitEnabled;
        public float CurrentFPS => _fps;

        #endregion

        #region Monobehavior

        private void Awake()
        {
            ApplyTargetFPS();
        }

        private void Update()
        {
            if (!_showFPS || !ShouldShowFPS())
                return;
            
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            _fps = 1.0f / _deltaTime;
        }

        private void OnGUI()
        {
            if (!_showFPS || !ShouldShowFPS())
                return;

            InitializeStyle();

            string fpsText = $"FPS: {(int)_fps}";
            GUI.Label(new Rect(10, 10, 200, 30), fpsText, _style);
        }

        #endregion

        #region Public Methods

        public void SetTargetFPS(int targetFPS)
        {
            _targetFPS = Mathf.Max(1, targetFPS);
            ApplyTargetFPS();
        }

        public void SetLimitEnabled(bool enabled)
        {
            _limitEnabled = enabled;
            ApplyTargetFPS();
        }

        public void ToggleLimit()
        {
            SetLimitEnabled(!_limitEnabled);
        }

        #endregion

        #region Private Methods

        private void ApplyTargetFPS()
        {
            if (_limitEnabled)
            {
                Application.targetFrameRate = _targetFPS;
            }
            else
            {
                Application.targetFrameRate = -1;
            }
        }

        private bool ShouldShowFPS()
        {
            return Application.isEditor || Debug.isDebugBuild;
        }

        private void InitializeStyle()
        {
            if (_style == null)
            {
                _style = new GUIStyle();
                _style.fontSize = _fontSize;
                _style.normal.textColor = _fpsColor;
                _style.fontStyle = FontStyle.Bold;
            }
        }

        #endregion
    }
}
