using System;
using System.Collections.Generic;
using D_Dev.Singleton;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace D_Dev.AnimatorView.AnimationPlayableHandler
{
    public class AnimationPlayableBlendManager : BaseSingleton<AnimationPlayableBlendManager>
    {
        #region Classes

        private struct BlendOperation
        {
            public int Id;
            public int Layer;
            public float Duration;
            public float Delay;
            public float Elapsed;
            public float DelayElapsed;
            public float FromWeight;
            public float ToWeight;
            public bool IsComplete;
            public AnimationLayerMixerPlayable Mixer;
            public Action OnComplete;
        }

        #endregion

        #region Fields

        private readonly List<BlendOperation> _operations = new(64);
        private readonly List<int> _toRemove = new(16);
        private int _nextId;

        #endregion

        #region Monobehaviour

        private void Update()
        {
            if (_operations.Count == 0)
                return;

            _toRemove.Clear();
            float dt = Time.deltaTime;

            for (int i = 0; i < _operations.Count; i++)
            {
                var op = _operations[i];

                if (op.IsComplete)
                {
                    _toRemove.Add(i);
                    continue;
                }

                if (op.DelayElapsed < op.Delay)
                {
                    op.DelayElapsed += dt;
                    _operations[i] = op;
                    continue;
                }

                op.Elapsed += dt;
                float t = Mathf.Clamp01(op.Elapsed / op.Duration);
                float weight = Mathf.Lerp(op.FromWeight, op.ToWeight, t);

                if (op.Mixer.IsValid())
                    op.Mixer.SetInputWeight(op.Layer, weight);

                if (t >= 1f)
                {
                    op.IsComplete = true;
                    op.OnComplete?.Invoke();
                }

                _operations[i] = op;
            }

            for (int i = _toRemove.Count - 1; i >= 0; i--)
                _operations.RemoveAt(_toRemove[i]);
        }

        #endregion

        #region Public

        public int Schedule(AnimationLayerMixerPlayable mixer, int layer,
            float fromWeight, float toWeight, float duration,
            float delay = 0f, Action onComplete = null)
        {
            _operations.Add(new BlendOperation
            {
                Id = _nextId++,
                Layer = layer,
                Duration = Mathf.Max(duration, 0.001f),
                Delay = delay,
                Elapsed = 0f,
                DelayElapsed = 0f,
                FromWeight = fromWeight,
                ToWeight = toWeight,
                IsComplete = false,
                Mixer = mixer,
                OnComplete = onComplete
            });
            return _nextId - 1;
        }

        public void Cancel(int id)
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                if (_operations[i].Id != id)
                    continue;

                var op = _operations[i];
                op.IsComplete = true;
                _operations[i] = op;
                break;
            }
        }

        public int GetActiveCount() => _operations.Count;

        #endregion
    }
}
