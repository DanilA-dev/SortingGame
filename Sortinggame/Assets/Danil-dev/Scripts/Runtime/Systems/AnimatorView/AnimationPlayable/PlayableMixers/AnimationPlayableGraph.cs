using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    [DefaultExecutionOrder(-1)]
    public class AnimationPlayableGraph : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Animator _animator;
        [SerializeField] private int _layerCount = 1;

        private AnimationPlayableOutput _animationPlayableOutput;
            
        #endregion

        #region Properties

        public PlayableGraph PlayableGraph { get; private set; }
        public AnimationLayerMixerPlayable RootLayerMixer { get; private set; }
        public Animator Animator => _animator;
        public bool IsPlayableGraphInitialized { get; private set; }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            PlayableGraph = PlayableGraph.Create($"AnimationPlayableGraph {gameObject.name} - {gameObject.GetInstanceID()}");
            _animationPlayableOutput = AnimationPlayableOutput.Create(PlayableGraph, "Animation", _animator);
            RootLayerMixer = AnimationLayerMixerPlayable.Create(PlayableGraph, _layerCount);
            _animationPlayableOutput.SetSourcePlayable(RootLayerMixer);
            PlayableGraph.Play();
            
            IsPlayableGraphInitialized = true;
        }

        private void OnDestroy()
        {
            if (!PlayableGraph.IsValid())
                return;

            if (RootLayerMixer.IsValid())
            {
                int inputCount = RootLayerMixer.GetInputCount();
                for (int i = 0; i < inputCount; i++)
                {
                    if (RootLayerMixer.GetInput(i).IsValid())
                    {
                        RootLayerMixer.DisconnectInput(i);
                    }
                }
            }

            if (_animationPlayableOutput.IsOutputValid())
            {
                _animationPlayableOutput.SetSourcePlayable(Playable.Null);
            }

            PlayableGraph.Destroy();
        }

        #endregion
    }
}
