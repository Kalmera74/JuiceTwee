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
using System.Linq;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EnumerableItem;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem;
using UnityEngine;
using UnityEngine.Events;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes
{

    /// <summary>
    /// Represents the data for a single effect node, including its target, extra items, events, and serialization helpers.
    /// </summary>
    [Serializable]
    public class EffectData
    {
        /// <summary>
        /// The effect node associated with this data.
        /// </summary>
        [SerializeReference] public EffectNode Node;

        /// <summary>
        /// The Unity object target for the node.
        /// </summary>
        public UnityEngine.Object Target;

        /// <summary>
        /// The display name of the target, or "Target" if not set.
        /// </summary>
        public string TargetName => Node?.TargetName ?? "Target";

        /// <summary>
        /// Gets the list of extra items for the editor.
        /// </summary>
        public List<NodeExtraItemData> ExtraItems => PrepareExtraItemsForEditor();

        /// <summary>
        /// Gets the list of enumerable items for the editor.
        /// </summary>
        public List<NodeEnumerableItemData> EnumerableItems => PrepareEnumerableItemsForEditor();

        /// <summary>
        /// Gets the serialized list of extra items.
        /// </summary>
        public List<EffectExtraItem> SerializedExtraItems => _effectExtraItemData;

        /// <summary>
        /// Gets the serialized list of enumerable Unity objects.
        /// </summary>
        public List<UnityEngine.Object> SerializedEnumerableItems => _enumerableItems;

        /// <summary>
        /// Gets the type of the node's target.
        /// </summary>
        public Type TargetType => Node?.TargetType ?? typeof(UnityEngine.Object);

        #region Unity Events

        /// <summary>
        /// Invoked when the effect starts.
        /// </summary>
        public UnityEvent OnStarted;

        /// <summary>
        /// Invoked when the effect stops.
        /// </summary>
        public UnityEvent OnStopped;

        /// <summary>
        /// Invoked when the effect updates.
        /// </summary>
        public UnityEvent OnUpdated;

        /// <summary>
        /// Invoked when the effect completes.
        /// </summary>
        public UnityEvent OnCompleted;

        #endregion

        [SerializeField] private List<EffectExtraItem> _effectExtraItemData = new();
        [SerializeField] private List<UnityEngine.Object> _enumerableItems = new();

        /// <summary>
        /// Serializes an extra item for this effect data.
        /// </summary>
        /// <param name="key">The key identifying the extra item.</param>
        /// <param name="unityObject">The Unity object to associate.</param>
        /// <param name="systemObject">An optional system object to serialize as a string.</param>
        public void SerializeExtraItem(string key, UnityEngine.Object unityObject, System.Object systemObject = null)
        {
            var item = _effectExtraItemData.Where(i => i.Key.Equals(key)).FirstOrDefault();
            if (item == null)
            {
                _effectExtraItemData.Add(new EffectExtraItem(
                    key,
                    unityObject,
                    systemObject?.ToString()
                ));
                return;
            }
            item.UnityObject = unityObject;
            item.SerializedSystemObject = systemObject?.ToString();
        }

        /// <summary>
        /// Prepares the list of extra items for use in the editor.
        /// </summary>
        /// <returns>A list of <see cref="NodeExtraItemData"/> for the editor, or null if not applicable.</returns>
        private List<NodeExtraItemData> PrepareExtraItemsForEditor()
        {
            if (Node == null)
            {
                return null;
            }
            if (Node is not IExtraItemNode)
            {
                return null;
            }

            IExtraItemNode extraItemNode = Node as IExtraItemNode;

            var nodeData = extraItemNode.GetExtraItems();
            if (nodeData == null)
            {
                return null;
            }

            for (int i = 0; i < nodeData.Count; i++)
            {
                NodeExtraItemData item = nodeData[i];

                var effectItem = _effectExtraItemData.FirstOrDefault(i => i.Key.Equals(item.Key));

                if (effectItem != null)
                {
                    item.UnityObject = effectItem.UnityObject;
                    item.SystemObject = effectItem.SerializedSystemObject;
                    nodeData[i] = item;
                }
            }
            return nodeData;
        }

        /// <summary>
        /// Sets the enumerable items for this effect data.
        /// </summary>
        /// <param name="items">A list of Unity objects to set as enumerable items.</param>
        public void SetEnumerableItems(List<UnityEngine.Object> items)
        {
            _enumerableItems.Clear();

            foreach (var item in items)
            {
                if (!_enumerableItems.Contains(item))
                {
                    _enumerableItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Prepares the list of enumerable items for use in the editor.
        /// </summary>
        /// <returns>A list of <see cref="NodeEnumerableItemData"/> for the editor, or null if not applicable.</returns>
        private List<NodeEnumerableItemData> PrepareEnumerableItemsForEditor()
        {
            if (Node == null)
            {
                return null;
            }
            if (Node is not IEnumerableItemNode)
            {
                return null;
            }
            var enumerableNode = Node as IEnumerableItemNode;
            List<NodeEnumerableItemData> nodeData = enumerableNode.GetEnumerableItems();
            if (nodeData == null)
            {
                return null;
            }

            List<NodeEnumerableItemData> enumerableItems = new List<NodeEnumerableItemData>();

            foreach (var item in nodeData)
            {
                var items = new NodeEnumerableItemData(item.Title, item.Type);
                items.Items = _enumerableItems;
                enumerableItems.Add(items);
            }

            return enumerableItems;
        }
    }

    /// <summary>
    /// Represents a serializable extra item for an effect, including a key, Unity object, and optional system object string.
    /// </summary>
    [Serializable]
    public class EffectExtraItem
    {
        /// <summary>
        /// The key identifying this extra item.
        /// </summary>
        public string Key;

        /// <summary>
        /// The Unity object associated with this extra item.
        /// </summary>
        public UnityEngine.Object UnityObject;

        /// <summary>
        /// The serialized system object as a string.
        /// </summary>
        public string SerializedSystemObject;

        /// <summary>
        /// Constructs a new EffectExtraItem.
        /// </summary>
        /// <param name="key">The key for the extra item.</param>
        /// <param name="unityObject">The Unity object to associate.</param>
        /// <param name="systemObject">An optional system object as a string.</param>
        public EffectExtraItem(string key, UnityEngine.Object unityObject, string systemObject = null)
        {
            Key = key;
            UnityObject = unityObject;
            SerializedSystemObject = systemObject as string;
        }
    }
}