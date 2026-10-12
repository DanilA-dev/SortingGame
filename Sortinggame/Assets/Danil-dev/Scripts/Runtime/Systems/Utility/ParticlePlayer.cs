using UnityEngine;

namespace D_Dev.Utility
{
    public class ParticlePlayer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private Vector3 _positionOffset;
        [SerializeField] private bool _withChildren = true;
        [SerializeField] private bool _clearOnPlay = true;

        #endregion

        #region Properties

        public ParticleSystem Particle => _particle;

        #endregion

        #region Monobehaviour

        private void Reset()
        {
            _particle = GetComponent<ParticleSystem>();
        }

        #endregion

        #region Public

        public void Play()
        {
            if (_particle == null)
                return;

            Play(_particle.transform.position, _particle.transform.rotation);
        }

        public void PlayAt(Vector3 position)
        {
            if (_particle == null)
                return;

            Play(position, _particle.transform.rotation);
        }

        public void PlayAt(Vector3 position, Vector3 normal)
        {
            if (_particle == null)
                return;

            var rotation = normal.sqrMagnitude > 0f
                ? Quaternion.LookRotation(normal)
                : _particle.transform.rotation;
            Play(position, rotation);
        }

        public void PlayAt(Vector3 position, Quaternion rotation) => Play(position, rotation);

        public void PlayAt(Transform target)
        {
            if (target == null)
                return;

            Play(target.position, target.rotation);
        }

        public void PlayAt(GameObject target)
        {
            if (target == null)
                return;

            PlayAt(target.transform);
        }

        public void PlayAt(Collider target)
        {
            if (target == null)
                return;

            PlayAt(target.transform);
        }

        public void Stop()
        {
            if (_particle == null)
                return;

            _particle.Stop(_withChildren, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        #endregion

        #region Private

        private void Play(Vector3 position, Quaternion rotation)
        {
            if (_particle == null)
                return;

            _particle.transform.SetPositionAndRotation(position + _positionOffset, rotation);

            if (_clearOnPlay)
                _particle.Stop(_withChildren, ParticleSystemStopBehavior.StopEmittingAndClear);

            _particle.Play(_withChildren);
        }

        #endregion
    }
}
