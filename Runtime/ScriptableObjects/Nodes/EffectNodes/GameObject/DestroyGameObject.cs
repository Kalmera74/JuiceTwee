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
    [NodeMenu("GameObject", "Destroy")]
    public class DestroyGameObject : EffectNode
    {
        public override Type TargetType => typeof(GameObject);
        private GameObject _target => originTarget as GameObject;


        public override IEnumerator Perform()
        {
            onStarted?.Invoke();
            Destroy(_target);
            onUpdated?.Invoke();
            onCompleted?.Invoke();
            yield return base.Perform();
        }
    }
}