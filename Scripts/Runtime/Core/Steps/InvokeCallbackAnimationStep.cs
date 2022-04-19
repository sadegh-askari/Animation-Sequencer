using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class InvokeCallbackAnimationStep : AnimationStepBase
    {
        public UnityEvent Callback;// = new UnityEvent();
        [SerializeField]
        private UnityEvent _resetCallback;
        
        public override string DisplayName => "Invoke Callback";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(null, null, () =>Callback.Invoke()).GenerateTween(0.01f);
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
            for (int i = 0; i < Callback.GetPersistentEventCount(); i++)
            {
                if (Callback.GetPersistentTarget(i) == null)
                    continue;

                persistentTargetNames = string.IsNullOrEmpty(persistentTargetNames) ?
                    Callback.GetPersistentTarget(i).name :
                    string.Join(", ", Callback.GetPersistentTarget(i).name, persistentTargetNames).Truncate(45);
            }

            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}