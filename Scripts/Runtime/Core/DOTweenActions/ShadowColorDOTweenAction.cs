using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace BrunoMikoski.AnimationSequencer
{
    public class ShadowColorDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Shadow);
        public override string DisplayName => "Effect Color";

        [SerializeField] private Color _color;

        private Shadow _targetShadow;
        private Color? _previousState;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (_targetShadow == null)
            {
                _targetShadow = target.GetComponent<Shadow>();
                if (_targetShadow == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            _previousState = _targetShadow.effectColor;

            float ratio = 0;
            var imageTween = DOTween.To(() => ratio, (newValue) =>
            {
                ratio = newValue;
                _targetShadow.effectColor = Color.Lerp(_targetShadow.effectColor, _color, ratio);
            }, 1, duration);
            return imageTween;
        }

        public override void ResetToInitialState()
        {
            if (_targetShadow == null || !_previousState.HasValue)
                return;

            _targetShadow.effectColor = _previousState.Value;
        }
    }
}