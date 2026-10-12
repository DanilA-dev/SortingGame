using System;
using UnityEngine;

namespace D_Dev.PopupSystem
{
    [Serializable]
    public struct PopupData
    {
        #region Fields

        public string Text;
        public Sprite Icon;

        #endregion

        #region Constructor

        public PopupData(string text, Sprite icon = null)
        {
            Text = text;
            Icon = icon;
        }

        #endregion
    }
}
