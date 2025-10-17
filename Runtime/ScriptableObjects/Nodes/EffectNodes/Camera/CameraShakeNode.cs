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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.CameraNodes
{
    [Serializable]
    [NodeMenu("Camera", "Camera Shake")]
    public class CameraShakeNode : EffectNode
    {
        public override Type TargetType => typeof(Camera);

        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private bool _useUnscaledTime = false;

        [SerializeField] private bool _returnToOriginalAfterShaking = true;
        [SerializeField] private bool _shakeRotation = false;
        [SerializeField] private bool _useWorldSpace = false;


        [SerializeField] private float _rotationMagnitude = 1f;
        [SerializeField] private float _shakeMagnitude = 0.1f;
        [SerializeField] private float _shakeRoughness = 5f;
        [SerializeField] private Vector3 _shakeDirection = Vector3.one;
        [SerializeField] private AnimationCurve _magnitudeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [SerializeField] private bool _lockXPosition = false;
        [SerializeField] private bool _lockYPosition = false;
        [SerializeField] private bool _lockZPosition = false;

        [SerializeField] private bool _lockXRotation = false;
        [SerializeField] private bool _lockYRotation = false;
        [SerializeField] private bool _lockZRotation = false;

        [SerializeField] private bool _smoothReturn = true;
        [SerializeField] private float _smoothReturnDuration = 0.2f;
        [SerializeField] private AnimationCurve _smoothReturnCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);



        private Camera _target => originTarget as Camera;
        private Vector3 _originalCameraPosition;
        private Vector3 _positionShakeOffset = new Vector3();
        private Quaternion _originalCameraRotation;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            if (_target == null)
            {
                Debug.LogWarning($"{NodeName} Target Camera is null. Cannot perform shake.");
                yield break;
            }

            _originalCameraPosition = _target.transform.localPosition;
            _originalCameraRotation = _target.transform.localRotation;

            onStarted?.Invoke();

            float startTime = _currentTime;
            float elapsedTime = _currentTime - startTime;

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                float currentMagnitude = _shakeMagnitude * _magnitudeCurve.Evaluate(t);

                float offsetX = _lockXPosition ? 0 : (Mathf.PerlinNoise(0f, _currentTime * _shakeRoughness) * 2 - 1) * currentMagnitude * _shakeDirection.x;
                float offsetY = _lockYPosition ? 0 : (Mathf.PerlinNoise(1f, _currentTime * _shakeRoughness) * 2 - 1) * currentMagnitude * _shakeDirection.y;
                float offsetZ = _lockZPosition ? 0 : (Mathf.PerlinNoise(2f, _currentTime * _shakeRoughness) * 2 - 1) * currentMagnitude * _shakeDirection.z;

                _positionShakeOffset.x = offsetX;
                _positionShakeOffset.y = offsetY;
                _positionShakeOffset.z = offsetZ;

                if (_useWorldSpace)
                {
                    _target.transform.position = _originalCameraPosition + _positionShakeOffset;
                }
                else
                {
                    _target.transform.localPosition = _originalCameraPosition + _positionShakeOffset;
                }

                if (_shakeRotation)
                {
                    float rotX = _lockXRotation ? 0 : (Mathf.PerlinNoise(3f, _currentTime * _shakeRoughness) * 2 - 1) * _rotationMagnitude;
                    float rotY = _lockYRotation ? 0 : (Mathf.PerlinNoise(4f, _currentTime * _shakeRoughness) * 2 - 1) * _rotationMagnitude;
                    float rotZ = _lockZRotation ? 0 : (Mathf.PerlinNoise(5f, _currentTime * _shakeRoughness) * 2 - 1) * _rotationMagnitude;

                    Quaternion shakeRot = Quaternion.Euler(rotX, rotY, rotZ);
                    _target.transform.localRotation = _originalCameraRotation * shakeRot;
                }


                yield return null;
                onUpdated?.Invoke();
            }

            if (_returnToOriginalAfterShaking)
            {
                if (_smoothReturn && _smoothReturnDuration > 0)
                {
                    Vector3 currentPos = _target.transform.localPosition;
                    Quaternion currentRot = _target.transform.localRotation;

                    float smoothReturnStartTime = _currentTime;
                    float smoothReturnElapsedTime = 0f;

                    while (smoothReturnElapsedTime < _smoothReturnDuration)
                    {
                        smoothReturnElapsedTime = _currentTime - smoothReturnStartTime;
                        float t = Mathf.Clamp01(smoothReturnElapsedTime / _smoothReturnDuration);

                        if (_useWorldSpace)
                        {
                            _target.transform.position = Vector3.Lerp(currentPos, _originalCameraPosition, _smoothReturnCurve.Evaluate(t));
                        }
                        else
                        {
                            _target.transform.localPosition = Vector3.Lerp(currentPos, _originalCameraPosition, _smoothReturnCurve.Evaluate(t));
                        }

                        _target.transform.localRotation = Quaternion.Slerp(currentRot, _originalCameraRotation, _smoothReturnCurve.Evaluate(t));

                        yield return null;
                        onUpdated?.Invoke();
                    }
                }

                _target.transform.localPosition = _originalCameraPosition;
                _target.transform.localRotation = _originalCameraRotation;
            }

            onCompleted?.Invoke();
        }

    }
}