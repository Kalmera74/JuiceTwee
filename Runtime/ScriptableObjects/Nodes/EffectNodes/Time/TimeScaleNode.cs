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
    [NodeMenu("Time", "Time Scale")]
    public class TimeScaleNode : EffectNode
    {
        public override Type TargetType => null;


        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _ignoreDuration = false;
        [SerializeField, Range(0, 1)] private float _timeScale;


        public override IEnumerator Perform()
        {
            float currentTimeScale = Time.timeScale;
            Time.timeScale = _timeScale;
            if (!_ignoreDuration)
            {
                yield return new WaitForSecondsRealtime(_duration);
                Time.timeScale = currentTimeScale;
            }

            yield return base.Perform();
        }

    }
}