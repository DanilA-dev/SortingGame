using UnityEngine;

namespace D_Dev.VATAnimationSystem
{
    [System.Serializable]
    public class VertexAnimationClipInfo
    {
        #region Fields

        public string Name;
        public int StartFrame;
        public int FrameCount;
        public int FPS;
        public float Duration;

        #endregion

        #region Properties

        public float EndFrame => StartFrame + FrameCount;
        public float FrameStep => Duration / Mathf.Max(1, FrameCount - 1);

        #endregion
    }
}