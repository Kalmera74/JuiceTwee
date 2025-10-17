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
using JuiceTwee.Runtime.Attributes;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem;
using UnityEngine;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TransformNodes
{
    [Serializable]
    [NodeMenu("Transform", "Position")]
    public class PositionNode : EffectNode, IExtraItemNode
    {

        public override Type TargetType => typeof(Transform);


        [Header("Time Options")]
        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _useSpeedInsteadOfDuration = false;
        [SerializeField] private float _speedInUnitsPerSeconds = 1f;

        [Header("Base Settings")]
        [SerializeField] private bool _usePathMovement = false;
        [SerializeField] private bool _useRelativePosition;
        [SerializeField] private bool _useLocalPosition;
        [SerializeField] private bool _snapToPosition;


        [Header("Path Settings")]
        [SerializeField] private Vector3[] _pathPoints;
        [SerializeField] private bool _useBezierPath = false;


        [Header("Position Settings")]
        [SerializeField] private bool _useSeparateAxisCurves;
        [SerializeField] private bool _useTargetForPositions = false;


        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPosition;
        [SerializeField] private AnimationCurve _positionCurve = AnimationCurve.Linear(0, 0, 1, 1);



        [SerializeField] private AnimationCurve _xPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _yPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _zPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);


        private Transform _startPositionTarget;
        private Transform _endPositionTarget;


        private Transform _target => originTarget as Transform;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
        private List<NodeExtraItemData> _extraItems;

        public List<NodeExtraItemData> GetExtraItems()
        {
            if (!_useTargetForPositions)
            {
                _extraItems = null;
                return null;
            }

            if (_extraItems != null)
            {
                return _extraItems;
            }

            _extraItems = new()
            {
                new (nameof(_startPositionTarget), "Start Position Target",typeof(Transform)),
                new (nameof(_endPositionTarget), "End Position Target",typeof(Transform)),
            };

            return _extraItems;
        }
        public void SetExtraItem(string key, UnityEngine.Object target)
        {
            if (key.Equals(nameof(_startPositionTarget)))
            {
                _startPositionTarget = target as Transform;
            }
            if (key.Equals(nameof(_endPositionTarget)))
            {
                _endPositionTarget = target as Transform;
            }
        }
        public override IEnumerator Perform()
        {
            if (_target == null) { yield break; }

            if (_usePathMovement && _pathPoints != null && _pathPoints.Length >= 2)
            {
                yield return OperatePath();
            }
            else
            {
                yield return Operate();
            }
            yield return base.Perform();
        }
        private IEnumerator Operate()
        {
            float startTime = _currentTime;

            if (_useTargetForPositions)
            {
                if (_startPositionTarget == null || _endPositionTarget == null)
                {
                    throw new NullReferenceException("Start or End target cannot be Null");
                }

                _startPosition = _startPositionTarget.position;
                _endPosition = _endPositionTarget.position;
            }

            Vector3 start = _useRelativePosition ? _target.position + _startPosition : _startPosition;
            Vector3 end = _useRelativePosition ? _target.position + _endPosition : _endPosition;
            float duration = GetDuration(start, end);

            onStarted?.Invoke();
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                Vector3 pos = CalculatePositions(start, end, t);

                if (_useLocalPosition)
                {
                    _target.localPosition = pos;
                }
                else
                {
                    _target.position = pos;
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_snapToPosition)
            {
                if (_useLocalPosition)
                {
                    _target.localPosition = _endPosition;
                }
                else
                {
                    _target.position = _endPosition;
                }
            }

            onCompleted?.Invoke();
        }
        private IEnumerator OperatePath()
        {
            onStarted?.Invoke();

            Vector3[] pathPoint = new Vector3[_pathPoints.Length];

            for (int i = 0; i < _pathPoints.Length; i++)
            {
                if (_useLocalPosition)
                {
                    pathPoint[i] = _target.InverseTransformDirection(_pathPoints[i]);
                    continue;
                }
                pathPoint[i] = _pathPoints[i];

                if (_useRelativePosition)
                {
                    pathPoint[i] += _target.position;
                }
            }

            float[] segmentLengths = new float[pathPoint.Length - 1];
            float totalLength = 0f;
            for (int i = 0; i < segmentLengths.Length; i++)
            {
                segmentLengths[i] = Vector3.Distance(pathPoint[i], pathPoint[i + 1]);
                totalLength += segmentLengths[i];
            }

            float duration = _useSpeedInsteadOfDuration && _speedInUnitsPerSeconds > 0 ? totalLength / _speedInUnitsPerSeconds : _duration;
            float startTime = _currentTime;

            while (_currentTime - startTime < duration)
            {
                float t = (_currentTime - startTime) / duration;
                Vector3 position = _useBezierPath ? GetBezierPoint(pathPoint, t) : GetLinearPathPoint(pathPoint, t);

                if (_useLocalPosition)
                {
                    _target.localPosition = position;
                }
                else
                {
                    _target.position = position;
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_snapToPosition)
            {
                Vector3 finalPos = pathPoint[^1];
                if (_useLocalPosition)
                {
                    _target.localPosition = finalPos;
                }
                else
                {
                    _target.position = finalPos;
                }
            }

            onCompleted?.Invoke();
        }
        private Vector3 GetBezierPoint(Vector3[] points, float t)
        {
            while (points.Length > 1)
            {
                Vector3[] temp = new Vector3[points.Length - 1];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = Vector3.Lerp(points[i], points[i + 1], t);
                points = temp;
            }
            return points[0];
        }

        private Vector3 GetLinearPathPoint(Vector3[] points, float t)
        {
            float totalLength = 0f;
            float[] lengths = new float[points.Length - 1];

            for (int i = 0; i < lengths.Length; i++)
            {
                lengths[i] = Vector3.Distance(points[i], points[i + 1]);
                totalLength += lengths[i];
            }

            float traveled = t * totalLength;
            float cumulative = 0f;

            for (int i = 0; i < lengths.Length; i++)
            {
                if (traveled <= cumulative + lengths[i])
                {
                    float localT = (traveled - cumulative) / lengths[i];
                    return Vector3.Lerp(points[i], points[i + 1], localT);
                }
                cumulative += lengths[i];
            }

            return points[^1];
        }

        private Vector3 CalculatePositions(Vector3 start, Vector3 end, float t)
        {
            if (!_useSeparateAxisCurves)
            {
                return Vector3.Lerp(start, end, _positionCurve.Evaluate(t));
            }

            float x = Mathf.Lerp(start.x, end.x, _xPositionCurve.Evaluate(t));
            float y = Mathf.Lerp(start.y, end.y, _yPositionCurve.Evaluate(t));
            float z = Mathf.Lerp(start.z, end.z, _zPositionCurve.Evaluate(t));

            return new Vector3(x, y, z);
        }
        private float GetDuration(Vector3 start, Vector3 end)
        {
            if (_useSpeedInsteadOfDuration && _speedInUnitsPerSeconds > 0f)
            {
                return Vector3.Distance(start, end) / _speedInUnitsPerSeconds;
            }

            return _duration;
        }

    }
}