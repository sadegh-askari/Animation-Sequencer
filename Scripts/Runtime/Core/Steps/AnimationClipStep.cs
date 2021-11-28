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

        private Animator _animator;
        private CancellationTokenSource _tokenSource;

        public override string DisplayName => "Animation Clip";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(PlayAnimation, PlayAnimation, CancelToken).GenerateTween(duration);
            tween.SetLoops(_loopCount);
            tween.SetDelay(Delay);

            if (FlowType == FlowType.Join)
                animationSequence.Join(tween);
            else
                animationSequence.Append(tween);
        }

        private void PlayAnimation()
        {
            if (_tokenSource != null)
                CancelToken();

            _tokenSource = new CancellationTokenSource();
            PlayAnimationHandler(_tokenSource.Token);
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
                playable.SetDuration(duration);
                playable.SetSpeed(_animationClip.length / duration);
                //playable.SetSpeed(_speed);

                await UniTask.Delay((int) (duration * 1000), cancellationToken: ct);
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
    }
}