using System;
using Cysharp.Threading.Tasks;

namespace D_Dev.SaveSystem
{
    public interface ISaveConfig
    {
        public void Save<T>(string key, T value);
        public UniTask SaveAsync<T>(string key, T value);
        public UniTask<T> LoadAsync<T>(string key, T defaultValue = default);
        public UniTask<bool> HasKeyAsync(string key);
        public UniTask DeleteKeyAsync(string key);
        public UniTask DeleteAllAsync();
    }
}
