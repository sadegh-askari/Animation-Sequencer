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

        [SerializeField] private string _propertyName;
        [SerializeField] private Color _color;

        private int _propertyId;
        private Renderer _targetRenderer;
        private Color _previousColor;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            _propertyId = Shader.PropertyToID(_propertyName);
            if (_targetRenderer == null)
            {
                _targetRenderer = target.GetComponent<Renderer>();
                if (_targetRenderer == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            _previousColor = _targetRenderer.material.color;
            TweenerCore<Color, Color, ColorOptions> materialTween = _targetRenderer.material.DOColor(_color, _propertyId, duration);
            return materialTween;
        }

        public override void ResetToInitialState()
        {
            if (_targetRenderer == null)
                return;

            _targetRenderer.material.SetColor(_propertyId, _previousColor);
        }
    }
}