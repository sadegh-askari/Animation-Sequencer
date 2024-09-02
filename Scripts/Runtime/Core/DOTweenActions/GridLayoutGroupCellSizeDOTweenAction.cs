using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class GridLayoutGroupCellSizeDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(LayoutGroup);
        public override string DisplayName => "GridLayoutGroup CellSize";

        [SerializeField] private Vector2 cellSize;

        private GridLayoutGroup _layoutGroup;
        private Vector2 _previousSize;
        

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

            _previousSize = _layoutGroup.cellSize;
            
            float temp = 0;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => temp, x =>
            {
                temp = x;
                var currentSize = Vector2.Lerp(_previousSize, isRelative ? _previousSize + cellSize : cellSize, temp);
                _layoutGroup.cellSize = currentSize;

            }, 1, duration);
        
            return t;
        }

        public override void ResetToInitialState()
        {
            if (_layoutGroup == null)
                return;

            _layoutGroup.cellSize = _previousSize;
        }
    }
}