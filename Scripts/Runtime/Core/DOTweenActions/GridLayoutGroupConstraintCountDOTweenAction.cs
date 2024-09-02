using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class GridLayoutGroupConstraintCountDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(LayoutGroup);
        public override string DisplayName => "GridLayoutGroup ConstraintCount";

        [SerializeField] private int constraintCount;

        private GridLayoutGroup _layoutGroup;
        private int _previousConstraintCount;
        

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = target.GetComponent<GridLayoutGroup>();

                if (_layoutGroup == null)
                {
                    Debug.LogError($"{target} does not have GridLayoutGroup component");
                    return null;
                }
            }

            _previousConstraintCount = _layoutGroup.constraintCount;
            
            float temp = 0;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => temp, x =>
            {
                temp = x;
                
                var currentCount = (int)Mathf.Lerp(_previousConstraintCount, isRelative ? _previousConstraintCount + constraintCount : constraintCount, temp);
                _layoutGroup.constraintCount = currentCount;

            }, 1, duration);
        
            return t;
        }

        public override void ResetToInitialState()
        {
            if (_layoutGroup == null)
                return;

            _layoutGroup.constraintCount = _previousConstraintCount;
        }
    }
}