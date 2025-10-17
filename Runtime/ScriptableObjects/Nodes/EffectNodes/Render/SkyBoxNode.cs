/*
 * Project: JuiceTwee
 * https://github.com/Kalmera74/JuiceTwee
 *
 * Author: Kalmera (GitHub: Kalmera74)
 * Copyright (c) 2025 Kalmera
 *
 * Licensed under the MIT License.
 * You may obtain a copy of the License at
 * https://opensource.org/licenses/MIT
 *
 * Version: 1.0.0
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JuiceTwee.Runtime.Attributes;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes
{

    [Serializable]
    [NodeMenu("Render", "Sky Box")]
    public class SkyBoxNode : EffectNode
    {
        public override Type TargetType => null;

        [Header("Time Options")]
        [SerializeField] private float _duration = 1;
        [SerializeField] protected bool _useUnscaledTime = false;

        [Header("Base Settings")]
        [SerializeField] private bool _updateGIAutomatically;
        [SerializeField] private bool _controlMaterial;
        [SerializeField] private bool _controlShadows;

        [Header("Material Settings")]
        [SerializeField] private bool _selectRandomSkyBox = false;
        [SerializeField] private List<Material> _skyBoxMaterials;

        [Header("Shadow Settings")]
        [SerializeField] private bool _useCurrentAsStart = true;
        [SerializeField] private Color _shadowStartColor;
        [SerializeField] private Color _shadowEndColor;

        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;




        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            onStarted?.Invoke();
            if (_controlMaterial)
            {
                if (_skyBoxMaterials.Count() == 0)
                {
                    throw new ArgumentException($"{NodeName}, There is no Skybox Material to use");
                }

                Material _skyBoxMaterial = _skyBoxMaterials[0];

                if (_selectRandomSkyBox)
                {
                    _skyBoxMaterial = _skyBoxMaterials[UnityEngine.Random.Range(0, _skyBoxMaterials.Count)];
                }

                RenderSettings.skybox = _skyBoxMaterial;
                yield return null;
                if (_updateGIAutomatically)
                {
                    DynamicGI.UpdateEnvironment();
                }
                onUpdated?.Invoke();

            }
            if (_controlShadows)
            {
                var startTime = _currentTime;
                if (_useCurrentAsStart)
                {
                    _shadowStartColor = RenderSettings.subtractiveShadowColor;
                }

                float elapsedTime = _currentTime - startTime;
                while (elapsedTime < _duration)
                {

                    elapsedTime = _currentTime - startTime;
                    float t = Mathf.Clamp01(elapsedTime / _duration);

                    Color.Lerp(_shadowStartColor, _shadowEndColor, t);
                    yield return null;
                    onUpdated?.Invoke();
                }
            }
            onCompleted?.Invoke();

        }
    }
}