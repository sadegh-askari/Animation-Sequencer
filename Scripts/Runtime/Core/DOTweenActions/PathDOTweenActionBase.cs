using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public abstract class PathDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField]
        protected bool isLocal;
        [SerializeField]
        private Color gizmoColor;
        [SerializeField]
        private int resolution = 10;
        [SerializeField]
        private PathMode pathMode = PathMode.Full3D;
        [SerializeField]
        private PathType pathType = PathType.CatmullRom;

        private Transform previousTarget;
        private Vector3 previousPosition;

        [SerializeField] private bool[] _fixerList; //fix for property drawer in nested class cant show first list
        private GameObject _target;

        private float _duration;
        TweenerCore<Vector3, Path, PathOptions> tween;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            _duration = duration;
            _target = target;

            SetPath(_target.transform);

            return tween;
        }

        public void SetPath(Transform target)
        {
            _duration = 0.8f;
            Debug.Log("Duration: " + _duration);

            previousTarget = target;
            if (!isLocal)
            {
                tween = target.DOPath(GetPathPositions(), _duration, pathType, pathMode, resolution, gizmoColor);
                previousPosition = target.position;
            }
            else
            {
                tween = target.DOLocalPath(GetPathPositions(), _duration, pathType, pathMode, resolution, gizmoColor);
                previousPosition = target.localPosition;
            }
        }


        protected abstract Vector3[] GetPathPositions();
        public override void ResetToInitialState()
        {
            if (isLocal)
            {
                previousTarget.transform.localPosition = previousPosition;
            }
            else
            {
                previousTarget.transform.position = previousPosition;
            }
        }
    }
}