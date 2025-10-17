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
    /// A flow node that counts activations and triggers its action after a specified number of activations.
    /// Optionally, it can be limited to activate only once.
    /// </summary>
    [Serializable]
    [NodeMenu("Flow", "Activation Counter")]
    public class ActivationCounterNode : FlowNode
    {
        /// <summary>
        /// The number of activations required to trigger the action.
        /// </summary>
        [SerializeField] private int _countToActivate = 1;

        /// <summary>
        /// If true, the node will only activate once, even if the activation count is reached again.
        /// </summary>
        [SerializeField] private bool _limitActivationToOnce;

        /// <summary>
        /// The current activation count.
        /// </summary>
        private int _currentCount = 0;

        /// <summary>
        /// Indicates whether the action has already been performed (when limited to once).
        /// </summary>
        private bool _hasPerformedAction = false;

        /// <summary>
        /// Performs the node's logic if the activation count is reached and, if limited, only once.
        /// </summary>
        /// <returns>An enumerator for coroutine execution.</returns>
        public override IEnumerator Perform()
        {
            if (_hasPerformedAction && _limitActivationToOnce)
            {
                yield break;
            }

            _currentCount++;
            if (_currentCount >= _countToActivate)
            {
                _hasPerformedAction = true;
                yield return base.Perform();
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// Resets the activation count and performed state.
        /// </summary>
        public override void ResetNode()
        {
            _currentCount = 0;
            _hasPerformedAction = false;
        }
    }
}