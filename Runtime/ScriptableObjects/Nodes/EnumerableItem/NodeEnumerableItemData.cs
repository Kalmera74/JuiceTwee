

using System;
using System.Collections.Generic;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EnumerableItem
{
    /// <summary>
    /// Represents enumerable item data for a node, including title, type, and a list of Unity objects.
    /// </summary>
    public class NodeEnumerableItemData
    {
        /// <summary>
        /// Gets the display title of this enumerable item.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the type of the enumerable item.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The list of Unity objects associated with this enumerable item.
        /// </summary>
        public List<UnityEngine.Object> Items;

        /// <summary>
        /// Constructs a new NodeEnumerableItemData instance.
        /// </summary>
        /// <param name="title">The display title for the enumerable item.</param>
        /// <param name="type">The type of the enumerable item.</param>
        public NodeEnumerableItemData(string title, Type type)
        {
            Title = title;
            Type = type;
        }
    }
}