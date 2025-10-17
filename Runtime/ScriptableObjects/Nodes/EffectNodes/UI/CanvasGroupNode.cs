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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.UINodes
{

    [Serializable]
    [NodeMenu("UI", "Canvas Group")]
    public class CanvasGroupNode : EffectNode
    {
        public override Type TargetType => typeof(CanvasGroup);

        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;

        [SerializeField] private bool _controlAlpha = true;
        [SerializeField] private bool _controlInteractable = false;
        [SerializeField] private bool _controlBlocksRaycasts = false;

        [SerializeField] private bool _useCurrentAlphaAsStart = true;
        [SerializeField, Range(0f, 1f)] private float _startAlpha = 1f;
        [SerializeField, Range(0f, 1f)] private float _endAlpha = 0f;
        [SerializeField] private AnimationCurve _alphaCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool _isInteractable = false;
        [SerializeField] private bool _blockRayCast = false;

        private CanvasGroup _target => originTarget as CanvasGroup;
        private float _actualStartAlpha;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;



        public override IEnumerator Perform()
        {
            if (_target == null)
            {
                Debug.LogError($"{_nodeName}: Target is null.");
                yield break;
            }

            _actualStartAlpha = _useCurrentAlphaAsStart ? _target.alpha : _startAlpha;

            if (_controlInteractable)
            {
                _target.interactable = _isInteractable;
            }
            if (_controlBlocksRaycasts)
            {
                _target.blocksRaycasts = _blockRayCast;
            }

            float startTime = _currentTime;
            float elapsedTime = 0;
            onStarted?.Invoke();

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                if (_controlAlpha)
                {
                    float t = Mathf.Clamp01(elapsedTime / _duration);
                    float curveT = Mathf.Clamp01(_alphaCurve.Evaluate(t));

                    _target.alpha = Mathf.Lerp(_actualStartAlpha, _endAlpha, curveT);
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_controlAlpha)
            {
                _target.alpha = _endAlpha;
            }

            onCompleted?.Invoke();
            yield return base.Perform();
        }
    }
}