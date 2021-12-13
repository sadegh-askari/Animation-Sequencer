using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
namespace BrunoMikoski.AnimationSequencer
{
    public class SwapSpriteDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(SpriteRenderer);
        public override string DisplayName => "Swap Sprite";

        [SerializeField] private Sprite _sprite;

        private SpriteRenderer _targetRenderer;
        private Sprite _previousSprite;

        private float _tempFloat;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (_targetRenderer == null)
            {
                _targetRenderer = target.GetComponent<SpriteRenderer>();
                if (_targetRenderer == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            _previousSprite = _targetRenderer.sprite;
            _tempFloat = 0;
            TweenerCore<float, float, FloatOptions> spriteTween = DOTween.To(() => _tempFloat, Setter, 1, duration);
            return spriteTween;
        }
        private void Setter(float input)
        {
            _tempFloat = input;
            if (input < 1) return;
            
            _targetRenderer.sprite = _sprite;
        }

        public override void ResetToInitialState()
        {
            if (_targetRenderer == null)
                return;

            _targetRenderer.sprite = _previousSprite;
        }
    }
}