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

        [SerializeField] private string _clipName;
        [SerializeField] private int _loopCount;

        public override string DisplayName => "Spine Animation Clip";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(PlayAnimation, PlayAnimation, StopAnimation).GenerateTween(duration);
            tween.SetLoops(_loopCount);
            tween.SetDelay(Delay);

            if (FlowType == FlowType.Join)
                animationSequence.Join(tween);
            else
                animationSequence.Append(tween);
        }

        private void PlayAnimation()
        {
            bool loop = _loopCount > 0;
            TrackEntry entry = _skeletonAnimation.AnimationState.SetAnimation(0, _clipName, loop);
        }
        

        private void StopAnimation()
        {
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