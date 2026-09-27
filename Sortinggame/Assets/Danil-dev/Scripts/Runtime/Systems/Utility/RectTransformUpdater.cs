using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Utility
{
    public class RectTransformUpdater : MonoBehaviour
    {
        #region Classes

        [Serializable]
        public class RectPreset
        {
            [HorizontalGroup("Header", Width = 40)]
            [HideLabel]
            public int Index;
            [HorizontalGroup("Header")]
            [HideLabel]
            public string Name;

            public RectTransform Target;

            public bool ApplyOnStart;

            [FoldoutGroup("Size")]
            [HorizontalGroup("Size/Width")]
            [LabelWidth(80)] public bool ApplyWidth;
            [HorizontalGroup("Size/Width"), EnableIf(nameof(ApplyWidth))]
            [HideLabel] public float Width;

            [FoldoutGroup("Size")]
            [HorizontalGroup("Size/Height")]
            [LabelWidth(80)] public bool ApplyHeight;
            [HorizontalGroup("Size/Height"), EnableIf(nameof(ApplyHeight))]
            [HideLabel] public float Height;

            [FoldoutGroup("Position")]
            [HorizontalGroup("Position/X")]
            [LabelWidth(80)] public bool ApplyPositionX;
            [HorizontalGroup("Position/X"), EnableIf(nameof(ApplyPositionX))]
            [HideLabel] public float PositionX;

            [FoldoutGroup("Position")]
            [HorizontalGroup("Position/Y")]
            [LabelWidth(80)] public bool ApplyPositionY;
            [HorizontalGroup("Position/Y"), EnableIf(nameof(ApplyPositionY))]
            [HideLabel] public float PositionY;

            [FoldoutGroup("Position")]
            [HorizontalGroup("Position/Z")]
            [LabelWidth(80)] public bool ApplyPositionZ;
            [HorizontalGroup("Position/Z"), EnableIf(nameof(ApplyPositionZ))]
            [HideLabel] public float PositionZ;

            [FoldoutGroup("Anchors")]
            [HorizontalGroup("Anchors/Min")]
            [LabelWidth(80)] public bool ApplyAnchorMin;
            [HorizontalGroup("Anchors/Min"), EnableIf(nameof(ApplyAnchorMin))]
            [HideLabel] public Vector2 AnchorMin;

            [FoldoutGroup("Anchors")]
            [HorizontalGroup("Anchors/Max")]
            [LabelWidth(80)] public bool ApplyAnchorMax;
            [HorizontalGroup("Anchors/Max"), EnableIf(nameof(ApplyAnchorMax))]
            [HideLabel] public Vector2 AnchorMax;

            [FoldoutGroup("Pivot")]
            [HorizontalGroup("Pivot/Value")]
            [LabelWidth(80)] public bool ApplyPivot;
            [HorizontalGroup("Pivot/Value"), EnableIf(nameof(ApplyPivot))]
            [HideLabel] public Vector2 Pivot;

            [FoldoutGroup("Offsets")]
            [HorizontalGroup("Offsets/Left")]
            [LabelWidth(80)] public bool ApplyLeft;
            [HorizontalGroup("Offsets/Left"), EnableIf(nameof(ApplyLeft))]
            [HideLabel] public float Left;

            [FoldoutGroup("Offsets")]
            [HorizontalGroup("Offsets/Right")]
            [LabelWidth(80)] public bool ApplyRight;
            [HorizontalGroup("Offsets/Right"), EnableIf(nameof(ApplyRight))]
            [HideLabel] public float Right;

            [FoldoutGroup("Offsets")]
            [HorizontalGroup("Offsets/Top")]
            [LabelWidth(80)] public bool ApplyTop;
            [HorizontalGroup("Offsets/Top"), EnableIf(nameof(ApplyTop))]
            [HideLabel] public float Top;

            [FoldoutGroup("Offsets")]
            [HorizontalGroup("Offsets/Bottom")]
            [LabelWidth(80)] public bool ApplyBottom;
            [HorizontalGroup("Offsets/Bottom"), EnableIf(nameof(ApplyBottom))]
            [HideLabel] public float Bottom;
        }

        #endregion

        #region Fields

        [Title("Presets")]
        [SerializeField] private RectPreset[] _presets;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            foreach (var preset in _presets)
            {
                if (preset.ApplyOnStart)
                    ApplyPreset(preset);
            }
        }

        #endregion

        #region Public

        public void ApplyPreset(int index)
        {
            foreach (var preset in _presets)
            {
                if (preset.Index == index)
                {
                    ApplyPreset(preset);
                    return;
                }
            }

            Debug.Log($"[RectTransformUpdater] Preset with index {index} not found.", this);
        }

        #endregion

        #region Private

        private void ApplyPreset(RectPreset preset)
        {
            if (preset.Target == null)
            {
                Debug.Log($"[RectTransformUpdater] Preset '{preset.Name}' has no Target assigned.", this);
                return;
            }

            if (preset.ApplyAnchorMin)
                preset.Target.anchorMin = preset.AnchorMin;

            if (preset.ApplyAnchorMax)
                preset.Target.anchorMax = preset.AnchorMax;

            if (preset.ApplyPivot)
                preset.Target.pivot = preset.Pivot;

            if (preset.ApplyWidth)
                preset.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preset.Width);

            if (preset.ApplyHeight)
                preset.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preset.Height);

            if (preset.ApplyPositionX || preset.ApplyPositionY || preset.ApplyPositionZ)
            {
                var pos = preset.Target.anchoredPosition3D;
                if (preset.ApplyPositionX)
                    pos.x = preset.PositionX;
                
                if (preset.ApplyPositionY)
                    pos.y = preset.PositionY;
                
                if (preset.ApplyPositionZ) 
                    pos.z = preset.PositionZ;

                preset.Target.anchoredPosition3D = pos;
            }

            if (preset.ApplyLeft || preset.ApplyBottom)
            {
                var offsetMin = preset.Target.offsetMin;
                if (preset.ApplyLeft)
                    offsetMin.x = preset.Left;
                
                if (preset.ApplyBottom)
                    offsetMin.y = preset.Bottom;
                
                preset.Target.offsetMin = offsetMin;
            }

            if (preset.ApplyRight || preset.ApplyTop)
            {
                var offsetMax = preset.Target.offsetMax;
                if (preset.ApplyRight)
                    offsetMax.x = -preset.Right;
                
                if (preset.ApplyTop)
                    offsetMax.y = -preset.Top;
                
                preset.Target.offsetMax = offsetMax;
            }
        }

        #endregion
    }
}