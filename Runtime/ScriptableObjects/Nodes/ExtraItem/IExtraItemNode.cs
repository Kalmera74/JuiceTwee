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
using System.Collections.Generic;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem
{
    /// <summary>
    /// Interface for nodes that support extra item data, allowing dynamic assignment and retrieval of additional Unity or system objects.
    /// </summary>
    interface IExtraItemNode
    {
        /// <summary>
        /// Gets a list of extra item data associated with this node.
        /// </summary>
        /// <returns>A list of <see cref="NodeExtraItemData"/> objects.</returns>
        public List<NodeExtraItemData> GetExtraItems();

        /// <summary>
        /// Sets an extra Unity object for this node by key.
        /// </summary>
        /// <param name="key">The key identifying the extra item.</param>
        /// <param name="unityObject">The Unity object to assign.</param>
        public void SetExtraItem(string key, UnityEngine.Object unityObject);

    }
}