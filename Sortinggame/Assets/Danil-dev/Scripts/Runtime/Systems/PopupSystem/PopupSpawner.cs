#if DOTWEEN
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.EntityPool;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.PopupSystem
{
    public class PopupSpawner : MonoBehaviour
    {
        #region Fields

        [Title("Pool")]
        [SerializeField, Required] private PoolableDataList _pool;
        [SerializeReference] private PolymorphicValue<EntityInfo> _popupInfo = new EntityInfoConstantValue();
        [Title("Placement")]
        [SerializeField] private Transform _target;
        [SerializeField] private bool _followTarget = true;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _randomOffset;
        [Title("Entity Popup")]
        [SerializeField] private string _entityFormat = "+{0} {1}";
        [SerializeField] private StringScriptableVariable _nameVariableId;
        [SerializeField] private StringScriptableVariable _iconVariableId;

        #endregion

        #region Monobehaviour

        private void Reset() => _target = transform;

        #endregion

        #region Public

        public void Show(string text) => ShowAsync(new PopupData(text)).Forget();

        public void Show(PopupData data) => ShowAsync(data).Forget();

        public void Show(EntityInfo info, int amount)
        {
            if (info == null)
                return;

            if (!info.TryGetVariableValue(_nameVariableId, out string entityName))
                entityName = info.name;

            info.TryGetVariableValue(_iconVariableId, out Sprite icon);

            Show(new PopupData(string.Format(_entityFormat, amount, entityName), icon));
        }

        public async UniTask<PopupView> ShowAsync(PopupData data)
        {
            if (_pool == null || _popupInfo?.Value == null)
                return null;

            var view = await _pool.Get<PopupView>(_popupInfo.Value);
            if (view == null)
                return null;

            var target = _target != null ? _target : transform;
            var offset = _offset + GetRandomOffset();

            if (_followTarget)
                view.Show(data, target, offset);
            else
                view.Show(data, target.position + offset);

            return view;
        }

        #endregion

        #region Private

        private Vector3 GetRandomOffset()
        {
            return new Vector3(
                Random.Range(-_randomOffset.x, _randomOffset.x),
                Random.Range(-_randomOffset.y, _randomOffset.y),
                Random.Range(-_randomOffset.z, _randomOffset.z));
        }

        #endregion
    }
}
#endif
