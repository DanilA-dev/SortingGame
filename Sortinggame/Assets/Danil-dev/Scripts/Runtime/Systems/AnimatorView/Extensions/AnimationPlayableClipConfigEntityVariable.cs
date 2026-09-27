using D_Dev.AnimatorView.AnimationPlayableHandler;
using D_Dev.EntityVariable;
using D_Dev.ScriptableVariables;

namespace D_Dev.AnimatorView.Extensions
{
    [System.Serializable]
    public class AnimationPlayableClipConfigEntityVariable : EntityVariable<AnimationPlayableClipConfig>
    {
        #region Constructors

        public AnimationPlayableClipConfigEntityVariable() {}
        
        public AnimationPlayableClipConfigEntityVariable(StringScriptableVariable id, AnimationPlayableClipConfig value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new AnimationPlayableClipConfigEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}