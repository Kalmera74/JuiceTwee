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

using System.Collections.Generic;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EnumerableItem
{


    /// <summary>
    /// Interface for nodes that support enumerable item data, allowing dynamic assignment and retrieval of lists of Unity objects.
    /// </summary>
    interface IEnumerableItemNode
    {
        /// <summary>
        /// Gets a list of enumerable item data associated with this node.
        /// </summary>
        /// <returns>A list of <see cref="NodeEnumerableItemData"/> objects.</returns>
        public List<NodeEnumerableItemData> GetEnumerableItems();

        /// <summary>
        /// Sets the enumerable Unity objects for this node.
        /// </summary>
        /// <param name="items">The list of Unity objects to assign.</param>
        public void SetEnumerableItems(List<UnityEngine.Object> items);
    }
}