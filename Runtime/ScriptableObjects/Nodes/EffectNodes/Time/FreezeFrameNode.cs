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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TimeNodes
{
    [Serializable]
    [NodeMenu("Time", "Freeze Frame")]
    public class FreezeFrameNode : EffectNode
    {
        public override Type TargetType => null;

        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;


        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
        public override IEnumerator Perform()
        {
            _useUnscaledTime = true;
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            float currentTimeScale = Time.timeScale;
            float startTime = _currentTime;
            float elapsedTime = 0;

            onStarted?.Invoke();
            Time.timeScale = 0;
            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                onUpdated?.Invoke();
                yield return null;
            }
            Time.timeScale = currentTimeScale;
            onCompleted?.Invoke();
        }
    }
}