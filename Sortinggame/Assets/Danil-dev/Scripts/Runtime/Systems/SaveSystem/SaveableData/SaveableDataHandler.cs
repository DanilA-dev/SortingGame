using System.IO;
using Cysharp.Threading.Tasks;
using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.SaveSystem.SaveableData
{
    public class SaveableDataHandler : MonoBehaviour
    {
        #region Fields

        [Title("Saveable Configs")]
        [SerializeReference] private BaseSaveableData[] _saveableDatas;
        [PropertySpace(15)]
        [SerializeField] private bool _debug;
        
        #endregion

        #region Monobehaviour

        private void Start() => LoadOnStart();
        private void OnDestroy() => SaveOnExit();
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

        public void LoadData()
        {
            foreach (var saveableData in _saveableDatas)
                Load(saveableData);
        }

        
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

        #endregion
        
        #region Private

        private void LoadOnStart()
        {
            foreach (var saveableData in _saveableDatas)
                if (saveableData.LoadOnStart)
                    Load(saveableData);
        }

        private void SaveOnExit()
        {
            foreach (var saveableData in _saveableDatas)
                if (saveableData.SaveOnExit)
                    Save(saveableData, true);
        }

        private void Save(BaseSaveableData data, bool immediate = false)
        {
            if (GlobalSaveService.Instance == null)
                return;

            if (immediate)
                GlobalSaveService.Instance.Save(data.Key.Value, data.SaveData());
            else
                GlobalSaveService.Instance.SaveAsync(data.Key.Value, data.SaveData()).Forget();

            if (_debug)
                Debug.Log($"[SaveableDataHandler] Save {data.Key.Value}");
        }

        private async void Load(BaseSaveableData data)
        {
            if (GlobalSaveService.Instance == null)
                return;
            
            var loaded = await GlobalSaveService.Instance.LoadAsync<object>(data.Key.Value, data.GetDefaultValue());
            
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
    }
}