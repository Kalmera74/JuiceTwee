
using System;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem
{
    /// <summary>
    /// Represents extra item data for a node, including key, title, type, and associated objects.
    /// </summary>
    [Serializable]
    public class NodeExtraItemData
    {
        /// <summary>
        /// Gets the key identifying this extra item.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the display title of this extra item.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The Unity object associated with this extra item (set by EffectData).
        /// </summary>
        public UnityEngine.Object UnityObject;

        /// <summary>
        /// The system object associated with this extra item (set by EffectData).
        /// </summary>
        public System.Object SystemObject;

        /// <summary>
        /// Gets the type of this extra item.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Constructs a new NodeExtraItemData instance.
        /// </summary>
        /// <param name="key">The key for the extra item.</param>
        /// <param name="title">The display title for the extra item.</param>
        /// <param name="type">The type of the extra item.</param>
        public NodeExtraItemData(string key, string title, Type type)
        {
            Key = key;
            Title = title;
            Type = type;
        }
    }

}