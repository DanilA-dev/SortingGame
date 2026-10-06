using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.SaveSystem.SaveableData
{
    public class SaveableDataHandler : MonoBehaviour
    {
        #region Fields

        [Title("Saveable Configs")]
        [SerializeReference] private BaseSaveableData[] _saveableDatas;
        [Title("Save On Change")]
        [SerializeField] private bool _saveOnChange;
        [ShowIf(nameof(_saveOnChange)), SuffixLabel("sec")]
        [SerializeField, Min(0f)] private float _saveOnChangeInterval = 2f;
        [PropertySpace(15)]
        [SerializeField] private bool _debug;
        [FoldoutGroup("Events"), PropertyOrder(100)]
        public UnityEvent OnLoaded;

        private readonly List<BaseSaveableData> _changedDatas = new();
        private readonly List<BaseSaveableData> _savingDatas = new();
        private bool _isSavingLocked;
        private bool _isSubscribed;
        private bool _isSaveScheduled;

        #endregion

        #region Properties

        public bool IsLoaded { get; private set; }

        #endregion

        #region Monobehaviour

        private void Start() => LoadAllAsync(true).Forget();
        private void OnDestroy()
        {
            UnsubscribeAll();
            SaveOnExit();
        }
        private void OnApplicationFocus(bool hasFocus)
        {
            if(!hasFocus)
                SaveOnExit();
        }

        private void OnApplicationQuit() => SaveOnExit();

        #endregion

        #region Public

        public void SaveData()
        {
            foreach (var saveableData in _saveableDatas)
                Save(saveableData);
        }

        public void LoadData() => LoadAllAsync(false).Forget();

        
        [Button]
        public void DeleteData(string key)
        {
            foreach (var saveableData in _saveableDatas)
            {
                if(saveableData.Key.Value == key)
                   DeleteKey(saveableData.Key.Value);
            }
        }
        
        [Button]
        public void DeleteData(PolymorphicValue<string> key)
        {
            foreach (var saveableData in _saveableDatas)
            {
                if(saveableData.Key.Value == key.Value)
                    DeleteKey(saveableData.Key.Value);
            }
        }
        
        [Button]
        public void DeleteAll()
        {
            foreach (var saveableData in _saveableDatas)
                DeleteKey(saveableData.Key.Value);
        }

        public void ResetAll(bool lockSaving = true)
        {
            _isSavingLocked = lockSaving;

            foreach (var saveableData in _saveableDatas)
            {
                if (saveableData == null)
                    continue;

                saveableData.LoadData(saveableData.GetDefaultValue());
                DeleteKey(saveableData.Key.Value);
            }
        }

        #endregion
        
        #region Private

        private async UniTaskVoid LoadAllAsync(bool onlyLoadOnStart)
        {
            IsLoaded = false;

            var tasks = new List<UniTask>();
            if (_saveableDatas != null)
            {
                foreach (var saveableData in _saveableDatas)
                    if (saveableData != null && (!onlyLoadOnStart || saveableData.LoadOnStart))
                        tasks.Add(LoadAsync(saveableData));
            }

            await UniTask.WhenAll(tasks);

            if (this == null)
                return;

            IsLoaded = true;

            if (_saveOnChange)
                SubscribeAll();

            OnLoaded?.Invoke();

            if (_debug)
                Debug.Log("[SaveableDataHandler] All data loaded");
        }

        private void SaveOnExit()
        {
            _changedDatas.Clear();

            foreach (var saveableData in _saveableDatas)
                if (saveableData.SaveOnExit)
                    Save(saveableData, true);
        }

        private void SubscribeAll()
        {
            if (_isSubscribed || _saveableDatas == null)
                return;

            _isSubscribed = true;
            foreach (var saveableData in _saveableDatas)
            {
                if (saveableData == null)
                    continue;

                saveableData.OnChanged += OnDataChanged;
                saveableData.Subscribe();
            }
        }

        private void UnsubscribeAll()
        {
            if (!_isSubscribed || _saveableDatas == null)
                return;

            _isSubscribed = false;
            foreach (var saveableData in _saveableDatas)
            {
                if (saveableData == null)
                    continue;

                saveableData.Unsubscribe();
                saveableData.OnChanged -= OnDataChanged;
            }
        }

        private void SaveChanged()
        {
            _savingDatas.AddRange(_changedDatas);
            _changedDatas.Clear();

            foreach (var saveableData in _savingDatas)
                Save(saveableData);

            _savingDatas.Clear();
        }

        private async UniTaskVoid SaveChangedDelayed()
        {
            _isSaveScheduled = true;

            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(_saveOnChangeInterval), DelayType.UnscaledDeltaTime,
                    cancellationToken: destroyCancellationToken)
                .SuppressCancellationThrow();

            _isSaveScheduled = false;

            if (!isCanceled)
                SaveChanged();
        }

        private void Save(BaseSaveableData data, bool immediate = false)
        {
            if (_isSavingLocked || GlobalSaveService.Instance == null || !data.CanSave)
                return;

            if (immediate)
                GlobalSaveService.Instance.Save(data.Key.Value, data.SaveData());
            else
                GlobalSaveService.Instance.SaveAsync(data.Key.Value, data.SaveData()).Forget();

            if (_debug)
                Debug.Log($"[SaveableDataHandler] Save {data.Key.Value}");
        }

        private async UniTask LoadAsync(BaseSaveableData data)
        {
            if (GlobalSaveService.Instance == null)
                return;

            var loaded = await GlobalSaveService.Instance.LoadAsync<object>(data.Key.Value, data.GetDefaultValue());

            if (this == null)
                return;

            if (loaded != null)
                data.LoadData(loaded);
            
            if (_debug)
                Debug.Log($"[SaveableDataHandler] Load {data.Key.Value}");
        }

        private void DeleteKey(string key)
        {
            if (!Application.isPlaying)
            {
                if (PlayerPrefs.HasKey(key))
                    PlayerPrefs.DeleteKey(key);
            
                string path = Path.Combine(Application.persistentDataPath, $"{key}.json");
                if (File.Exists(path))
                    File.Delete(path);
            }
            
            if(Application.isPlaying)
                GlobalSaveService.Instance.DeleteKeyAsync(key);
        }

        #endregion

        #region Listeners

        private void OnDataChanged(BaseSaveableData data)
        {
            if (_isSavingLocked || _changedDatas.Contains(data))
                return;

            _changedDatas.Add(data);

            if (!_isSaveScheduled)
                SaveChangedDelayed().Forget();
        }

        #endregion
    }
}