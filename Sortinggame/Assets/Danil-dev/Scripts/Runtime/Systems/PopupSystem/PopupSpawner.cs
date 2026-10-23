#if DOTWEEN
using System.Globalization;
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.EntitySpawner;
using D_Dev.Extensions;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.PopupSystem
{
    public class PopupSpawner : MonoBehaviour
    {
        #region Fields

        [Title("Spawn Settings")]
        [SerializeField] private EntitySpawnSettings _spawnSettings;
        [Title("Entity Popup")]
        [SerializeField] private string _entityFormat = "+{0} {1}";
        [SerializeField] private StringScriptableVariable _nameVariableId;
        [SerializeField] private StringScriptableVariable _iconVariableId;
        [Title("Amount Popup")]
        [SerializeField] private string _amountFormat = "+{0}";
        [SerializeField] private Sprite _amountIcon;
        [SerializeField] private bool _shortAmount = true;

        #endregion

        #region Public

        public void Show(string text) => ShowAsync(new PopupData(text)).Forget();

        public void ShowAmount(int amount) => ShowAmount((long)amount);

        public void ShowAmount(long amount)
        {
            var amountText = _shortAmount ? amount.ToShortString() : amount.ToString();
            ShowAmountText(amountText);
        }

        public void ShowAmount(float amount) => ShowAmount((double)amount);

        public void ShowAmount(double amount)
        {
            var amountText = _shortAmount
                ? amount.ToShortString()
                : amount.ToString("0.##", CultureInfo.InvariantCulture);
            ShowAmountText(amountText);
        }

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
            if (_spawnSettings == null)
                return null;

            var go = await _spawnSettings.Get();
            if (go == null)
                return null;

            var view = go.GetComponent<PopupView>();
            if (view == null)
                return null;

            var pos = _spawnSettings.PositionSettings.GetPosition();
            var rot = _spawnSettings.RotationSettings.GetRotation();

            view.Show(data, pos, rot);

            return view;
        }

        #endregion

        #region Private

        private void ShowAmountText(string amountText)
        {
            Show(new PopupData(string.Format(_amountFormat, amountText), _amountIcon));
        }


        #endregion
    }
}
#endif
