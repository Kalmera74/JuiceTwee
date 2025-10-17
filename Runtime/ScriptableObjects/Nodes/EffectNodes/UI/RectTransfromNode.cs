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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.UINodes
{
    [Serializable]
    [NodeMenu("UI", "Rect Transform")]
    public class RectTransformNode : EffectNode, IExtraItemNode
    {
        public override Type TargetType => typeof(RectTransform);

        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;

        [SerializeField] private bool _controlAnchoredPosition = true;
        [SerializeField] private bool _controlSizeDelta = false;
        [SerializeField] private bool _controlRotation = false;
        [SerializeField] private bool _controlScale = false;
        [SerializeField] private bool _useTargetForValues = false;

        [SerializeField] private bool _useCurrentAsStart = true;
        [SerializeField] private bool _useSeparateAxisCurves = false;

        [SerializeField] private AnimationCurve _blendCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _xCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _yCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _zCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private Vector2 _startAnchoredPosition;
        [SerializeField] private Vector2 _endAnchoredPosition;

        [SerializeField] private Vector2 _startSizeDelta;
        [SerializeField] private Vector2 _endSizeDelta;

        [SerializeField] private Vector3 _startRotation;
        [SerializeField] private Vector3 _endRotation;

        [SerializeField] private Vector3 _startScale;
        [SerializeField] private Vector3 _endScale;

        private RectTransform _target => originTarget as RectTransform;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
        private List<NodeExtraItemData> _extraItems;


        private Vector2 _anchoredPosition;
        private Vector2 _sizeDelta;
        private Vector3 _rotation;
        private Vector3 _scale;
        private RectTransform _targetRect;


        public List<NodeExtraItemData> GetExtraItems()
        {
            if (!_useTargetForValues)
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
            new (nameof(_targetRect),"Rect Transform Target",typeof(RectTransform))
        };
            return _extraItems;
        }
        public void SetExtraItem(string key, UnityEngine.Object target)
        {
            if (key.Equals(nameof(_targetRect)))
            {
                _targetRect = target as RectTransform;
            }
        }
        public override IEnumerator Perform()
        {
            if (_target == null)
            {
                Debug.LogError("RectTransformNode: Target is null.", _player);
                yield break;
            }
            if (_useCurrentAsStart)
            {
                _anchoredPosition = _target.anchoredPosition;
                _sizeDelta = _target.sizeDelta;
                _rotation = _target.localEulerAngles;
                _scale = _target.localScale;
            }
            else
            {
                if (_useTargetForValues)
                {

                }
                else
                {
                    _anchoredPosition = _startAnchoredPosition;
                    _sizeDelta = _startSizeDelta;
                    _rotation = _startRotation;
                    _scale = _startScale;
                }
            }

            float startTime = _currentTime;
            float elapsedTime = 0;
            onStarted?.Invoke();

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_useSeparateAxisCurves)
                {
                    float x_t = _xCurve.Evaluate(t);
                    float y_t = _yCurve.Evaluate(t);
                    float z_t = _zCurve.Evaluate(t);

                    if (_controlAnchoredPosition)
                    {
                        _target.anchoredPosition = new Vector2(Mathf.Lerp(_anchoredPosition.x, _endAnchoredPosition.x, x_t), Mathf.Lerp(_anchoredPosition.y, _endAnchoredPosition.y, y_t));
                    }
                    if (_controlSizeDelta)
                    {
                        _target.sizeDelta = new Vector2(Mathf.Lerp(_sizeDelta.x, _endSizeDelta.x, x_t), Mathf.Lerp(_sizeDelta.y, _endSizeDelta.y, y_t));
                    }
                    if (_controlRotation)
                    {
                        _target.localEulerAngles = new Vector3(Mathf.Lerp(_rotation.x, _endRotation.x, x_t), Mathf.Lerp(_rotation.y, _endRotation.y, y_t), Mathf.Lerp(_rotation.z, _endRotation.z, z_t));
                    }
                    if (_controlScale)
                    {
                        _target.localScale = new Vector3(Mathf.Lerp(_scale.x, _endScale.x, x_t), Mathf.Lerp(_scale.y, _endScale.y, y_t), Mathf.Lerp(_scale.z, _endScale.z, z_t));
                    }
                }
                else
                {
                    float curveT = _blendCurve.Evaluate(t);

                    if (_controlAnchoredPosition)
                    {
                        _target.anchoredPosition = Vector2.Lerp(_anchoredPosition, _endAnchoredPosition, curveT);
                    }
                    if (_controlSizeDelta)
                    {
                        _target.sizeDelta = Vector2.Lerp(_sizeDelta, _endSizeDelta, curveT);
                    }
                    if (_controlRotation)
                    {
                        _target.localEulerAngles = Vector3.Lerp(_rotation, _endRotation, curveT);
                    }
                    if (_controlScale)
                    {
                        _target.localScale = Vector3.Lerp(_scale, _endScale, curveT);
                    }
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_controlAnchoredPosition)
            {
                _target.anchoredPosition = _endAnchoredPosition;
            }
            if (_controlSizeDelta)
            {
                _target.sizeDelta = _endSizeDelta;
            }
            if (_controlRotation)
            {
                _target.localEulerAngles = _endRotation;
            }
            if (_controlScale)
            {
                _target.localScale = _endScale;
            }

            onCompleted?.Invoke();
            yield return base.Perform();
        }
    }
}