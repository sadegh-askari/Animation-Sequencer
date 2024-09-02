using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public sealed class RotateToEulerAnglesFromTransformRotateDOTweenAction : RotateDOTweenActionBase
    {
        public override string DisplayName => "Rotate To Transform Euler Angles";

        [SerializeField]
        private Transform target;
        [SerializeField]
        private bool useLocalEulerAngles;
        
        
        protected override Vector3 GetRotation(Transform t)
        {
            if (!useLocalEulerAngles)
                return target.eulerAngles;
            return target.localEulerAngles;
        }
    }
}
