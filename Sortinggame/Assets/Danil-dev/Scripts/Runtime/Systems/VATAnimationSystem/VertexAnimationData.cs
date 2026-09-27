using UnityEngine;

namespace D_Dev.VATAnimationSystem
{
    [CreateAssetMenu(menuName = "D-Dev/VAT Data")]
    public class VertexAnimationData : ScriptableObject
    {
        #region Fields
 
        public string ModelName;
        public int TotalFrames;
        public int VertexCount;
        public Vector3 BoundsMin;
        public Vector3 BoundsSize;
        public VertexAnimationClipInfo[] Clips;
        public Texture2D VATTexture;
        public Texture2D BoneTexture;
        public string[] BoneNames;
        public Mesh BaseMesh;
 
        #endregion
 
        #region Public
 
        public bool TryGetClip(string clipName, out VertexAnimationClipInfo info)
        {
            if (Clips == null)
            { 
                info = null;
                return false;
            }

            foreach (var c in Clips)
            {
                if (c.Name == clipName)
                {
                    info = c; 
                    return true;
                }
            }
            
            info = null;
            return false;
        }
 
        public int GetBoneIndex(string boneName)
        {
            if (BoneNames == null)
                return -1;
            
            for (int i = 0; i < BoneNames.Length; i++)
                if (BoneNames[i] == boneName)
                    return i;
            
            return -1;
        }
 
        #endregion
    }
}