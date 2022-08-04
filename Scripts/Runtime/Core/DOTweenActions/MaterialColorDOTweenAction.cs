using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Object = System.Object;

namespace BrunoMikoski.AnimationSequencer
{
    [Serializable]
    public class MaterialColorDOTweenAction : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Renderer);
        public override string DisplayName => "Material Color";

        public string PropertyName = "_AdditiveColor";
        [SerializeField] private Color _color;

        private int _propertyId;
        private Renderer _targetRenderer;
        private Color _previousColor;
        [SerializeField] private Material _material;
        [SerializeField] private Material _sharedMaterial;

        [SerializeField] private bool _useDynamicSharedMaterial;
        private static Dictionary<string, SharedMatData> _staticSharedMaterials = new();

        class SharedMatData
        {
            public Material Material;
            public bool Playing;
        }

        private bool _playing = false;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            _propertyId = Shader.PropertyToID(PropertyName);
            if (_targetRenderer == null)
            {
                _targetRenderer = target.GetComponent<Renderer>();
                if (_targetRenderer == null)
                {
                    Debug.LogError($"{target} does not have {TargetComponentType} component");
                    return null;
                }
            }

            if (_sharedMaterial == null)
                _sharedMaterial = _targetRenderer.sharedMaterial;

            if (_useDynamicSharedMaterial)
            {
                _staticSharedMaterials.TryGetValue(_targetRenderer.name, out var sharedMatData);
                if (sharedMatData == null)
                {
                    sharedMatData = new SharedMatData();
                    _staticSharedMaterials.Add(_targetRenderer.name, sharedMatData);
                }

                if (sharedMatData.Material == null)
                {
                    sharedMatData.Material = new Material(_targetRenderer.sharedMaterial);
                    sharedMatData.Playing = false;
                }
            }
            
            _material = _targetRenderer.sharedMaterial;
            if (Application.isPlaying)
            {
                if (_useDynamicSharedMaterial)
                    _material = _staticSharedMaterials[_targetRenderer.name].Material;
                else
                    _material = _targetRenderer.material;
            }


            if (_material.HasColor(_propertyId))
            {
                if (_useDynamicSharedMaterial)
                {
                    if (_staticSharedMaterials[_targetRenderer.name].Playing)
                    {
                        TweenerCore<int, int, NoOptions> noOpTween = DOTween.To(() => 0, _ => {}, 1, duration);
                        
                        noOpTween.OnComplete(() =>
                        {
                            _targetRenderer.material = _sharedMaterial;
                        });
                        
                        _targetRenderer.material = _staticSharedMaterials[_targetRenderer.name].Material;
                        return noOpTween;
                    }
                    else
                    {
                        return CreateNormalTween(duration);
                    }
                }
                else
                {
                    return CreateNormalTween(duration);
                }
            }

            return null;
        }
        private Tweener CreateNormalTween(float duration)
        {
            _playing = true;
            
            _previousColor = _material.GetColor(_propertyId);
            TweenerCore<Color, Color, ColorOptions> materialTween = _material.DOColor(_color, _propertyId, duration);
            materialTween.OnComplete(() =>
            {
                _targetRenderer.material = _sharedMaterial;

                if (_useDynamicSharedMaterial)
                {
                    if (_staticSharedMaterials.TryGetValue(_targetRenderer.name, out var dMat))
                    {
                        UnityEngine.Object.Destroy(dMat.Material);
                        _staticSharedMaterials.Remove(_targetRenderer.name);
                    }
                }
            });

            materialTween.OnStart(() =>
            {
                _targetRenderer.material = _material;
            });

            _targetRenderer.material = _sharedMaterial;
            return materialTween;
        }

        public override void ResetToInitialState()
        {
            if (_targetRenderer == null )
                return;

            if (_playing)
            {
                _material = _targetRenderer.sharedMaterial;
                if (Application.isPlaying)
                {
                    if (_useDynamicSharedMaterial)
                    {
                        if (!_staticSharedMaterials.TryGetValue(_targetRenderer.name, out var matData))
                        {
                            _material = _targetRenderer.material;
                        }
                        else
                        {
                            _material = matData.Material;
                        }
                    }
                    else
                        _material = _targetRenderer.material;
                }
                _material.SetColor(_propertyId, _previousColor);
            }
            
            _targetRenderer.material = _sharedMaterial;
        }
    }
}