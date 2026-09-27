using D_Dev.EntitySpawner;
using D_Dev.PositionRotationConfig;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace D_Dev.CurrencySystem.Extensions
{
    public class CurrencyEntitySpawner : MonoBehaviour
    {
        #region Fields

        [Title("Data")]
        [SerializeField] private EntitySpawnSettings _currencySpawnSettings;
        [SerializeField] private int _maxAmount = 25;
        [SerializeReference] private BasePositionSettings _endPointPosition = new Vector3PositionSettings();
        [SerializeField] private Vector3 _endPointOffset;

        [Title("Jump Animation")]
        [SerializeField] private bool _useJumpAnimation = true;
        [ShowIf(nameof(_useJumpAnimation))]
        [SerializeField] private int _jumpTweenPower = 1;
        [ShowIf(nameof(_useJumpAnimation))]
        [SerializeField] private int _jumpTweenAmount = 1;
        [ShowIf(nameof(_useJumpAnimation))]
        [SerializeField] private float _jumpTime = 0.5f;
        [ShowIf(nameof(_useJumpAnimation))]
        [SerializeField] private Ease _jumpEase = Ease.OutQuad;

        [Title("Scale Punch Animation")]
        [SerializeField] private bool _useScalePunchAnimation = true;
        [ShowIf(nameof(_useScalePunchAnimation))]
        [SerializeField] private Vector3 _punchScale = new Vector3(0.4f, 0.4f, 0.4f);
        [ShowIf(nameof(_useScalePunchAnimation))]
        [SerializeField] private float _punchDuration = 0.3f;
        [ShowIf(nameof(_useScalePunchAnimation))]
        [SerializeField] private int _punchVibrato = 5;
        [ShowIf(nameof(_useScalePunchAnimation))]
        [SerializeField] private float _punchElasticity = 0.5f;

        [Title("Spawn Rotation")]
        [SerializeField] private bool _randomizeCurrencyRotation;
        [ShowIf(nameof(_randomizeCurrencyRotation))]
        [SerializeField] private Vector3 _minRandomRot;
        [ShowIf(nameof(_randomizeCurrencyRotation))]
        [SerializeField] private Vector3 _maxRandomRot;

        #endregion

        #region Public

        [FoldoutGroup("Events")]
        public UnityEvent<int> OnCurrencySpawned;

        public void SpawnCurrencies()
        {
            if (_currencySpawnSettings?.Amount != null)
                SpawnCurrenciesAsync(_currencySpawnSettings.Amount.Value, this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void SpawnCurrencies(int amount)
        {
            SpawnCurrenciesAsync(amount, this.GetCancellationTokenOnDestroy()).Forget();
        }

        #endregion

        #region Private

        private async UniTaskVoid SpawnCurrenciesAsync(int amount, CancellationToken token)
        {
            if (amount <= 0 || _currencySpawnSettings == null)
                return;

            int maxSpawns = Mathf.Min(amount, _maxAmount);
            int baseAmount = amount / maxSpawns;
            int extraAmount = amount % maxSpawns;

            for (int i = 0; i < maxSpawns; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                CurrencyInfoSetter currencyInfoSetter = null;

                if (_currencySpawnSettings.GlobalPool != null)
                {
                    currencyInfoSetter = await _currencySpawnSettings.GlobalPool.Get<CurrencyInfoSetter>(_currencySpawnSettings.Data.Value);
                }
                else
                {
                    var go = await _currencySpawnSettings.Get();
                    if (go != null) go.TryGetComponent(out currencyInfoSetter);
                }

                if (currencyInfoSetter == null)
                    continue;

                Transform currencyTransform = currencyInfoSetter.transform;
                currencyTransform.rotation = Quaternion.Euler(GetCurrencyRotation());
                currencyTransform.position = _currencySpawnSettings.PositionSettings.GetPosition();

                int entityAmount = baseAmount + (i < extraAmount ? 1 : 0);
                currencyInfoSetter.SetAmount(entityAmount);

                if (_useScalePunchAnimation)
                {
                    currencyTransform
                        .DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity)
                        .SetLink(currencyTransform.gameObject);
                }

                if (_useJumpAnimation)
                {
                    Vector3 targetPos = _endPointPosition.GetPosition() + _endPointOffset;

                    currencyTransform
                        .DOJump(targetPos, _jumpTweenPower, _jumpTweenAmount, _jumpTime)
                        .SetEase(_jumpEase)
                        .SetLink(currencyTransform.gameObject);
                }
            }

            OnCurrencySpawned?.Invoke(amount);
        }

        private Vector3 GetCurrencyRotation()
        {
            if (!_randomizeCurrencyRotation)
                return Vector3.zero;

            return new Vector3(
                Random.Range(_minRandomRot.x, _maxRandomRot.x),
                Random.Range(_minRandomRot.y, _maxRandomRot.y),
                Random.Range(_minRandomRot.z, _maxRandomRot.z)
            );
        }

        #endregion
    }
}
