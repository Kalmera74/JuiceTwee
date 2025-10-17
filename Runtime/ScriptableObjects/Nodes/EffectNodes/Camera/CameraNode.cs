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
    [NodeMenu("Camera", "Camera")]
    public class CameraNode : EffectNode
    {
        public override Type TargetType => typeof(Camera);

        [Header("Time Options")]
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _useUnscaledTime = false;

        [Header("Camera Settings")]
        [SerializeField] private CameraProjectionType _projection;

        [Header("Perspective Settings")]
        [SerializeField] private bool _useCurrentFovAsStarting = false;
        [SerializeField] private float _startingFOV = 60;
        [SerializeField] private float _endFOV = 60;
        [SerializeField] private AnimationCurve _fovCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Orthographic Settings")]
        [SerializeField] private bool _useCurrentSizeAsStarting = false;
        [SerializeField] private float _startingSize = 5;
        [SerializeField] private float _endSize = 5;
        [SerializeField] private AnimationCurve _sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Camera _target => originTarget as Camera;
        private bool _isPerspective => _projection == CameraProjectionType.Perspective;
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
                Debug.LogError($"{NodeName} Target Camera is null. Cannot perform operation");
                yield break;
            }

            if (_useCurrentFovAsStarting)
            {
                _startingFOV = _target.fieldOfView;
            }
            else if (_useCurrentSizeAsStarting)
            {
                _startingSize = _target.orthographicSize;
            }

            onStarted?.Invoke();

            _target.orthographic = !_isPerspective;

            float startTime = _currentTime;
            float elapsedTime = _currentTime - startTime;
            while (elapsedTime < _duration)
            {

                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_isPerspective)
                {
                    _target.fieldOfView = Mathf.Lerp(_startingFOV, _endFOV, _fovCurve.Evaluate(t));
                }
                else
                {
                    _target.orthographicSize = Mathf.Lerp(_startingSize, _endSize, _sizeCurve.Evaluate(t));
                }
                yield return null;
                onUpdated?.Invoke();
            }

            onCompleted?.Invoke();

        }
    }
    public enum CameraProjectionType
    {
        Perspective,
        Orthographic
    }
}