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
using JuiceTwee.Runtime.Attributes;
using UnityEngine;


namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.PhysicNodes
{
    [Serializable]
    [NodeMenu("Physic", "Collider")]
    public class ColliderNode : EffectNode
    {
        public override Type TargetType => typeof(Collider);

        [SerializeField] private bool _controlEnabled = false;
        [SerializeField] private bool _controlTrigger;
        [SerializeField] private bool _controlMaterial;
        [SerializeField] private bool _controlLayer;


        [SerializeField] private bool _isEnabled = true;


        [SerializeField] private bool _isTrigger = false;


        [SerializeField] private PhysicsMaterial _material;


        [SerializeField] private int _layerOverridePriority = 0;
        [SerializeField] private LayerMask _includeLayerMask;
        [SerializeField] private LayerMask _excludeLayerMask;

        private Collider _target => originTarget as Collider;

        public override void OnTargetChangedDuringEditorPlay()
        {
            _layerOverridePriority = _target.layerOverridePriority;
            _includeLayerMask = _target.includeLayers;
            _excludeLayerMask = _target.excludeLayers;
            _isTrigger = _target.isTrigger;
        }
        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            onStarted?.Invoke();

            if (_controlEnabled)
            {
                _target.enabled = _isEnabled;
            }

            if (_controlMaterial)
            {
                _target.material = _material;
            }

            if (_controlLayer)
            {
                _target.layerOverridePriority = _layerOverridePriority;
                _target.includeLayers = _includeLayerMask;
                _target.excludeLayers = _excludeLayerMask;
            }
            if (_controlTrigger)
            {
                _target.isTrigger = _isTrigger;
            }

            yield return null;

            onUpdated?.Invoke();
            onCompleted?.Invoke();

        }
    }
}