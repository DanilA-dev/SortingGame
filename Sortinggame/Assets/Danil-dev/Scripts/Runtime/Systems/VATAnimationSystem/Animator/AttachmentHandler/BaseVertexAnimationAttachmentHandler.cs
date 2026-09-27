using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.VATAnimationSystem
{
    public abstract class BaseVertexAnimationAttachmentHandler : MonoBehaviour
    {
        #region Classes

        [System.Serializable]
        public class BoneAttachment
        {
            public string BoneName;
            public Transform AttachRoot;

            public int BoneIndex { get; set; }
        }

        #endregion

        #region Fields

        [SerializeField] private VertexAnimationData _data;
        [SerializeField] private Transform _unitRoot;
        [SerializeField] private bool _useInstancedAnimator;
        [HideIf(nameof(_useInstancedAnimator))]
        [SerializeField] private BaseVertexAnimator _animator;
        [ShowIf(nameof(_useInstancedAnimator))]
        [SerializeField] private VertexAnimatorInstanced _instancedAnimator;
        [SerializeField] private List<BoneAttachment> _attachments = new();

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_data == null)
                return;

            foreach (var attachment in _attachments)
                attachment.BoneIndex = _data.GetBoneIndex(attachment.BoneName);
        }

        #endregion

        #region Public

        public void UpdateAttachments()
        {
            if (_data == null || _data.BoneTexture == null)
                return;

            VertexAnimationClipInfo clip;
            float normalizedTime;

            if (_useInstancedAnimator)
            {
                if (_instancedAnimator == null || _instancedAnimator.CurrentClip == null)
                    return;

                clip = _instancedAnimator.CurrentClip;
                normalizedTime = _instancedAnimator.NormalizedTime;
            }
            else
            {
                if (_animator == null || _animator.CurrentClip == null)
                    return;

                clip = _animator.CurrentClip;
                normalizedTime = _animator.NormalizedTime;
            }

            int frame = GetCurrentFrame(clip, normalizedTime);

            foreach (var attachment in _attachments)
            {
                if (attachment.AttachRoot == null || attachment.BoneIndex < 0) continue;

                ReadBoneTexture(attachment.BoneIndex, frame, out Vector3 localPos, out Quaternion localRot);

                if (_unitRoot != null)
                {
                    attachment.AttachRoot.position = _unitRoot.TransformPoint(localPos);
                    attachment.AttachRoot.rotation = _unitRoot.rotation * localRot;
                }
                else
                {
                    attachment.AttachRoot.position = localPos;
                    attachment.AttachRoot.rotation = localRot;
                }
            }
        }

        #endregion

        #region Private

        private int GetCurrentFrame(VertexAnimationClipInfo clip, float normalizedTime)
        {
            float t = Mathf.Clamp01(normalizedTime);
            return Mathf.RoundToInt(clip.StartFrame + t * (clip.FrameCount - 1));
        }

        private void ReadBoneTexture(int boneIndex, int frame, out Vector3 pos, out Quaternion rot)
        {
            int texX = boneIndex * 2;
            Color posData = _data.BoneTexture.GetPixel(texX, frame);
            Color rotData = _data.BoneTexture.GetPixel(texX + 1, frame);

            pos = new Vector3(posData.r, posData.g, posData.b);
            rot = new Quaternion(rotData.r, rotData.g, rotData.b, rotData.a);
        }

        #endregion
    }
}