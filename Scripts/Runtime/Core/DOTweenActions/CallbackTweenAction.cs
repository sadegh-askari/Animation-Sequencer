using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class CallbackTweenAction
    {
        private TweenCallback _onPlayCallback;
        private TweenCallback _onStepCallback;
        private TweenCallback _onCompleteCallback;

        public CallbackTweenAction(TweenCallback onPlayCallback, TweenCallback onStepCallback, TweenCallback onCompleteCallback)
        {
            _onPlayCallback = onPlayCallback;
            _onStepCallback = onStepCallback;
            _onCompleteCallback = onCompleteCallback;
        }

        public Tweener GenerateTween(float duration)
        {
            TweenerCore<int, int, NoOptions> t = DOTween.To(() => 0, x => _ = x, 1, duration);
            t.onPlay = _onPlayCallback;
            t.onStepComplete = _onStepCallback;
            t.onComplete = _onCompleteCallback;
            
            return t;
        }
    }
}