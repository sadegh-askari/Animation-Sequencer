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
        [SerializeField] private bool _stopWhenOver;

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
            if (_stopWhenOver)
            {
                _audioHandle?.Stop();
                _audioHandle = null;
            }
        }

        private void PlayAudio()
        {
            _audioHandle?.Stop();
            if (_audioPlayer != null)
                _audioHandle = _audioPlayer.Play();
        }

        public override void ResetToInitialState()
        {
            StopAudio();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string persistentTargetNames = String.Empty;
            if (_audioPlayer != null)
                persistentTargetNames = _audioPlayer.name;

            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}