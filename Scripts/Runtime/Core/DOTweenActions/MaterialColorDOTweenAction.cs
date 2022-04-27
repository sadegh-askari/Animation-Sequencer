using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class MaterialColorDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Renderer);
        public override string DisplayName => "Material Color";

        public string PropertyName = "_AdditiveColor";
        [SerializeField] private Color _color;

        private int _propertyId;
        private Renderer _targetRenderer;
        private Color _previousColor;
        [SerializeField] private Material _material;
        [SerializeField] private Material _sharedMaterial;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            _propertyId = Shader.PropertyToID(PropertyName);
            if (_targetRenderer == null)
            {
                _targetRenderer = target.GetComponent<Renderer>();
                if (_targetRenderer == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            if (_sharedMaterial == null)
                _sharedMaterial = _targetRenderer.sharedMaterial;
            
            _material = _targetRenderer.sharedMaterial;
            if (Application.isPlaying)
                _material = _targetRenderer.material;

            if (_material.HasColor(_propertyId))
            {
                _previousColor = _material.GetColor(_propertyId);
                TweenerCore<Color, Color, ColorOptions> materialTween = _material.DOColor(_color, _propertyId, duration);
                materialTween.OnComplete(() =>
                {
                    _targetRenderer.material = _sharedMaterial;
                });

                materialTween.OnStart(() =>
                {
                    _targetRenderer.material = _material;
                });
                
                _targetRenderer.material = _sharedMaterial;
                return materialTween;
            }

            return null;
        }

        public override void ResetToInitialState()
        {
            if (_targetRenderer == null)
                return;
            _material = _targetRenderer.sharedMaterial;
            if (Application.isPlaying)
                _material = _targetRenderer.material;
            _material.SetColor(_propertyId, _previousColor);

            _targetRenderer.material = _sharedMaterial;
        }
    }
}