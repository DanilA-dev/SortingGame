using System.Collections;
using System.Collections.Generic;
using D_Dev.PolymorphicValueSystem;
using D_Dev.PositionRotationConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.EntityPuller
{
    public class EntityPuller : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private BasePositionSettings _targetPoint = new Vector3PositionSettings();
        [SerializeReference] private PolymorphicValue<float> _pullDuration = new FloatConstantValue();
        [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0,0,1,1);
        
        private static readonly HashSet<PullableEntity> _subscribers = new();

        #endregion

        #region Static

        public static void Subscribe(PullableEntity p) => _subscribers.Add(p);
        public static void Unsubscribe(PullableEntity p) => _subscribers.Remove(p);

        #endregion

        #region Public

        [Button]
        public void PullAll()
        {
            foreach (var p in _subscribers)
            {
                if (p == null)
                    continue;

                StartCoroutine(PullRoutine(p));
            }
        }

        #endregion

        #region Coroutine

        private IEnumerator PullRoutine(PullableEntity p)
        {
            Transform tr = p.transform;
            Vector3 start = tr.position;
            float t = 0f;
            while (t < _pullDuration.Value && p != null)
            {
                t += Time.deltaTime;
                float k = _curve.Evaluate(Mathf.Clamp01(t / _pullDuration.Value));
                tr.position = Vector3.LerpUnclamped(start, _targetPoint.GetPosition(), k);
                yield return null;
            }
            if (p != null)
            {
                tr.position = _targetPoint.GetPosition();
                p?.OnPulledCallback();
            }
        }

        #endregion
    }
}