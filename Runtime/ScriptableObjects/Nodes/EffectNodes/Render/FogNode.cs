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
using JuiceTwee.Runtime.Attributes;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes
{

    [Serializable]
    [NodeMenu("Render", "Fog")]
    public class FogNode : EffectNode
    {
        public override Type TargetType => null;

        [SerializeField] private float _duration = 1f;
        [SerializeField] protected bool _useUnscaledTime = false;


        [SerializeField] private bool _controlEnabledState = true;
        [SerializeField] private bool _controlFogMode = false;
        [SerializeField] private bool _tweenFogColor = true;


        [SerializeField] private bool _useCurrentAsStart = true;
        [SerializeField] private AnimationCurve _blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


        [SerializeField] private bool _isFogEnabled = true;


        [SerializeField] private FogMode _fogMode = FogMode.ExponentialSquared;


        [SerializeField] private Color _startColor = Color.gray;
        [SerializeField] private Color _endColor = Color.white;


        [SerializeField, Min(0)] private float _startDensity = 0.01f;
        [SerializeField, Min(0)] private float _endDensity = 0.05f;


        [SerializeField] private float _startLinearStart = 0f;
        [SerializeField] private float _startLinearEnd = 300f;
        [SerializeField] private float _endLinearStart = 0f;
        [SerializeField] private float _endLinearEnd = 100f;


        private StartValues _startValues;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        public override IEnumerator Perform()
        {
            if (_useCurrentAsStart)
            {
                _startValues.Color = RenderSettings.fogColor;
                _startValues.Density = RenderSettings.fogDensity;
                _startValues.LinearStart = RenderSettings.fogStartDistance;
                _startValues.LinearEnd = RenderSettings.fogEndDistance;
            }
            else
            {
                _startValues.Color = _startColor;
                _startValues.Density = _startDensity;
                _startValues.LinearStart = _startLinearStart;
                _startValues.LinearEnd = _startLinearEnd;
            }

            if (_controlEnabledState)
            {
                RenderSettings.fog = _isFogEnabled;
            }
            if (_controlFogMode)
            {
                RenderSettings.fogMode = _fogMode;
            }

            float startTime = _currentTime;

            onStarted?.Invoke();

            float elapsedTime = _currentTime - startTime;

            while (elapsedTime < _duration)
            {

                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);
                float curveT = _blendCurve.Evaluate(t);

                if (_tweenFogColor)
                {
                    RenderSettings.fogColor = Color.Lerp(_startValues.Color, _endColor, curveT);
                }

                if (_controlFogMode)
                {

                    if (_fogMode == FogMode.Exponential || _fogMode == FogMode.ExponentialSquared)
                    {
                        RenderSettings.fogDensity = Mathf.Lerp(_startValues.Density, _endDensity, curveT);
                    }

                    else
                    {
                        RenderSettings.fogStartDistance = Mathf.Lerp(_startValues.LinearStart, _endLinearStart, curveT);
                        RenderSettings.fogEndDistance = Mathf.Lerp(_startValues.LinearEnd, _endLinearEnd, curveT);
                    }
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_tweenFogColor)
            {
                RenderSettings.fogColor = _endColor;
            }
            if (_controlFogMode)
            {

                if (_fogMode == FogMode.Exponential || _fogMode == FogMode.ExponentialSquared)
                {
                    RenderSettings.fogDensity = _endDensity;
                }
                else
                {
                    RenderSettings.fogStartDistance = _endLinearStart;
                    RenderSettings.fogEndDistance = _endLinearEnd;
                }
            }
            onCompleted?.Invoke();
            yield return base.Perform();
        }
        private struct StartValues
        {
            public Color Color;
            public float Density;
            public float LinearStart;
            public float LinearEnd;
        }
    }
}