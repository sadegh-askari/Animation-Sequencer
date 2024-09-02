using System;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[Serializable]
public class AnchoredPositionMoveSizeDOTweenActionBase : AnchoredPositionMoveDOTweenActionBase
{
    [SerializeField] private Vector2 size;
    private Vector2 _previousSize;

    public override string DisplayName => "Move To Size";

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

        _previousSize = rectTransform.sizeDelta;
        
        TweenerCore<Vector2, Vector2, VectorOptions> anchorPosTween = rectTransform.DOSizeDelta(size, duration);
        return anchorPosTween;
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

        rectTransform.sizeDelta = _previousSize;
    }
}
