using System;
using DG.Tweening;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class SpineAnimationClipStep : GameObjectAnimationStep
    {
        //[SerializeField] private float _speed;
        [SerializeField] private SkeletonAnimation  _skeletonAnimation;

        [SerializeField] private AnimationReferenceAsset _animationRefrence;
        [SerializeField] private int _loopCount;

        public override string DisplayName => "Spine Animation Clip";

        public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(PlayAnimation, PlayAnimation, StopAnimation).GenerateTween(duration);
            tween.SetLoops(_loopCount);
            tween.SetDelay(Delay);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Join(tween);

            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }

        private void PlayAnimation()
        {
            bool loop = _loopCount != 0;
            _skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
            TrackEntry entry = _skeletonAnimation.AnimationState.SetAnimation(0, _animationRefrence, loop);
        }


        private void StopAnimation()
        {
            //_skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
        }
        
        public override void ResetToInitialState()
        {
            StopAnimation();
        }
        
        public override string GetDisplayNameForEditor(int index)
        {
            string clipName = "Animation";
            if (_skeletonAnimation != null)
                clipName = _skeletonAnimation.name;
            
            return $"{index}. Play {clipName} Clip";
        }
    }
}