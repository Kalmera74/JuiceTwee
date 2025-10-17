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
using JuiceTwee.Runtime.ScriptableObjects.Nodes;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects
{

    /// <summary>
    /// Manages groups of nodes within an EffectTree, allowing for creation, removal, and node assignment to groups.
    /// </summary>
    [Serializable]
    public class EffectTreeGroupManager
    {
        /// <summary>
        /// Gets a read-only list of all groups managed by this manager.
        /// </summary>
        public IReadOnlyList<EffectTreeGroup> Groups => _groups.AsReadOnly();

        [SerializeField] private List<EffectTreeGroup> _groups = new List<EffectTreeGroup>();

        /// <summary>
        /// Adds a new group with the specified rectangle and name.
        /// </summary>
        /// <param name="rect">The rectangle defining the group's position and size.</param>
        /// <param name="name">The name of the group.</param>
        public void AddGroup(Rect rect, string name)
        {
            if (_groups.Any(x => x.Rect.Contains(rect.center)))
            {
                Debug.LogWarning("Group already exists at this position");
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "New Group";
            }
            if (_groups.Any(x => x.Title.ToLower().Equals(name.ToLower())))
            {
                Debug.LogWarning($"Group with name {name} already exists");
                return;
            }

            _groups.Add(new EffectTreeGroup(rect, name));
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        }

        /// <summary>
        /// Removes a group by its name.
        /// </summary>
        /// <param name="groupName">The name of the group to remove.</param>
        public void RemoveGroup(string groupName)
        {
            var group = _groups.FirstOrDefault(x => x.Title.ToLower().Equals(groupName.ToLower()));
            if (group == null)
            {
                Debug.LogWarning($"Group {groupName} not found");
                return;
            }
            RemoveGroup(group);
        }

        /// <summary>
        /// Removes the specified group.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        public void RemoveGroup(EffectTreeGroup group)
        {
            if (!_groups.Contains(group)) { return; }

            _groups.Remove(group);
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        }

        /// <summary>
        /// Adds a node to a group by group name.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="node">The node to add.</param>
        public void AddNodeToGroup(string groupName, Node node)
        {
            if (node == null) { return; }

            var group = _groups.FirstOrDefault(x => x.Title.ToLower().Equals(groupName.ToLower()));
            if (group == null)
            {
                Debug.LogWarning($"Group {groupName} not found");
                return;
            }
            if (group.NodeIds.Contains(node.Id)) { return; }
            group.NodeIds.Add(node.Id);

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        }

        /// <summary>
        /// Removes a node from a group by group name.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="node">The node to remove.</param>
        public void RemoveNodeFromGroup(string groupName, Node node)
        {
            if (node == null) { return; }

            var group = _groups.FirstOrDefault(x => x.Title.ToLower().Equals(groupName.ToLower()));
            if (group == null)
            {
                Debug.LogWarning($"Group {groupName} not found");
                return;
            }
            RemoveNodeFromGroup(group, node);
        }

        /// <summary>
        /// Removes a node from the specified group.
        /// </summary>
        /// <param name="group">The group to remove the node from.</param>
        /// <param name="node">The node to remove.</param>
        public void RemoveNodeFromGroup(EffectTreeGroup group, Node node)
        {
            if (!group.NodeIds.Contains(node.Id)) { return; }
            group.NodeIds.Remove(node.Id);
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        }

        /// <summary>
        /// Updates the name of a group.
        /// </summary>
        /// <param name="oldName">The current name of the group.</param>
        /// <param name="newName">The new name for the group.</param>
        public void UpdateGroupName(string oldName, string newName)
        {
            var group = _groups.FirstOrDefault(x => x.Title.ToLower().Equals(oldName.ToLower()));
            if (group == null)
            {
                Debug.LogWarning($"Group {oldName} not found");
                return;
            }
            if (string.IsNullOrEmpty(newName))
            {
                Debug.LogWarning($"Cannot change the name of group {oldName} because new name is null or empty");
            }

            group.Title = newName;
        }

        /// <summary>
        /// Determines whether the specified node is in any group.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node is in a group; otherwise, false.</returns>
        public bool IsNodeInAGroup(Node node)
        {
            return _groups.Any(g => g.NodeIds.Contains(node.Id));
        }

        /// <summary>
        /// Removes a node from its group, if it belongs to one.
        /// </summary>
        /// <param name="node">The node to remove from its group.</param>
        /// <returns>The group the node was removed from, or null if not found.</returns>
        public EffectTreeGroup RemoveNodeFromGroup(Node node)
        {
            var group = _groups.FirstOrDefault(g => g.NodeIds.Contains(node.Id));
            if (group == null)
            {
                Debug.LogWarning($"{node.NodeName} does no belong to any group to be removed from");
                return null;
            }

            RemoveNodeFromGroup(group, node);
            return group;
        }
    }

    /// <summary>
    /// Represents a group of nodes in the EffectTree, including its title, position, and contained node IDs.
    /// </summary>
    [Serializable]
    public class EffectTreeGroup
    {
        /// <summary>
        /// The title of the group.
        /// </summary>
        public string Title;
        /// <summary>
        /// The rectangle defining the group's position and size.
        /// </summary>
        public Rect Rect;
        /// <summary>
        /// The unique identifier for the group.
        /// </summary>
        public string Id;
        /// <summary>
        /// The list of node IDs contained in this group.
        /// </summary>
        public List<string> NodeIds = new();

        /// <summary>
        /// Constructs a new EffectTreeGroup with the specified rectangle and title.
        /// </summary>
        /// <param name="rect">The rectangle for the group.</param>
        /// <param name="title">The title of the group.</param>
        public EffectTreeGroup(Rect rect, string title)
        {
            Rect = rect;
            Title = title;
#if UNITY_EDITOR
        Id = GUID.Generate().ToString();
#endif
        }
    }
}