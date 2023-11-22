using System;
using DG.Tweening;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class SpineUIAnimationClipStep : GameObjectAnimationStep
    {
        //[SerializeField] private float _speed;
        [SerializeField] private SkeletonGraphic  _skeletonAnimation;

        [SerializeField] private AnimationReferenceAsset _animationRefrence;
        [SerializeField] private int _loopCount;

        private bool _played;
        
        public override string DisplayName => "Spine UI Animation Clip";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(PlayAnimation, PlayStepAnimation, StopAnimation).GenerateTween(duration);
            tween.SetLoops(_loopCount);
            tween.SetDelay(Delay);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Join(tween);

            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }
        
        private void PlayStepAnimation()
        {
            //Debug.LogError("Step: " + _animationRefrence.name);
            if (_loopCount == 0 && _played)
                return;

            PlayAnimation();
        }

        private void PlayAnimation()
        {
            //Debug.LogError("Play: " + _animationRefrence.name);
            _played = true;
            
            bool loop = _loopCount != 0;
            //_skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
            TrackEntry entry = _skeletonAnimation.AnimationState.SetAnimation(0, _animationRefrence, loop);
        }
        

        private void StopAnimation()
        {
            _played = false;
            //_skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
            //Debug.LogError("Complete: " + _animationRefrence.name);
        }
        
        public override void ResetToInitialState()
        {
            //Debug.LogError("ResetState: " + _animationRefrence.name);
           // StopAnimation();
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