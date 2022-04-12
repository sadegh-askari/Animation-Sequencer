﻿using System;
using System.Linq;
using System.Security.Permissions;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class DOTweenAnimationStep : GameObjectAnimationStep
    {
        public override string DisplayName => "Tween Target";
        [SerializeField] private int loopCount;

        [SerializeField] private LoopType loopType;
        [SerializeReference] private DOTweenActionBase[] actions;
        public DOTweenActionBase[] Actions => actions;

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = DOTween.Sequence();

            foreach (DOTweenActionBase action in actions)
            {
                if (action == null)
                    continue;

                Tween tween = action.GenerateTween(target, duration);
                tween.SetDelay(Delay);
                tween.SetLoops(loopCount, loopType);
                sequence.Join(tween);
            }

            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }

        public override void ResetToInitialState()
        {
            for (int i = actions.Length - 1; i >= 0; i--)
            {
                actions[i].ResetToInitialState();
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (target != null)
                targetName = target.name;

            return
                $"{index}. {targetName}: {String.Join(", ", actions.Select(action => action.DisplayName)).Truncate(45)}";
        }
    }
}