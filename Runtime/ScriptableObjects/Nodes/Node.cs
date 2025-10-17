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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes
{
    /// <summary>
    /// Abstract base class for all nodes in the effect tree. 
    /// Provides core functionality for child management, execution, and editor integration.
    /// </summary>
    [Serializable]
    public abstract class Node : ScriptableObject
    {
        /// <summary>
        /// Gets a read-only list of this node's children.
        /// </summary>
        public IReadOnlyList<Node> Children => _children.AsReadOnly();

        /// <summary>
        /// Gets the player (MonoBehaviour) associated with this node.
        /// </summary>
        public MonoBehaviour Player => _player;

        /// <summary>
        /// Gets the unique identifier (GUID) for this node.
        /// </summary>
        public string Id => _guid;

        /// <summary>
        /// Gets the position of this node in the editor.
        /// </summary>
        public Vector2 Position => _position;

        /// <summary>
        /// Gets the display name of this node.
        /// </summary>
        public string NodeName => _nodeName;

        [SerializeField] protected string _nodeName;
        [SerializeReference, HideInInspector] protected List<Node> _children = new List<Node>();
        [SerializeField, HideInInspector] private string _guid;
        [SerializeField, HideInInspector] private Vector2 _position = Vector2.zero;

        /// <summary>
        /// The player (MonoBehaviour) associated with this node and its children.
        /// </summary>
        protected MonoBehaviour _player;

        /// <summary>
        /// Event invoked when the node is updated (typically in the editor).
        /// </summary>
        public virtual event Action<Node> OnUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            _nodeName = GetType().Name;
        }

        /// <summary>
        /// Sets the effect player for this node and all its children.
        /// </summary>
        /// <param name="player">The MonoBehaviour to associate as the player.</param>
        public void SetEffectPlayer(MonoBehaviour player)
        {
            _player = player;
            foreach (var node in _children)
            {
                node._player = player;
            }
        }

        /// <summary>
        /// Called by Unity when the script is loaded or a value changes in the inspector.
        /// Ensures the node name is set and invokes the <see cref="OnUpdate"/> event.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(_nodeName))
            {
                _nodeName = GetType().Name;
            }

            OnUpdate?.Invoke(this);

            // name = _nodeName;
            // EditorUtility.SetDirty(this);
            // AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Sets the position of this node in the editor.
        /// </summary>
        /// <param name="position">The new position.</param>
        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        /// <summary>
        /// Generates a new GUID for this node (editor only).
        /// </summary>
        public void GenerateGuid()
        {
#if UNITY_EDITOR
        _guid = GUID.Generate().ToString();
#endif
        }

        /// <summary>
        /// Executes this node and all its children as a coroutine.
        /// </summary>
        /// <returns>An enumerator for coroutine execution.</returns>
        public virtual IEnumerator Perform()
        {
            foreach (var node in _children)
            {
                _player.StartCoroutine(node.Perform());
            }

            yield return null;
        }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        /// <param name="node">The child node to add.</param>
        public virtual void AddChild(Node node)
        {
            if (_children.Contains(node)) { return; }

            _children.Add(node);
        }

        /// <summary>
        /// Removes a child node from this node.
        /// </summary>
        /// <param name="node">The child node to remove.</param>
        public virtual void RemoveChild(Node node)
        {
            _children.Remove(node);
        }

        /// <summary>
        /// Determines whether this node is the parent of the specified node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if this node is the parent; otherwise, false.</returns>
        public bool IsParentOf(Node node)
        {
            return _children.Contains(node);
        }

        /// <summary>
        /// Stops all coroutines running on the player.
        /// </summary>
        public virtual void StopNode()
        {
            _player.StopAllCoroutines();
        }

        /// <summary>
        /// Resets this node to its initial state. Override in derived classes for custom reset logic.
        /// </summary>
        public virtual void ResetNode() { }
    }
}