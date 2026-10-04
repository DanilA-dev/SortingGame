using System;
using System.Collections;
using D_Dev.CoroutineManagerSystem;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    [CreateAssetMenu(menuName = "Game/Bonuses/Timed Bonus")]
    public class TimedBonusInfo : ScriptableObject
    {
        #region Fields

        [Title("Multiplier")]
        [SerializeReference] private PolymorphicValue<float> _multiplier = new FloatConstantValue();
        [SerializeField] private float _activeMultiplier = 2f;
        [SerializeField] private float _defaultMultiplier = 1f;
        [Title("Duration")]
        [SerializeField, Min(0f), SuffixLabel("sec")] private float _duration = 120f;
        [SerializeField] private bool _useUnscaledTime;
        [SerializeReference] private PolymorphicValue<float> _remainingTime = new FloatConstantValue();

        private Coroutine _routine;

        public event Action OnActivated;
        public event Action OnExpired;

        #endregion

        #region Properties

        [ShowInInspector, ReadOnly]
        public bool IsActive => _routine != null;

        [ShowInInspector, ReadOnly]
        public float RemainingTime { get; private set; }

        public float Duration => _duration;

        #endregion

        #region ScriptableObject

        private void OnEnable()
        {
            Application.quitting += ResetBonus;
            ResetBonus();
        }

        private void OnDisable()
        {
            Application.quitting -= ResetBonus;
            ResetBonus();
        }

        #endregion

        #region Public

        [Button]
        public void Activate()
        {
            CoroutineManager.Stop(_routine);
            _routine = null;
            _routine = CoroutineManager.Run(BonusRoutine());
            OnActivated?.Invoke();
        }

        [Button]
        public void ResetBonus()
        {
            CoroutineManager.Stop(_routine);
            _routine = null;
            SetRemainingTime(0f);
            SetMultiplier(_defaultMultiplier);
        }

        #endregion

        #region Private

        private void SetMultiplier(float multiplier)
        {
            if (_multiplier != null)
                _multiplier.Value = multiplier;
        }

        private void SetRemainingTime(float time)
        {
            RemainingTime = time;

            if (_remainingTime != null)
                _remainingTime.Value = time;
        }

        #endregion

        #region Coroutines

        private IEnumerator BonusRoutine()
        {
            SetMultiplier(_activeMultiplier);
            SetRemainingTime(_duration);

            while (RemainingTime > 0f)
            {
                yield return null;
                var delta = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                SetRemainingTime(Mathf.Max(0f, RemainingTime - delta));
            }

            _routine = null;
            SetMultiplier(_defaultMultiplier);
            OnExpired?.Invoke();
        }

        #endregion
    }
}
