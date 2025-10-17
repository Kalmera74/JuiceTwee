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
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes
{
    /// <summary>
    /// Abstract base class for all effect nodes in the effect tree.
    /// Provides core functionality for target assignment, event registration, and node initialization.
    /// </summary>
    [Serializable]
    public abstract class EffectNode : Node
    {
        /// <summary>
        /// Gets the type of the target object this node operates on.
        /// </summary>
        public abstract Type TargetType { get; }

        /// <summary>
        /// Gets the display name of the target for this node.
        /// </summary>
        public virtual string TargetName => null;

        /// <summary>
        /// The original Unity object target for this node.
        /// </summary>
        [SerializeField, HideInInspector] protected Object originTarget;

        /// <summary>
        /// Event invoked when the node starts execution.
        /// </summary>
        protected UnityEvent onStarted;

        /// <summary>
        /// Event invoked when the node is updated.
        /// </summary>
        protected UnityEvent onUpdated;

        /// <summary>
        /// Event invoked when the node completes execution.
        /// </summary>
        protected UnityEvent onCompleted;

        /// <summary>
        /// Sets the Unity object target for this node.
        /// </summary>
        /// <param name="target">The Unity object to assign as the target.</param>
        public virtual void SetTarget(Object target)
        {
            if (target == null) { return; }
            originTarget = target;
        }

        /// <summary>
        /// Called before the node runs, for any required initialization.
        /// </summary>
        public virtual void InitializeNodeBeforeRunning() { }

        /// <summary>
        /// Called when the target is changed during editor play mode.
        /// </summary>
        public virtual void OnTargetChangedDuringEditorPlay() { }

        /// <summary>
        /// Registers a UnityEvent to be invoked when the node starts.
        /// </summary>
        /// <param name="onStarted">The UnityEvent to register.</param>
        public void RegisterOnStarted(UnityEvent onStarted)
        {
            this.onStarted = onStarted;
        }

        /// <summary>
        /// Deregisters the onStarted event.
        /// </summary>
        public void DeregisterOnStarted()
        {
            onStarted = null;
        }

        /// <summary>
        /// Registers a UnityEvent to be invoked when the node updates.
        /// </summary>
        /// <param name="onUpdated">The UnityEvent to register.</param>
        public void RegisterOnUpdated(UnityEvent onUpdated)
        {
            this.onUpdated = onUpdated;
        }

        /// <summary>
        /// Deregisters the onUpdated event.
        /// </summary>
        public void DeregisterOnUpdated()
        {
            onUpdated = null;
        }

        /// <summary>
        /// Registers a UnityEvent to be invoked when the node completes.
        /// </summary>
        /// <param name="onCompleted">The UnityEvent to register.</param>
        public void RegisterOnCompleted(UnityEvent onCompleted)
        {
            this.onCompleted = onCompleted;
        }

        /// <summary>
        /// Deregisters the onCompleted event.
        /// </summary>
        public void DeregisterOnCompleted()
        {
            onCompleted = null;
        }

        /// <summary>
        /// Deregisters all registered events for this node.
        /// </summary>
        public void DeregisterAllEvents()
        {
            DeregisterOnStarted();
            DeregisterOnUpdated();
            DeregisterOnCompleted();
        }
    }

}