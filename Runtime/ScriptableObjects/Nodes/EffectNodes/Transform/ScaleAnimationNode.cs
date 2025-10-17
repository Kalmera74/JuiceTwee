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
using JuiceTwee.Runtime.Utility;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TransformNodes
{
    [Serializable]
    [NodeMenu("Transform", "Scale Animation")]
    public class ScaleAnimationNode : EffectNode
    {
        public override Type TargetType => typeof(Transform);

        [Header("Time Options")]
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _useUnscaledTime = false;

        [Header("Base Settings")]
        [SerializeField] private bool _overrideStartScale = false;
        [SerializeField] private Vector3 _startScale = Vector3.one;
        [SerializeField] private bool _returnToInitialScale = true;
        [SerializeField] private bool _smoothReturn = true;
        [SerializeField] private float _smoothReturnDuration = 0.5f;
        [SerializeField] private AnimationCurveType _returnCurveType = AnimationCurveType.Linear;

        [Header("Scale Animation Settings")]
        [SerializeField] private ScaleAnimation _scaleAnimation = ScaleAnimation.Squish;
        [SerializeField] private bool _lockXScale = false;
        [SerializeField] private bool _lockYScale = false;
        [SerializeField] private bool _lockZScale = false;

        [Header("Squish Settings")]
        [SerializeField] private float _squishAmount = 0.5f;
        [SerializeField] private AnimationAxis _squishAxis;
        [SerializeField] private AnimationCurveType _squishCurveType = AnimationCurveType.EaseInOut;
        [SerializeField] private bool _useCustomSquishCurve = false;
        [SerializeField] private AnimationCurve _customSquishCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Stretch Settings")]
        [SerializeField] private float _stretchAmount = 1.5f;
        [SerializeField] private AnimationAxis _stretchAxis;
        [SerializeField] private AnimationCurveType _stretchCurveType = AnimationCurveType.EaseInOut;
        [SerializeField] private bool _useCustomStretchCurve = false;
        [SerializeField] private AnimationCurve _customStretchCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Transform _target => originTarget as Transform;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        private Vector3 _initialScale;
        private Vector3 _animationStartScale;

        public override IEnumerator Perform()
        {
            if (_target == null)
            {
                Debug.LogWarning($"{NodeName}: Target Transform is not set. Skipping animation.", this);
                yield break;
            }

            _initialScale = _target.localScale;

            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            float startTime = _currentTime;
            float elapsedTime = 0f;

            _animationStartScale = _overrideStartScale ? _startScale : _target.localScale;

            onStarted?.Invoke();

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);
                switch (_scaleAnimation)
                {
                    case ScaleAnimation.Squish:
                        AnimateSquish(t);
                        break;
                    case ScaleAnimation.Stretch:
                        AnimateStretch(t);
                        break;
                }
                yield return null;
                onUpdated?.Invoke();
            }


            if (_returnToInitialScale)
            {
                if (_smoothReturn)
                {
                    yield return SmoothReturn();
                }
                else
                {
                    _target.localScale = _initialScale;
                }
            }
            onCompleted?.Invoke();
        }

        private void AnimateStretch(float t)
        {
            Vector3 endScale = Vector3.one * _stretchAmount;
            var stretchAmount = Mathf.Abs(_stretchAmount);
            float currentVolumeFactor;
            float compensationFactor;
            float finalXScale, finalYScale, finalZScale;

            switch (_stretchAxis)
            {
                case AnimationAxis.X:
                    finalXScale = _animationStartScale.x * (1f + stretchAmount);
                    currentVolumeFactor = finalXScale / _animationStartScale.x;
                    compensationFactor = Mathf.Sqrt(1f / currentVolumeFactor);

                    endScale.x = finalXScale;
                    endScale.y = _animationStartScale.y * compensationFactor;
                    endScale.z = _animationStartScale.z * compensationFactor;
                    break;
                case AnimationAxis.Y:
                    finalYScale = _animationStartScale.y * (1f + stretchAmount);
                    currentVolumeFactor = finalYScale / _animationStartScale.y;
                    compensationFactor = Mathf.Sqrt(1f / currentVolumeFactor);

                    endScale.x = _animationStartScale.x * compensationFactor;
                    endScale.y = finalYScale;
                    endScale.z = _animationStartScale.z * compensationFactor;
                    break;
                case AnimationAxis.Z:
                    finalZScale = _animationStartScale.z * (1f + stretchAmount);
                    currentVolumeFactor = finalZScale / _animationStartScale.z;
                    compensationFactor = Mathf.Sqrt(1f / currentVolumeFactor);

                    endScale.x = _animationStartScale.x * compensationFactor;
                    endScale.y = _animationStartScale.y * compensationFactor;
                    endScale.z = finalZScale;
                    break;
            }


            var easedValue = _useCustomStretchCurve ? _customStretchCurve.Evaluate(t) : EasingFunctions.GetEasedValue(t, _stretchCurveType);
            var currentScale = Vector3.Lerp(_animationStartScale, endScale, easedValue);

            LockAxis(ref currentScale);
            _target.localScale = currentScale;

        }
        private void AnimateSquish(float t)
        {
            Vector3 endScale = _animationStartScale;
            float effectiveSquishAmount = Mathf.Abs(_squishAmount);

            float currentVolumeFactor;
            float compensationFactor;
            float finalAxisScale;

            switch (_squishAxis)
            {
                case AnimationAxis.X:
                    finalAxisScale = _animationStartScale.x * (1f - effectiveSquishAmount);
                    finalAxisScale = Mathf.Max(0.001f, finalAxisScale);
                    currentVolumeFactor = finalAxisScale / _animationStartScale.x;
                    compensationFactor = (currentVolumeFactor > 0) ? Mathf.Sqrt(1f / currentVolumeFactor) : 1f;

                    endScale.x = finalAxisScale;
                    endScale.y = _animationStartScale.y * compensationFactor;
                    endScale.z = _animationStartScale.z * compensationFactor;
                    break;

                case AnimationAxis.Y:
                    finalAxisScale = _animationStartScale.y * (1f - effectiveSquishAmount);
                    finalAxisScale = Mathf.Max(0.001f, finalAxisScale);
                    currentVolumeFactor = finalAxisScale / _animationStartScale.y;
                    compensationFactor = (currentVolumeFactor > 0) ? Mathf.Sqrt(1f / currentVolumeFactor) : 1f;

                    endScale.x = _animationStartScale.x * compensationFactor;
                    endScale.y = finalAxisScale;
                    endScale.z = _animationStartScale.z * compensationFactor;
                    break;

                case AnimationAxis.Z:
                    finalAxisScale = _animationStartScale.z * (1f - effectiveSquishAmount);
                    finalAxisScale = Mathf.Max(0.001f, finalAxisScale);
                    currentVolumeFactor = finalAxisScale / _animationStartScale.z;
                    compensationFactor = (currentVolumeFactor > 0) ? Mathf.Sqrt(1f / currentVolumeFactor) : 1f;

                    endScale.x = _animationStartScale.x * compensationFactor;
                    endScale.y = _animationStartScale.y * compensationFactor;
                    endScale.z = finalAxisScale;
                    break;
            }

            var easedValue = _useCustomSquishCurve ? _customSquishCurve.Evaluate(t) : EasingFunctions.GetEasedValue(t, _squishCurveType);

            Vector3 currentScale = Vector3.Lerp(_animationStartScale, endScale, easedValue);

            LockAxis(ref currentScale);
            _target.localScale = currentScale;
        }


        private IEnumerator SmoothReturn()
        {
            float startTime = _currentTime;
            float elapsedTime = 0f;

            Vector3 currentTargetScale = _target.localScale;

            while (elapsedTime < _smoothReturnDuration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _smoothReturnDuration);
                _target.localScale = Vector3.Lerp(currentTargetScale, _initialScale, EasingFunctions.GetEasedValue(t, _returnCurveType));
                yield return null;
            }

            _target.localScale = _initialScale;
        }

        private void LockAxis(ref Vector3 scale)
        {
            if (_lockXScale && _lockYScale && _lockZScale)
            {
                scale = _initialScale;
                return;
            }

            if (_lockXScale)
            {
                scale.x = _initialScale.x;
            }
            if (_lockYScale)
            {
                scale.y = _initialScale.y;
            }
            if (_lockZScale)
            {
                scale.z = _initialScale.z;
            }
        }

    }

    public enum ScaleAnimation
    {
        Squish,
        Stretch,
    }
    public enum AnimationAxis
    {
        X,
        Y,
        Z,
    }
}