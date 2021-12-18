using DG.Tweening;
using UnityEngine;
namespace BrunoMikoski.AnimationSequencer
{
    public class InstantiateObjectClipStep : GameObjectAnimationStep
    {
        [SerializeField] private GameObject _prefab;

        [SerializeField] private bool _destroyWhenOver;
        private GameObject _initedObj;
        public override string DisplayName => "Instantiate Prefab";
        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Tween tween = new CallbackTweenAction(InstantiateObj, null, ResetToInitialState).GenerateTween(duration);
            tween.SetDelay(Delay);

            if (FlowType == FlowType.Join)
                animationSequence.Join(tween);
            else
                animationSequence.Append(tween);
        }
        private void InstantiateObj()
        {
            _initedObj = target != null ? Object.Instantiate(_prefab, target.transform, false) : Object.Instantiate(_prefab);
        }
        public override void ResetToInitialState()
        {
            if (_destroyWhenOver)
                Object.Destroy(_initedObj);
            else
                _initedObj.transform.SetParent(null);
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string prefName = "NULL";
            if (_prefab != null)
                prefName = _prefab.name;

            return $"{index}. Instantiate: {prefName}";
        }
    }
}