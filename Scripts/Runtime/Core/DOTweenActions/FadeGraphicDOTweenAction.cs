﻿using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class FadeGraphicDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Graphic);
        public override string DisplayName => "Fade Graphic";

        [SerializeField]
        private float alpha;
        
        private Graphic targetGraphic;
        private float previousAlpha;
        
        public Graphic TargetGraphic => targetGraphic;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            if (targetGraphic == null)
            {
                targetGraphic = target.GetComponent<Graphic>();
                if (targetGraphic == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            previousAlpha = targetGraphic.color.a;
            TweenerCore<Color, Color, ColorOptions> graphicTween = targetGraphic.DOFade(alpha, duration);
            return graphicTween;
        }

        public override void ResetToInitialState()
        {
            if (targetGraphic == null)
                return;

            Color color = targetGraphic.color;
            color.a = previousAlpha;
            targetGraphic.color = color;
        }
    }
}
