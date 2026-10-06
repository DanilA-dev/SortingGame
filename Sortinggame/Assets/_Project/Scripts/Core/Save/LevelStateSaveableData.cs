using System;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace _Project.Scripts
{
    [Serializable]
    public class LevelStateSaveableData : BaseSaveableData<LevelSaveData>
    {
        #region Fields

        [SerializeField] private LevelStateController _controller;

        #endregion

        #region Properties

        public override bool CanSave => _controller != null && _controller.CanCapture;

        #endregion

        #region Overrides

        public override void Subscribe()
        {
            if (_controller != null)
                _controller.OnStateChanged += NotifyChanged;
        }

        public override void Unsubscribe()
        {
            if (_controller != null)
                _controller.OnStateChanged -= NotifyChanged;
        }

        protected override LevelSaveData GetTypedSaveData() => _controller.Capture();

        protected override void SetTypedSaveData(LevelSaveData data)
        {
            if (_controller != null)
                _controller.SetLoadedData(data);
        }

        protected override LevelSaveData GetTypedDefaultValue() => new();

        #endregion
    }
}
