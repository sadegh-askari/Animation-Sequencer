using System;
using UnityEngine;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class LookAtTargetDoTweenAction : RotateDOTweenActionBase
    {
        public override string DisplayName => "Look At Target";
        
        [SerializeField]
        private Transform target;
        
        protected override Vector3 GetRotation(Transform t)
        {
            Vector3 direction = target.position - t.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 lookRotationEuler = lookRotation.eulerAngles;

            return lookRotationEuler;
        }
    }
}