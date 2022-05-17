using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class AnimationClipStep : GameObjectAnimationStep
    {
        //[SerializeField] private float _speed;
        [SerializeField] private AnimationClip _animationClip;
        [SerializeField] private int _loopCount;
        [SerializeField] private float _playbackSpeed = 1;

        private Animator _animator;
        private CancellationTokenSource _tokenSource;

        public override string DisplayName => "Animation Clip";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            if (_playbackSpeed == 0)
            {
                _playbackSpeed = 1;
            }

            Tween tween =
                new CallbackTweenAction(PlayAnimation, OnStepCallback, CancelToken).GenerateTween(
                    _animationClip.length/_playbackSpeed);

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
            if (_tokenSource != null)
                CancelToken();


            _tokenSource = new CancellationTokenSource();
            PlayAnimationHandler(_tokenSource.Token);
        }

        private void OnStepCallback()
        {
            if (_loopCount == 0)
            {
                return;
            }

            PlayAnimation();
        }

        private async void PlayAnimationHandler(CancellationToken ct)
        {
            AnimationClipPlayable playable = default;
            try
            {
                _animator = target.GetComponent<Animator>();
                if (_animator == null)
                    _animator = target.AddComponent<Animator>();

                _animationClip.legacy = false;


                playable = AnimationPlayableUtilities.PlayClip(_animator, _animationClip, out PlayableGraph _);
                playable.SetDuration(_animationClip.length/_playbackSpeed);

                playable.SetSpeed(_playbackSpeed);
                //playable.SetSpeed(_speed);

                await UniTask.Delay((int) (_animationClip.length/_playbackSpeed * 1000), cancellationToken: ct);
            }
            catch (OperationCanceledException e)
            {
            }
            finally
            {
                
                playable.Pause();
                playable.Destroy();
            }
        }

        public override void ResetToInitialState()
        {
            CancelToken();
        }

        private void CancelToken()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string clipName = "Animation";
            if (_animationClip != null)
                clipName = _animationClip.name;

            return $"{index}. Play {clipName} Clip";
        }
    }
}