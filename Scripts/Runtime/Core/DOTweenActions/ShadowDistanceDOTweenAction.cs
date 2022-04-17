using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    public class ShadowDistanceDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Shadow);
        public override string DisplayName => "Effect Distance";

        [SerializeField] private Vector2 _distance;

        private Shadow _targetShadow;
        private Vector2? _previousState;

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

            _previousState = _targetShadow.effectDistance;

            float ratio = 0;
            var imageTween = DOTween.To(() => ratio, (newValue) =>
            {
                ratio = newValue;
                _targetShadow.effectDistance = Vector2.Lerp(_targetShadow.effectDistance, _distance, ratio);
            }, 1, duration);
            return imageTween;
        }

        public override void ResetToInitialState()
        {
            if (_targetShadow == null || !_previousState.HasValue)
                return;

            _targetShadow.effectDistance = _previousState.Value;
        }
    }
}