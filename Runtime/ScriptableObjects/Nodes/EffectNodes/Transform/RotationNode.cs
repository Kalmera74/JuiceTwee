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

using UnityEngine;
using System.Collections;
using System;
using JuiceTwee.Runtime.Attributes;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TransformNodes
{
    [Serializable]
    [NodeMenu("Transform", "Rotation")]
    public class RotationNode : EffectNode
    {
        public override Type TargetType => typeof(Transform);

        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _useSpeedInsteadOfDuration;


        [SerializeField] private bool _useRelativeRotation;
        [SerializeField] private bool _useLocalRotation;
        [SerializeField] private bool _snapToRotation;
        [SerializeField] private bool _useSeparateAxisCurves;

        [SerializeField] private Vector3 _startRotation;
        [SerializeField] private Vector3 _endRotation;
        [SerializeField] private float _rotationSpeed = 90f;
        [SerializeField] private AnimationCurve _rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _xRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _yRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _zRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

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

            Vector3 startEuler = _startRotation;
            Vector3 endEuler = _endRotation;

            if (_useRelativeRotation)
            {
                Vector3 baseEuler = _useLocalRotation ? _target.localRotation.eulerAngles : _target.rotation.eulerAngles;
                startEuler += baseEuler;
                endEuler += baseEuler;
            }

            Quaternion startRot = Quaternion.Euler(startEuler);
            Quaternion endRot = Quaternion.Euler(endEuler);

            if (_useSpeedInsteadOfDuration)
            {
                if (Mathf.Approximately(_rotationSpeed, 0f))
                {
                    _duration = 0f;
                }
                else
                {
                    float angle = Quaternion.Angle(startRot, endRot);
                    _duration = Mathf.Abs(angle / _rotationSpeed);
                }
            }

            if (Mathf.Approximately(_duration, 0f))
            {
                if (_snapToRotation)
                {
                    if (_useLocalRotation) _target.localRotation = endRot;
                    else _target.rotation = endRot;
                }
                onCompleted?.Invoke();
                yield break;
            }

            float endTime = startTime + _duration;
            float currentTime = startTime;

            Quaternion totalRotationDelta = Quaternion.Inverse(startRot) * endRot;
            Vector3 totalEulerDiff = totalRotationDelta.eulerAngles;

            if (totalEulerDiff.x > 180f) totalEulerDiff.x -= 360f;
            if (totalEulerDiff.y > 180f) totalEulerDiff.y -= 360f;
            if (totalEulerDiff.z > 180f) totalEulerDiff.z -= 360f;

            onStarted?.Invoke();

            while (currentTime < endTime)
            {
                currentTime = _currentTime;
                float t = Mathf.Clamp01((currentTime - startTime) / _duration);

                Quaternion newRotation;
                if (_useSeparateAxisCurves)
                {
                    float easedX = _xRotationCurve.Evaluate(t);
                    float easedY = _yRotationCurve.Evaluate(t);
                    float easedZ = _zRotationCurve.Evaluate(t);

                    Quaternion rotX = Quaternion.AngleAxis(totalEulerDiff.x * easedX, Vector3.right);
                    Quaternion rotY = Quaternion.AngleAxis(totalEulerDiff.y * easedY, Vector3.up);
                    Quaternion rotZ = Quaternion.AngleAxis(totalEulerDiff.z * easedZ, Vector3.forward);

                    newRotation = startRot * rotY * rotX * rotZ;
                }
                else
                {
                    float evaluatedT = _rotationCurve.Evaluate(t);
                    newRotation = Quaternion.Slerp(startRot, endRot, evaluatedT);
                }

                if (_useLocalRotation)
                {
                    _target.localRotation = newRotation;
                }
                else
                {
                    _target.rotation = newRotation;
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_snapToRotation)
            {
                if (_useLocalRotation)
                {
                    _target.localRotation = endRot;
                }
                else
                {
                    _target.rotation = endRot;
                }
            }

            onCompleted?.Invoke();
        }
    }
}