using D_Dev.ScriptableVariables;
using D_Dev.TweenAnimations;
using D_Dev.ValueViewProvider;
using UnityEngine;

namespace D_Dev.ScriptableVariables.Extensions.ViewProviders
{
    public abstract class ScriptableVariableViewProvider<TValue, TVariable, TAnimation> : GenericValueViewProvider<TValue, TAnimation>
        where TVariable : BaseScriptableVariable<TValue>
        where TAnimation : BaseTweenValueAnimation<TValue>
    {
        #region Fields

        [SerializeField] protected TVariable _variable;

        #endregion

        #region Monobehaviour

        protected virtual void OnEnable()
        {
            if (_variable != null)
            {
                _variable.OnValueUpdate += UpdateView;
                UpdateView(_variable.Value);
            }
        }

        protected virtual void OnDisable()
        {
            if (_variable != null)
                _variable.OnValueUpdate -= UpdateView;
        }

        #endregion
    }
}
