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

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.GameObjectNodes
{
    [Serializable]
    [NodeMenu("GameObject", "Activate&Deactivate")]
    public class ActivateDeactivateNode : EffectNode
    {
        public override Type TargetType => _targetMonobehaviourInstead ? typeof(MonoBehaviour) : typeof(GameObject);

        [Header("Activation Settings")]
        [SerializeField] private bool _targetMonobehaviourInstead;
        [SerializeField] private bool _activate = true;

        private GameObject _target => originTarget as GameObject;



        public override IEnumerator Perform()
        {
            onStarted?.Invoke();
            if (_targetMonobehaviourInstead)
            {
                var component = originTarget as MonoBehaviour;
                component.enabled = _activate;
            }
            else
            {
                _target.SetActive(_activate);
            }
            onUpdated?.Invoke();
            onCompleted?.Invoke();
            yield return base.Perform();
        }
    }
}