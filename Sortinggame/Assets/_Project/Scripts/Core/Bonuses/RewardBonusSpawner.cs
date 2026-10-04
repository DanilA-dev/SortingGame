using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts
{
    public class RewardBonusSpawner : MonoBehaviour
    {
        #region Fields

        [Title("Spawn")]
        [SerializeField] private Transform[] _points;
        [SerializeReference] private PolymorphicValue<float> _delay = new FloatConstantValue();
        [Title("Rewards")]
        [SerializeField] private GameObject[] _rewardPrefabs;
        [SerializeField] private Transform _parent;

        private List<GameObject> _rewards = new();
        private List<GameObject> _inactiveRewards = new();

        #endregion

        #region Monobehaviour

        private void Start()
        {
            CreateRewards();
            SpawnLoopAsync(destroyCancellationToken).Forget();
        }

        #endregion

        #region Private

        private void CreateRewards()
        {
            if (_rewardPrefabs == null)
                return;

            foreach (var prefab in _rewardPrefabs)
            {
                if (prefab == null)
                    continue;

                var reward = Instantiate(prefab, _parent);
                reward.SetActive(false);
                _rewards.Add(reward);
            }
        }

        private async UniTaskVoid SpawnLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(Mathf.Max(0f, _delay.Value));
                if (await UniTask.Delay(delay, cancellationToken: token).SuppressCancellationThrow())
                    return;

                TryShowRandomReward();
            }
        }

        private void TryShowRandomReward()
        {
            if (_points == null || _points.Length == 0)
                return;

            _inactiveRewards.Clear();
            foreach (var reward in _rewards)
            {
                if (reward != null && !reward.activeSelf)
                    _inactiveRewards.Add(reward);
            }

            if (_inactiveRewards.Count == 0)
                return;

            var point = _points[Random.Range(0, _points.Length)];
            if (point == null)
                return;

            var selectedReward = _inactiveRewards[Random.Range(0, _inactiveRewards.Count)];
            selectedReward.transform.SetPositionAndRotation(point.position, point.rotation);
            selectedReward.SetActive(true);
        }

        #endregion
    }
}
