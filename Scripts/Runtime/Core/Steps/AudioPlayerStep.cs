using System;
using System.Security;
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
        [SerializeField] private int _loops = 0;
        [SerializeField] private bool _stopWhenOver;

        private AudioHandle? _audioHandle;
        private bool _hasLoops;
        public override string DisplayName => "Audio Player";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(PlayAudio, ReplayAudio, StopAudio).GenerateTween(_duration);
            tween.SetDelay(Delay);
            tween.SetLoops(_loops);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(tween);
            
            _hasLoops = sequence.hasLoops || animationSequence.hasLoops;

            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }

        private bool _firstLoop = false;
        private void ReplayAudio()
        {
            if (!_hasLoops)
                return;

            if (!_firstLoop)
            {
                _firstLoop = true;
                return;
            }

            PlayAudio();
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
            {
                _audioHandle = _audioPlayer.Play();
            }
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