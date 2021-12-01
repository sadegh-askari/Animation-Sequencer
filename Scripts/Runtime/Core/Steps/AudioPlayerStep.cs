using System;
using Audoty;
using DG.Tweening;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class AudioPlayerStep : AnimationStepBase
    {
        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private float _duration = 1;

        private AudioHandle? _audioHandle;
        public override string DisplayName => "Audio Player";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(PlayAudio, null, StopAudio).GenerateTween(_duration);
            tween.SetDelay(Delay);

            if (FlowType == FlowType.Join)
                animationSequence.Join(tween);
            else
                animationSequence.Append(tween);
        }

        private void StopAudio()
        {
            _audioHandle?.Stop();
            _audioHandle = null;
        }

        private void PlayAudio()
        {
            _audioHandle?.Stop();
            _audioHandle = _audioPlayer.Play();
        }

        public override void ResetToInitialState()
        {
            StopAudio();
        }
    }
}