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
using JuiceTwee.Runtime.ScriptableObjects;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EnumerableItem;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem;
using UnityEngine;
using UnityEngine.Events;
namespace JuiceTwee.Runtime
{

    /// <summary>
    /// Specifies when the EffectPlayer should automatically start playing.
    /// </summary>
    [Serializable]
    public enum AutoStartOptions
    {
        /// <summary>Play effects when the component is enabled.</summary>
        OnEnable,
        /// <summary>Play effects during Awake.</summary>
        OnAwake,
        /// <summary>Play effects during Start.</summary>
        OnStart
    }

    /// <summary>
    /// Plays and manages an EffectTree, initializing nodes and handling their lifecycle.
    /// </summary>
    public class EffectPlayer : MonoBehaviour
    {
        /// <summary>
        /// Invoked when the effect playback is stopped.
        /// </summary>
        public UnityEvent OnStopped;

        [SerializeField]
        [Tooltip("If true, the effect will play automatically based on the selected auto start option.")]
        private bool _autoPlay = false;

        [SerializeField]
        [Tooltip("Determines when the effect should automatically start.")]
        private AutoStartOptions _autoStartOn = AutoStartOptions.OnStart;

        [SerializeField]
        [Tooltip("The EffectTree asset to play.")]
        private EffectTree _effectTree;

        [SerializeReference]
        [Tooltip("List of effect data, each containing a node and its configuration.")]
        private List<EffectData> _effectData = new List<EffectData>();



        /// <summary>
        /// Gets the list of effect nodes from the effect tree.
        /// </summary>
        public IReadOnlyList<EffectNode> _nodes => _effectTree.EffectNodes;

        private bool _hasInitialized = false;

        /// <summary>
        /// Unity OnEnable callback. Initializes the tree and auto-plays if configured.
        /// </summary>
        private void OnEnable()
        {

            InitTree();
            if (_autoPlay && _autoStartOn == AutoStartOptions.OnEnable)
            {
                Play();
            }
        }

        /// <summary>
        /// Unity Awake callback. Initializes the tree and auto-plays if configured.
        /// </summary>
        private void Awake()
        {
            InitTree();

            if (_autoPlay && _autoStartOn == AutoStartOptions.OnAwake)
            {
                Play();
            }
        }

        /// <summary>
        /// Unity Start callback. Initializes the tree and auto-plays if configured.
        /// </summary>
        private void Start()
        {

            InitTree();
            if (_autoPlay && _autoStartOn == AutoStartOptions.OnStart)
            {
                Play();
            }
        }

        /// <summary>
        /// Initializes the effect tree and configures all effect nodes with their targets and extra items.
        /// </summary>
        private void InitTree()
        {
            if (_hasInitialized) { return; }

            _effectTree.SetEffectPlayer(this);

            foreach (var data in _effectData)
            {
                data.Node.SetTarget(data.Target);

                var extraItems = data.SerializedExtraItems;
                foreach (var extra in extraItems)
                {
                    if (data.Node is not IExtraItemNode)
                    {
                        continue;
                    }
                    var extraItemNode = data.Node as IExtraItemNode;
                    extraItemNode.SetExtraItem(extra.Key, extra.UnityObject);
                }
                var enumerableItems = data.SerializedEnumerableItems;
                if (data.Node is not IEnumerableItemNode)
                {
                    continue;
                }
                var enumerableNode = data.Node as IEnumerableItemNode;
                enumerableNode.SetEnumerableItems(enumerableItems);
            }

            _hasInitialized = true;
        }

        /// <summary>
        /// Starts playing the effect tree from the root node, registering events and initializing nodes.
        /// </summary>
        public void Play()
        {
            var _rootNode = _effectTree.RootNode;

            foreach (var data in _effectData)
            {
                data.Node.RegisterOnStarted(data.OnStarted);
                data.Node.RegisterOnUpdated(data.OnUpdated);
                data.Node.RegisterOnCompleted(data.OnCompleted);

                data.Node.InitializeNodeBeforeRunning();
            }

            StartCoroutine(_rootNode.Perform());
        }

        /// <summary>
        /// Stops all nodes in the effect tree, resets them, and invokes the OnStopped event.
        /// </summary>
        public void Stop()
        {
            _effectTree.StopAllNodes();
            _effectTree.ResetAllNodes();
            OnStopped?.Invoke();
        }

        /// <summary>
        /// Unity OnDestroy callback. Stops all effects when the component is destroyed.
        /// </summary>
        void OnDestroy()
        {
            Stop();
        }



#if UNITY_EDITOR
        /// <summary>
        /// Updates the effect data list to match the nodes in the effect tree. Used in the editor.
        /// </summary>
        public void UpdateEffectData()
        {
            if (_effectTree == null)
            {
                return;
            }
            if (_effectTree.EffectNodes.Count == 0)
            {
                _effectData.Clear();
                return;
            }


            if (_effectData.Count != _effectTree.EffectNodes.Count)
            {
                if (_effectData.Count > _effectTree.EffectNodes.Count)
                {
                    var removedNodes = _effectData.Select(x => x.Node).Except(_effectTree.EffectNodes).ToList();
                    foreach (var node in removedNodes)
                    {
                        _effectData.RemoveAll(x => x.Node == node);
                    }
                }


                if (_effectData.Count < _effectTree.EffectNodes.Count)
                {
                    var addedNodes = _effectTree.EffectNodes.Except(_effectData.Select(x => x.Node)).ToList();
                    foreach (var node in addedNodes)
                    {
                        AddNodeToEffectList(node);
                    }
                }

            }

        }

        /// <summary>
        /// Adds a new node to the effect data list.
        /// </summary>
        /// <param name="node">The node to add.</param>
        private void AddNodeToEffectList(EffectNode node)
        {
            var data = new EffectData
            {
                Node = node,
            };

            _effectData.Add(data);
        }

#endif
    }
}