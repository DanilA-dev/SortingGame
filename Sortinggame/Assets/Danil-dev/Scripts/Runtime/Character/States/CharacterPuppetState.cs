using D_Dev.PolymorphicValueSystem;
using D_Dev.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Character.States
{
    public class CharacterPuppetState : CharacterState
    {
        #region Fields

        [Space]
        [SerializeField, Required] private RagdollController _ragdollController;
        [SerializeReference] private PolymorphicValue<bool> _isPuppetActive = new BoolConstantValue();

        #endregion

        #region State

        public override void OnEnter()
        {
            base.OnEnter();
            _ragdollController?.ActivateRagdoll();
            _isPuppetActive.Value = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            _ragdollController?.DeactivateRagdoll();
            _isPuppetActive.Value = false;
        }

        #endregion
    }
}