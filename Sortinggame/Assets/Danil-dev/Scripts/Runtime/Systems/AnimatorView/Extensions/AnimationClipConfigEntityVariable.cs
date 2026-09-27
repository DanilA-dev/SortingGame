using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;

namespace D_Dev.AnimatorView.Extensions
{
    [System.Serializable]
    public class AnimationClipConfigEntityVariable : EntityVariable<AnimationClipConfig>
    {
        #region Constructors

        public AnimationClipConfigEntityVariable() {}
        
        public AnimationClipConfigEntityVariable(StringScriptableVariable id, AnimationClipConfig value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new AnimationClipConfigEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}