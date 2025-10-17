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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects
{

        /// <summary>
        /// Represents a tree of effect nodes for sequencing and managing effects in a node-based structure.
        /// </summary>
        [Serializable]
        [CreateAssetMenu(fileName = "EffectTree", menuName = "EffectTree")]
        public class EffectTree : ScriptableObject
        {
                /// <summary>
                /// Gets a read-only list of all effect nodes in the tree.
                /// </summary>
                public IReadOnlyList<EffectNode> EffectNodes => _allNodes.OfType<EffectNode>().ToList().AsReadOnly();

                /// <summary>
                /// Gets a read-only list of all nodes in the tree.
                /// </summary>
                public IReadOnlyList<Node> AllNodes => _allNodes.AsReadOnly();

                /// <summary>
                /// Gets a read-only list of all groups in the tree.
                /// </summary>
                public IReadOnlyList<EffectTreeGroup> Groups => _groupManager.Groups;

                /// <summary>
                /// Gets the group manager for this effect tree.
                /// </summary>
                public EffectTreeGroupManager GroupManager => _groupManager;

                /// <summary>
                /// Gets the root node of the effect tree.
                /// </summary>
                public RootNode RootNode => _rootNode;

                /// <summary>
                /// Gets the effect player associated with this tree.
                /// </summary>
                public EffectPlayer Player => _player;

                [SerializeField] private RootNode _rootNode;
                [SerializeField] private EffectTreeGroupManager _groupManager = new();
                [SerializeReference] private List<Node> _allNodes = new List<Node>();
                private EffectPlayer _player;

                /// <summary>
                /// Sets the effect player for this tree and all its nodes.
                /// </summary>
                /// <param name="player">The effect player to associate.</param>
                public void SetEffectPlayer(EffectPlayer player)
                {
                        _player = player;
                        _rootNode.SetEffectPlayer(player);

                        foreach (var node in EffectNodes)
                        {
                                node.SetEffectPlayer(player);
                        }
                }

                /// <summary>
                /// Ensures the root node exists; creates it if missing.
                /// </summary>
                public void SetRootNode()
                {
                        if (_rootNode != null) { return; }

                        _rootNode = ScriptableObject.CreateInstance<RootNode>() as RootNode;
                        _rootNode.name = "RootNode";
                        _rootNode.GenerateGuid();
                        _rootNode.SetPosition(Vector2.zero);

#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(_rootNode, this);
#endif

                        _allNodes.Add(_rootNode);

#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
#endif
                }

                /// <summary>
                /// Creates a new node of the specified type and adds it to the tree.
                /// </summary>
                /// <param name="type">The type of node to create.</param>
                /// <returns>The created node.</returns>
                public Node CreateNode(Type type)
                {
                        var node = ScriptableObject.CreateInstance(type) as Node;
                        node.name = type.ToString();
                        node.GenerateGuid();

#if UNITY_EDITOR
                Undo.RecordObject(this, "Create Node");
#endif
                        _allNodes.Add(node);

#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(node, this);
                Undo.RegisterCreatedObjectUndo(node, $"Create {type.Name}");
                AssetDatabase.SaveAssets();
#endif

                        return node;
                }

                /// <summary>
                /// Removes a node from the tree and disconnects it from its parent.
                /// </summary>
                /// <param name="node">The node to remove.</param>
                public void RemoveNode(Node node)
                {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Remove Node");
#endif
                        _allNodes.Remove(node);

                        var parentOfNode = _allNodes.Where(x => x.IsParentOf(node)).FirstOrDefault();
                        if (parentOfNode != null)
                        {
                                parentOfNode.RemoveChild(node);
                        }

                        //AssetDatabase.RemoveObjectFromAsset(node);

#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(node);
                AssetDatabase.SaveAssets();
#endif
                }

                /// <summary>
                /// Adds a child node to a parent node.
                /// </summary>
                /// <param name="parent">The parent node.</param>
                /// <param name="child">The child node to add.</param>
                public void AddChild(Node parent, Node child)
                {
#if UNITY_EDITOR
                Undo.RecordObject(parent, "Add Child");
#endif
                        parent.AddChild(child);

#if UNITY_EDITOR
                EditorUtility.SetDirty(parent);
#endif
                }

                /// <summary>
                /// Removes a child node from a parent node.
                /// </summary>
                /// <param name="parent">The parent node.</param>
                /// <param name="child">The child node to remove.</param>
                public void RemoveChild(Node parent, Node child)
                {
#if UNITY_EDITOR
                Undo.RecordObject(parent, "Remove Child");
#endif
                        parent.RemoveChild(child);

#if UNITY_EDITOR
                EditorUtility.SetDirty(parent);
#endif
                }

                /// <summary>
                /// Gets the list of children for a given parent node.
                /// </summary>
                /// <param name="parent">The parent node.</param>
                /// <returns>A list of child nodes.</returns>
                public List<Node> GetChildren(Node parent)
                {
                        return parent.Children.ToList();
                }

                /// <summary>
                /// Updates the names of all nodes in the tree to match their NodeName or type.
                /// </summary>
                public void UpdateNodeNames()
                {
                        foreach (var node in _allNodes)
                        {
                                node.name = string.IsNullOrEmpty(node.NodeName) ? node.GetType().Name : node.NodeName;
                        }

#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
#endif
                }

                /// <summary>
                /// Saves the specified node, updating its name and persisting changes.
                /// </summary>
                /// <param name="node">The node to save.</param>
                public void SaveNode(Node node)
                {
                        if (node == null) { return; }

                        node.name = string.IsNullOrEmpty(node.NodeName) ? node.GetType().Name : node.NodeName;

#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
#endif
                }

                /// <summary>
                /// Resets all nodes in the tree to their initial state.
                /// </summary>
                public void ResetAllNodes()
                {
                        foreach (var node in _allNodes)
                        {
                                node.ResetNode();
                        }
                }

                /// <summary>
                /// Stops all nodes in the tree.
                /// </summary>
                public void StopAllNodes()
                {
                        foreach (var node in _allNodes)
                        {
                                node.StopNode();
                        }
                }
        }

}