using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    public class AnimationPlayableEventHandler : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class ClipTimeEvent
        {
            [Range(0f, 1f)]
            public float NormalizedTime = 0.5f;
            public UnityEvent OnTrigger;
        }

        [System.Serializable]
        public class ClipEvent
        {
            public AnimationClip Clip;
            public List<ClipTimeEvent> TimeEvents = new();
        }

        private struct TrackedEvent
        {
            public AnimationClip Clip;
            public ClipTimeEvent TimeEvent;
            public float NormalizedTime;
            public float PreviousNormalizedTime;
            public bool WasTriggeredThisCycle;
        }

        #endregion

        #region Fields

        [SerializeField] private AnimationClipPlayableMixer _mixer;
        
        [SerializeField] private List<ClipEvent> _animationEvents;

        private List<TrackedEvent> _trackedEvents = new();

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            InitializeTrackedEvents();
        }

        private void LateUpdate()
        {
            if (_mixer == null)
                return;

            ProcessActivePlayables();
        }

        private void OnValidate()
        {
            InitializeTrackedEvents();
        }

        #endregion

        #region Private

        private void InitializeTrackedEvents()
        {
            _trackedEvents.Clear();

            if (_animationEvents == null)
                return;

            foreach (var clipEvent in _animationEvents)
            {
                if (clipEvent?.Clip == null || clipEvent.TimeEvents == null)
                    continue;

                foreach (var timeEvent in clipEvent.TimeEvents)
                {
                    if (timeEvent == null)
                        continue;

                    _trackedEvents.Add(new TrackedEvent
                    {
                        Clip = clipEvent.Clip,
                        TimeEvent = timeEvent,
                        NormalizedTime = Mathf.Clamp01(timeEvent.NormalizedTime),
                        PreviousNormalizedTime = -1f,
                        WasTriggeredThisCycle = false
                    });
                }
            }
        }

        private void ProcessActivePlayables()
        {
            var activePlayables = _mixer.GetActivePlayables();
            
            foreach (var kvp in activePlayables)
            {
                var pair = kvp.Value;
                
                if (!pair.Playable.IsValid())
                    continue;

                var clip = pair.Playable.GetAnimationClip();
                if (clip == null)
                    continue;

                float currentTime = (float)pair.Playable.GetTime();
                float clipLength = clip.length;
                
                if (clipLength <= 0)
                    continue;

                float normalizedTime = (currentTime % clipLength) / clipLength;
                bool isLooping = clip.isLooping;

                ProcessEventsForClip(clip, normalizedTime, isLooping);
            }
        }

        private void ProcessEventsForClip(AnimationClip clip, float normalizedTime, bool isLooping)
        {
            for (int i = 0; i < _trackedEvents.Count; i++)
            {
                var trackedEvent = _trackedEvents[i];
                
                if (trackedEvent.Clip != clip)
                    continue;

                float eventTime = trackedEvent.NormalizedTime;
                float prevTime = trackedEvent.PreviousNormalizedTime;

                bool shouldTrigger = false;

                if (isLooping)
                {
                    if (prevTime < 0)
                    {
                        shouldTrigger = false;
                    }
                    else if (normalizedTime < prevTime)
                    {
                        if (eventTime >= prevTime || eventTime <= normalizedTime)
                        {
                            shouldTrigger = true;
                        }
                    }
                    else if (prevTime < eventTime && normalizedTime >= eventTime)
                    {
                        shouldTrigger = true;
                    }
                }
                else
                {
                    if (prevTime >= 0 && prevTime < eventTime && normalizedTime >= eventTime)
                    {
                        shouldTrigger = true;
                    }
                }

                if (shouldTrigger && !trackedEvent.WasTriggeredThisCycle)
                {
                    trackedEvent.TimeEvent?.OnTrigger?.Invoke();
                    trackedEvent.WasTriggeredThisCycle = true;
                }

                if (isLooping && prevTime > normalizedTime)
                {
                    trackedEvent.WasTriggeredThisCycle = false;
                }

                if (!isLooping && prevTime < 0)
                {
                    trackedEvent.WasTriggeredThisCycle = false;
                }

                trackedEvent.PreviousNormalizedTime = normalizedTime;
                _trackedEvents[i] = trackedEvent;
            }
        }

        #endregion

        #region Public

        public void AddEvent(AnimationClip clip, float normalizedTime, UnityAction action)
        {
            if (clip == null)
                return;

            var clipEvent = _animationEvents?.Find(ce => ce.Clip == clip);
            
            if (clipEvent == null)
            {
                clipEvent = new ClipEvent
                {
                    Clip = clip,
                    TimeEvents = new List<ClipTimeEvent>()
                };
                _animationEvents ??= new List<ClipEvent>();
                _animationEvents.Add(clipEvent);
            }

            var timeEvent = new ClipTimeEvent
            {
                NormalizedTime = normalizedTime
            };
            timeEvent.OnTrigger.AddListener(action);
            clipEvent.TimeEvents.Add(timeEvent);

            InitializeTrackedEvents();
        }

        public void ClearEvents()
        {
            _animationEvents?.Clear();
            InitializeTrackedEvents();
        }

        #endregion
    }
}
