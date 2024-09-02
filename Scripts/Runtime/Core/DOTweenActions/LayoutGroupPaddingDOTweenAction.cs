using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class LayoutGroupPaddingDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(LayoutGroup);
        public override string DisplayName => "LayoutGroup Padding";

        [SerializeField] private RectOffset padding;

        private LayoutGroup _layoutGroup;
        private RectOffset _previousPadding;
        

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = target.GetComponent<LayoutGroup>();

                if (_layoutGroup == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            _previousPadding = _layoutGroup.padding;
        
            float temp = 0;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => temp, x =>
            {
                temp = x;
                
                int currentLeft = (int)Mathf.Lerp(_previousPadding.left, isRelative ? _previousPadding.left + padding.left : padding.left, temp);
                int currentRight = (int)Mathf.Lerp(_previousPadding.right, isRelative ? _previousPadding.right +padding.right : padding.right, temp);
                int currentTop = (int)Mathf.Lerp(_previousPadding.top, isRelative ? _previousPadding.top + padding.top : padding.top, temp);
                int currentBottom = (int)Mathf.Lerp(_previousPadding.bottom, isRelative ? _previousPadding.bottom + padding.bottom : padding.bottom, temp);

                _layoutGroup.padding = new RectOffset(currentLeft, currentRight, currentTop, currentBottom);

            }, 1, duration);
        
            return t;
        }

        public override void ResetToInitialState()
        {
            if (_layoutGroup == null)
                return;

            _layoutGroup.padding = _previousPadding;
        }
    }
}