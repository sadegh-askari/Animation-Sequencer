using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class InvokeCallbackAnimationStep : AnimationStepBase
    {
        [SerializeField]
        private UnityEvent callback;// = new UnityEvent();

        public UnityEvent Callback => callback;
        
        [SerializeField]
        private UnityEvent _resetCallback;
        
        public override string DisplayName => "Invoke Callback";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(null, null, () =>callback.Invoke()).GenerateTween(0.01f);
            tween.SetDelay(Delay);

            if (FlowType == FlowType.Join)
                animationSequence.Join(tween);
            else
                animationSequence.Append(tween);
        }

        public override void ResetToInitialState()
        {
            _resetCallback.Invoke();
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string persistentTargetNames = String.Empty;
            for (int i = 0; i < callback.GetPersistentEventCount(); i++)
            {
                if (callback.GetPersistentTarget(i) == null)
                    continue;

                persistentTargetNames = string.IsNullOrEmpty(persistentTargetNames) ?
                    callback.GetPersistentTarget(i).name :
                    string.Join(", ", callback.GetPersistentTarget(i).name, persistentTargetNames).Truncate(45);
            }

            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}