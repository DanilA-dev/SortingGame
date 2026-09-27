using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    public class AnimationClipPlayableClipSwitcher : MonoBehaviour
    {
        [System.Serializable]
        public class ClipPlayableConfig
        {
            [SerializeField] private AnimationPlayableClipConfig _clip;
            
            public AnimationClipPlayableMixer Handler { get; set; }

            [Button]
            private void PlayClip()
            {
                Handler?.Play(_clip);
            }
        }
        
        [SerializeField] private AnimationClipPlayableMixer _animationClipPlayableHandler;
        [SerializeField] private ClipPlayableConfig[] _clips;

        private void Start()
        {
            if(_clips.Length == 0)
                return;

            foreach (var clipPlayableConfig in _clips)
                clipPlayableConfig.Handler = _animationClipPlayableHandler;
        }
    }
}
