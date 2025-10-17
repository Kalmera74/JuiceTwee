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


namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.ParticleNodes
{
    [Serializable]
    [NodeMenu("Particle System", "Particle Player")]
    public class ParticleSystemNode : EffectNode
    {
        public override Type TargetType => typeof(ParticleSystem);

        [SerializeField] private bool _play = true;

        private ParticleSystem _target => originTarget as ParticleSystem;
        public override IEnumerator Perform()
        {

            onStarted?.Invoke();
            if (_play)
            {
                _target.Play();
            }
            else
            {
                _target.Stop();
            }
            onUpdated?.Invoke();
            onCompleted?.Invoke();
            yield return base.Perform();
        }
    }
}