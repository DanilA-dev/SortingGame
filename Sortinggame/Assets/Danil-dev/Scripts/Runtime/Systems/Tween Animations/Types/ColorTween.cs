#if DOTWEEN
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace D_Dev.TweenAnimations.Types
{
    [Serializable]
    public class ColorTween : BaseAnimationTween
    {
        #region Enums

        public enum ColorObjectType
        {
            Image,
            Material,
            SpriteRenderer
        }

        #endregion

        #region Fields

        [SerializeField] private ColorObjectType _colorObjectType;
        [ShowIf("@this._colorObjectType == ColorObjectType.Image")]
        [SerializeField] private Image _image;
        [ShowIf("@this._colorObjectType == ColorObjectType.Material")]
        [SerializeField] private MeshRenderer _meshMaterial;
        [ShowIf("@this._colorObjectType == ColorObjectType.SpriteRenderer")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool _useInitialColorAsStart;
        [HideIf(nameof(_useInitialColorAsStart))]
        [SerializeField] private Color _startValue;
        [SerializeField] private Color _endValue;

        #endregion

        #region Properties

        public ColorObjectType ColorType
        {
            get => _colorObjectType;
            set => _colorObjectType = value;
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

        public SpriteRenderer SpriteRenderer
        {
            get => _spriteRenderer;
            set => _spriteRenderer = value;
        }

        public Color StartValue
        {
            get => _startValue;
            set => _startValue = value;
        }

        public Color EndValue
        {
            get => _endValue;
            set => _endValue = value;
        }

        public bool UseInitialColorAsStart
        {
            get => _useInitialColorAsStart;
            set => _useInitialColorAsStart = value;
        }

        #endregion

        #region Override

        public override Tween Play()
        {
            switch (_colorObjectType)
            {
                case ColorObjectType.Image:
                {
                    if (_image == null) return null;
                    SetTarget(_image.gameObject);
                    var startColor = _useInitialColorAsStart ? _image.color : _startValue;
                    return Tween = _image.DOColor(_endValue, Duration).From(startColor);
                }
                case ColorObjectType.Material:
                {
                    if (_meshMaterial == null) return null;
                    SetTarget(_meshMaterial.gameObject);
                    var material = _meshMaterial.sharedMaterial;
                    var startColor = _useInitialColorAsStart ? material.color : _startValue;
                    return Tween = material.DOColor(_endValue, Duration).From(startColor);
                }
                case ColorObjectType.SpriteRenderer:
                {
                    if (_spriteRenderer == null) return null;
                    SetTarget(_spriteRenderer.gameObject);
                    var startColor = _useInitialColorAsStart ? _spriteRenderer.color : _startValue;
                    return Tween = _spriteRenderer.DOColor(_endValue, Duration).From(startColor);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
#endif
