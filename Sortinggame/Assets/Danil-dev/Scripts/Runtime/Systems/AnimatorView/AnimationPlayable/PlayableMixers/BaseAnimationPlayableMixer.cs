using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    public abstract class BaseAnimationPlayableMixer : MonoBehaviour
    {
        #region Fields

        [Title("Playable Graph")] 
        [SerializeField] protected int _layer;
        [SerializeField] protected bool _isAdditive;
        [SerializeField] protected AnimationPlayableGraph _playableGraph;
        [Space]
        [SerializeField] protected bool _tryFoundOnItSelf;
        [SerializeField] protected bool _tryFoundOnGameObject;
        [ShowIf(nameof(_tryFoundOnGameObject))]
        [SerializeField] protected GameObject _graphObject;
        [SerializeField] protected bool _tryFoundOnParent;
        
        #endregion

        #region Properties

        public int Layer
        {
            get => _layer;
            set => _layer = value;
        }

        #endregion

        #region Monobehaviour

        protected virtual void OnEnable()
        {
            if(_tryFoundOnItSelf)
                TryGetComponent(out _playableGraph);
            
            if(_tryFoundOnGameObject && _graphObject != null)
                _graphObject.TryGetComponent(out _playableGraph);
            
            if(_tryFoundOnParent && transform.parent != null)
                transform.parent.TryGetComponent(out _playableGraph);
        }

        #endregion

        #region Helpers

        protected AnimationClipPlayable CreatePlayableClip(AnimationPlayableClipConfig config)
        {
            if (_playableGraph?.PlayableGraph.IsValid() != true)
                return new AnimationClipPlayable();

            if (config == null)
                return new AnimationClipPlayable();

            var clip = config.GetAnimationClip();
            if (clip == null)
                return new AnimationClipPlayable();

            var newPlayable = AnimationClipPlayable.Create(_playableGraph.PlayableGraph, clip);
            if(!config.IsLooping)
                newPlayable.SetDuration(clip.length);
            
            newPlayable.SetApplyFootIK(config.UseFootIK);
            newPlayable.SetTime(0);
            newPlayable.SetSpeed(config.AnimationSpeed);
            return newPlayable;
        }

        #endregion
    }
}
