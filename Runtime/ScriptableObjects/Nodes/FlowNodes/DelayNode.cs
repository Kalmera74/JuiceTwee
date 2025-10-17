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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.FlowNodes
{
    [Serializable]
    [NodeMenu("Flow", "Delay")]
    public class DelayNode : FlowNode
    {
        [SerializeField] private float _delay;



        public override IEnumerator Perform()
        {
            yield return new WaitForSeconds(_delay);
            yield return base.Perform();
        }

    }
}