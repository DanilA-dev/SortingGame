using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.AnimatorView.Extensions
{
    public abstract class BaseAnimationStatePlayer<TConfig, TPlayer> : MonoBehaviour
    {
        [Serializable]
        public class StateConfig
        {
            [SerializeReference] public PolymorphicValue<string> StateName;
            public TConfig AnimationConfig;
        }

        [SerializeField] protected TPlayer _player;
        [SerializeField] protected StateConfig[] _configs;

        public void PlayAnimation(string stateName)
        {
            if (_configs == null || _configs.Length <= 0)
                return;

            foreach (var config in _configs)
            {
                if (config.StateName.Value.Equals(stateName))
                {
                    OnPlay(config.AnimationConfig);
                    return;
                }
            }
        }

        public void PlayAnimation(PolymorphicValue<string> stateName)
        {
            if (_configs == null || _configs.Length <= 0)
                return;

            foreach (var config in _configs)
            {
                if (config.StateName.Equals(stateName))
                {
                    OnPlay(config.AnimationConfig);
                    return;
                }
            }
        }

        protected abstract void OnPlay(TConfig config);
    }
}
