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
    /// <summary>
    /// A flow node that limits the number of times it can be activated.
    /// </summary>
    [Serializable]
    [NodeMenu("Flow", "Limit Counter")]
    public class LimitCounterNode : FlowNode
    {
        /// <summary>
        /// The maximum number of times this node can be activated.
        /// </summary>
        [SerializeField] private int _maxActivationCount = 1;

        /// <summary>
        /// The current number of activations.
        /// </summary>
        private int _currentActivationCount = 0;

        /// <summary>
        /// Performs the node's logic if the activation count is below the maximum.
        /// </summary>
        /// <returns>An enumerator for coroutine execution.</returns>
        public override IEnumerator Perform()
        {
            if (_currentActivationCount < _maxActivationCount)
            {
                _currentActivationCount++;
                yield return base.Perform();
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// Resets the activation count to zero.
        /// </summary>
        public override void ResetNode()
        {
            _currentActivationCount = 0;
        }
    }
}