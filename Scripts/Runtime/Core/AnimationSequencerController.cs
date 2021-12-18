using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
{
    [DisallowMultipleComponent]
    public class AnimationSequencerController : MonoBehaviour
    {
        public enum PlayType
        {
            Forward,
            Backward
        }

        [SerializeReference]
        private AnimationStepBase[] animationSteps = new AnimationStepBase[0];
        [SerializeField]
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField]
        private bool timeScaleIndependent = false;
        [SerializeField]
        private bool playOnAwake;
        [SerializeField]
        private bool pauseOnAwake;
        [SerializeField]
        private PlayType playType = PlayType.Forward;

        [SerializeField] [Range(0, 10)]
        private float timeScale = 1;

        public float TimeScale => timeScale;
        
        [SerializeField]
        private int loops = 0;
        [SerializeField]
        private LoopType loopType = LoopType.Restart;
        [SerializeField]
        private bool autoKill = true;

        [SerializeField]
        private UnityEvent onStartEvent = new UnityEvent();
        public UnityEvent OnStartEvent => onStartEvent;
        [SerializeField] private UnityEvent onFinishedEvent = new UnityEvent();
        public UnityEvent OnFinishedEvent => onFinishedEvent;
        [SerializeField] private UnityEvent onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => onProgressEvent;

        private Sequence playingSequence;
        public Sequence PlayingSequence => playingSequence;
        private TweenCancelBehaviour _cancelBehaviour;

        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();
        public bool IsPaused => playingSequence != null && playingSequence.IsActive() && !playingSequence.IsPlaying();

        public virtual void Awake()
        {
            if (playOnAwake)
            {
                Play();
                if (pauseOnAwake)
                    playingSequence.Pause();
            }
        }

        private void OnDestroy()
        {
            ClearPlayingSequence();
        }

        public void Play(TweenCancelBehaviour cancelBehaviour, CancellationToken ct, Action onCompleteCallback = null)
        {
            _cancelBehaviour = cancelBehaviour;
            ct.RegisterWithoutCaptureExecutionContext(Cancel);

            if (ct.IsCancellationRequested)
            {
                Cancel();
                return;
            }
            
            Play(onCompleteCallback);
        }

        public void Cancel()
        {
            if (animationSteps.Length == 0)
                return;

            //if (playingSequence.ElapsedPercentage() < 1)
            {
                //if (_cancelBehaviour == TweenCancelBehaviour.Kill)
                Kill(_cancelBehaviour == TweenCancelBehaviour.Complete);
            }
        }

        public virtual void Play(Action onCompleteCallback = null)
        {
            ClearPlayingSequence();
            
            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            playingSequence = GenerateSequence();
            playingSequence.timeScale = timeScale;
            
            switch (playType)
            {
                case PlayType.Backward:
                    playingSequence.PlayBackwards();
                    break;

                case PlayType.Forward:
                    playingSequence.PlayForward();
                    break;

                default:
                    playingSequence.Play();
                    break;
            }
        }

        public virtual void PlayForward(bool restFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null)
                Play();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            
            if (restFirst)
                SetProgress(0);
            
            playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (playingSequence == null)
                Play();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            
            if (completeFirst)
                SetProgress(1);
            
            playingSequence.PlayBackwards();
        }

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (playingSequence == null)
                Play();

            float duration = playingSequence.Duration();
            float progress = Mathf.Clamp01(seconds / duration);
            SetProgress(progress, andPlay);
        }
        
        public virtual void SetProgress(float progress, bool andPlay = true)
        {
            progress = Mathf.Clamp01(progress);

            if (playingSequence == null)
                Play();

            playingSequence.Goto(progress, andPlay);
        }

        public virtual void TogglePause()
        {
            if (playingSequence == null)
                return;

            playingSequence.TogglePause();
        }

        public virtual void Pause()
        {
            if (!IsPlaying)
                return;

            playingSequence.Pause();
        }

        public virtual void Resume()
        {
            if (playingSequence == null)
                return;

            playingSequence.Play();
        }


        public virtual void Complete(bool withCallbacks = true)
        {
            if (playingSequence == null)
                return;

            playingSequence.Complete(withCallbacks);
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            if (playingSequence == null)
                return;

            playingSequence.Rewind(includeDelay);
        }

        public virtual void Kill(bool complete = false)
        {
            if (!IsPlaying)
                return;

            playingSequence.Kill(complete);
        }

        public virtual IEnumerator PlayEnumerator(TweenCancelBehaviour cancelBehaviour)
        {
            _cancelBehaviour = cancelBehaviour;
            Play();
            yield return playingSequence.WaitForCompletion();
        }

        public virtual Sequence GenerateSequence()
        {
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase step = animationSteps[i];
                if (step != null)
                {
                    step.AddTweenToSequence(sequence);
                }
                else
                {
                    string hierarchy = name;
                    Transform p = transform;
                    while ((p = p.parent) != null)
                    {
                        hierarchy = p.name + "/" + hierarchy;
                    }
                    
                    Debug.LogError($"AnimationStep at {i} is null in object `{hierarchy}`");
                }
            }

            sequence.SetTarget(this);
            sequence.SetAutoKill(autoKill);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.OnStart(() =>
            {
                if (playType == PlayType.Forward)
                {
                    onStartEvent.Invoke();
                }
                else
                {
                    onFinishedEvent.Invoke();
                }
            });
            sequence.OnUpdate(() => { onProgressEvent.Invoke(); });
            sequence.OnComplete(() =>
            {
                if (playType == PlayType.Forward)
                {
                    onFinishedEvent.Invoke();
                }
                else
                {
                    onStartEvent.Invoke();
                }
            });

            int targetLoops = loops;

            if (!Application.isPlaying)
            {
                if (loops == -1)
                    targetLoops = 10;
            }

            sequence.SetLoops(targetLoops, loopType);
            return sequence;
        }

        public virtual void ResetToInitialState()
        {
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                animationSteps[i].ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            DOTween.Kill(this);
            DOTween.Kill(playingSequence);
            playingSequence = null;
        }
    }
}