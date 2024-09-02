using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[Serializable]
public class AnchoredPositionMoveOffsetDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
{
    [SerializeField] private float left;
    [SerializeField] private float right;
    [SerializeField] private float top;
    [SerializeField] private float bottom;

    private float _previousLeft;
    private float _previousRight;
    private float _previousTop;
    private float _previousBottom;

    public override string DisplayName => "Move To Offset Pos";

    protected override Tweener GenerateTween_Internal(GameObject target, float duration)
    {
        if (rectTransform == null)
        {
            rectTransform = target.transform as RectTransform;

            if (rectTransform == null)
            {
                Debug.LogError($"{target} does not have {TargetComponentType} component");
                return null;
            }
        }

        _previousLeft = rectTransform.offsetMin.x;
        _previousBottom = rectTransform.offsetMin.y;

        _previousRight = rectTransform.offsetMax.x;
        _previousTop = rectTransform.offsetMax.y;
        
        
        float temp = 0;
        TweenerCore<float, float, FloatOptions> t = DOTween.To(() => temp, x =>
        {
            temp = x;
            float currentLeft = Mathf.Lerp(_previousLeft, isRelative ? _previousLeft + left : left, temp);
            float currentRight = Mathf.Lerp(_previousRight, isRelative ? _previousRight + right : right, temp);
            float currentTop = Mathf.Lerp(_previousTop, isRelative ? _previousTop + top : top, temp);
            float currentBottom = Mathf.Lerp(_previousBottom, isRelative ? _previousBottom + bottom : bottom, temp);
            
            rectTransform.offsetMin = new Vector2(currentLeft, currentBottom);
            rectTransform.offsetMax = new Vector2(currentRight, currentTop);
        }, 1, duration);
        
        return t;
    }

    protected override Vector2 GetPosition()
    {
        return Vector2.zero;
    }

    //protected abstract Vector2 GetPosition();

    public override void ResetToInitialState()
    {
        if (rectTransform == null)
            return;

        rectTransform.offsetMin = new Vector2(_previousLeft, _previousBottom);
        rectTransform.offsetMax = new Vector2(_previousRight, _previousTop);
    }
}
