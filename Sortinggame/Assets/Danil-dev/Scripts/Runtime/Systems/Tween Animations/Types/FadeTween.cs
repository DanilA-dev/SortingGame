#if DOTWEEN
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace D_Dev.TweenAnimations.Types
{
    [Serializable]
    public class FadeTween : BaseAnimationTween
    {
        #region Enums

        public enum FadeObject
        {
            Image,
            CanvasGroup,
            Material,
            SpriteRenderer
        }

        #endregion

        #region Fields

        [SerializeField] private FadeObject _fadeObject;
        [ShowIf("@this._fadeObject == FadeObject.Image")]
        [SerializeField] private Image _image;
        [FormerlySerializedAs("_material")]
        [ShowIf("@this._fadeObject == FadeObject.Material")]
        [SerializeField] private MeshRenderer _meshMaterial;
        [ShowIf("@this._fadeObject == FadeObject.CanvasGroup")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [ShowIf("@this._fadeObject == FadeObject.SpriteRenderer")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [Space]
        [SerializeField] private float _startValue;
        [SerializeField] private float _endValue;

        #endregion

        #region Properties

        public FadeObject Fade
        {
            get => _fadeObject;
            set => _fadeObject = value;
        }

        public Image Image
        {
            get => _image;
            set => _image = value;
        }

        public MeshRenderer MeshMaterial
        {
            get => _meshMaterial;
            set => _meshMaterial = value;
        }

        public CanvasGroup Group
        {
            get => _canvasGroup;
            set => _canvasGroup = value;
        }

        public SpriteRenderer SpriteRenderer
        {
            get => _spriteRenderer;
            set => _spriteRenderer = value;
        }

        public float StartValue
        {
            get => _startValue;
            set => _startValue = value;
        }

        public float EndValue
        {
            get => _endValue;
            set => _endValue = value;
        }

        #endregion

        #region Override

        public override Tween Play()
        {
            switch (_fadeObject)
            {
                case FadeObject.Image:
                    if (_image == null) return null;
                    SetTarget(_image.gameObject);
                    return Tween = _image.DOFade(_endValue, Duration).From(_startValue);
                case FadeObject.CanvasGroup:
                    if (_canvasGroup == null) return null;
                    SetTarget(_canvasGroup.gameObject);
                    return Tween = _canvasGroup.DOFade(_endValue, Duration).From(_startValue);
                case FadeObject.Material:
                {
                    if (_meshMaterial == null) return null;
                    SetTarget(_meshMaterial.gameObject);
                    var material = _meshMaterial.sharedMaterial;
                    return Tween = material.DOFade(_endValue, Duration).From(_startValue);
                }
                case FadeObject.SpriteRenderer:
                    if (_spriteRenderer == null) return null;
                    SetTarget(_spriteRenderer.gameObject);
                    return Tween = _spriteRenderer.DOFade(_endValue, Duration).From(_startValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
#endif
