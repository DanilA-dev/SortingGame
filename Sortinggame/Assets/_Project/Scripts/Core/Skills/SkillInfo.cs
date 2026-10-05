using System;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Skills
{
    [CreateAssetMenu(menuName = "Game/Skills/New Skill Info")]
    public class SkillInfo : ScriptableObject
    {
        #region Fields

        [Title("Base")]
        [SerializeReference] private PolymorphicValue<string> _skillName = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<string> _skillDescription = new StringConstantValue();
        [PreviewField(75, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;

        [Space]
        [Title("Values")] 
        [SerializeReference] private PolymorphicValue<bool> _isLocked = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<float> _duration = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _cooldown = new FloatConstantValue();

        public event Action OnUseStarted;
        public event Action OnUseStopped;
        public event Action OnReady;
        public event Action OnNotAvailable;
        public event Action OnUseRequested;
        public event Action OnUseImmediately;
        public event Action OnStopImmediately;

        #endregion

        #region Properties

        public PolymorphicValue<string> SkillName => _skillName;

        public PolymorphicValue<string> SkillDescription => _skillDescription;

        public Sprite Icon => _icon;

        public PolymorphicValue<float> Duration => _duration;

        public PolymorphicValue<float> Cooldown => _cooldown;

        public PolymorphicValue<bool> IsLocked => _isLocked;

        #endregion

        #region Public

        public void NotifyUseStarted() => OnUseStarted?.Invoke();
        public void NotifyUseStopped() => OnUseStopped?.Invoke();
        public void NotifyReady() => OnReady?.Invoke();
        public void NotifyNotAvailable() => OnNotAvailable?.Invoke();

        public void UseImmediately() => OnUseImmediately?.Invoke();
        public void StopImmediately() => OnStopImmediately?.Invoke();
        
        public void RequestUse() => OnUseRequested?.Invoke();

        #endregion

    }
}