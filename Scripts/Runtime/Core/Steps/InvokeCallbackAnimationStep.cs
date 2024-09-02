using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BrunoMikoski.AnimationSequencer
{
    public enum InvokeType
    {
        OneTime,
        Recurring
    }

    [Serializable]
    public sealed class InvokeCallbackAnimationStep : AnimationStepBase
    {
        public InvokeType invokeType;
        public float duration;

        [SerializeField] private UnityEvent callback; // = new UnityEvent();

        public UnityEvent Callback => callback;

        [SerializeField] private UnityEvent _resetCallback;

        public override string DisplayName => "Invoke Callback";

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween =
                new CallbackTweenAction(() =>
                {
                    callback?.Invoke();
                }, OnStepCallback, null).GenerateTween(duration);
            tween.SetDelay(Delay);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(tween);

            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }

        private void OnStepCallback()
        {
            if (invokeType == InvokeType.Recurring)
            {
                callback?.Invoke();
            }
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

                persistentTargetNames = string.IsNullOrEmpty(persistentTargetNames)
                    ? callback.GetPersistentTarget(i).name
                    : string.Join(", ", callback.GetPersistentTarget(i).name, persistentTargetNames).Truncate(45);
            }

            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}