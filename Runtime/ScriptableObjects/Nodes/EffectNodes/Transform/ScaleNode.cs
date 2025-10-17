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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TransformNodes
{
    [Serializable]
    [NodeMenu("Transform", "Scale")]
    public class ScaleNode : EffectNode
    {
        public override Type TargetType => typeof(Transform);

        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _useSpeedInsteadOfDuration;


        [SerializeField] private bool _useRelativeScale;
        [SerializeField] private bool _snapToScale;

        [SerializeField] private float _scaleSpeed = 1f;
        [SerializeField] private bool _useSeparateAxisCurves;

        [SerializeField] private Vector3 _startScale = Vector3.one;
        [SerializeField] private Vector3 _endScale = Vector3.one;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _xScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _yScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _zScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);


        private Transform _target => originTarget as Transform;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        public override IEnumerator Perform()
        {
            if (_target == null) yield break;

            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            float startTime = _currentTime;

            Vector3 startValue = _startScale;
            Vector3 endValue = _endScale;

            if (_useRelativeScale)
            {
                Vector3 baseScale = _target.localScale;
                startValue = Vector3.Scale(baseScale, _startScale);
                endValue = Vector3.Scale(baseScale, _endScale);
            }

            if (_useSpeedInsteadOfDuration)
            {
                float distance = Vector3.Distance(startValue, endValue);
                _duration = Mathf.Approximately(_scaleSpeed, 0f) ? 0f : distance / Mathf.Abs(_scaleSpeed);
            }

            if (Mathf.Approximately(_duration, 0f))
            {
                if (_snapToScale)
                {
                    _target.localScale = endValue;
                }
                onCompleted?.Invoke();
                yield break;
            }

            float endTime = startTime + _duration;
            float currentTime = startTime;

            onStarted?.Invoke();
            while (currentTime < endTime)
            {
                currentTime = _currentTime;
                float t = Mathf.Clamp01((currentTime - startTime) / _duration);

                Vector3 newScale;
                if (_useSeparateAxisCurves)
                {
                    float x = Mathf.Lerp(startValue.x, endValue.x, _xScaleCurve.Evaluate(t));
                    float y = Mathf.Lerp(startValue.y, endValue.y, _yScaleCurve.Evaluate(t));
                    float z = Mathf.Lerp(startValue.z, endValue.z, _zScaleCurve.Evaluate(t));
                    newScale = new Vector3(x, y, z);
                }
                else
                {
                    newScale = Vector3.Lerp(startValue, endValue, _scaleCurve.Evaluate(t));
                }

                _target.localScale = newScale;
                onUpdated?.Invoke();
                yield return null;
            }

            if (_snapToScale)
            {
                _target.localScale = endValue;
            }

            onCompleted?.Invoke();
        }
    }
}