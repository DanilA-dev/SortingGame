using D_Dev.InputSystem;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public class ButtonInputBinding : BaseInputBinding
    {
        #region Enums

        public enum Button
        {
            Space = 0,
            Shift = 1,
            Ctrl = 2,
            Esc = 3,
            E = 4,
            Lmb = 5,
            Rmb = 6
        }

        #endregion

        #region Fields

        [SerializeField] private Button _button;
        [SerializeReference] private PolymorphicValue<bool> _output = new BoolConstantValue();

        #endregion

        #region Overrides

        public override void Bind(InputRouter router)
        {
            switch (_button)
            {
                case Button.Space: router.SpacePressed += OnButton; break;
                case Button.Shift: router.ShiftPressed += OnButton; break;
                case Button.Ctrl: router.CtrlPressed += OnButton; break;
                case Button.Esc: router.EscPressed += OnButton; break;
                case Button.E: router.EPressed += OnButton; break;
                case Button.Lmb: router.LmbPressed += OnButton; break;
                case Button.Rmb: router.RmbPressed += OnButton; break;
            }
        }

        public override void Unbind(InputRouter router)
        {
            switch (_button)
            {
                case Button.Space: router.SpacePressed -= OnButton; break;
                case Button.Shift: router.ShiftPressed -= OnButton; break;
                case Button.Ctrl: router.CtrlPressed -= OnButton; break;
                case Button.Esc: router.EscPressed -= OnButton; break;
                case Button.E: router.EPressed -= OnButton; break;
                case Button.Lmb: router.LmbPressed -= OnButton; break;
                case Button.Rmb: router.RmbPressed -= OnButton; break;
            }
        }

        #endregion

        #region Listeners

        private void OnButton(bool isPressed)
        {
            if (_output != null)
                _output.Value = isPressed;
        }

        #endregion
    }
}
