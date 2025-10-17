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
using JuiceTwee.Runtime.Attributes;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.GameObjectNodes
{

    [Serializable]
    [NodeMenu("GameObject", "Instantiation")]

    public class InstantiationNode : EffectNode, IExtraItemNode
    {
        public override Type TargetType => typeof(GameObject);


        [SerializeField] private bool _useTarget = false;
        [SerializeField] private bool _setParent = false;
        [SerializeField] private bool _instantiateActivated = true;
        [SerializeField] private bool _overridePosition = false;
        [SerializeField] private bool _overrideRotation = false;
        [SerializeField] private bool _overrideScale = false;


        [SerializeField] private bool _relativeToParent = false;
        [SerializeField] private Vector3 _instantiationPosition;

        [SerializeField] private Vector3 _instantiationRotation;

        [SerializeField] private Vector3 _instantiationScale;


        private GameObject _target => originTarget as GameObject;
        private Transform _instantiationTarget;
        private Transform _parent;
        private List<NodeExtraItemData> _extraItems;

        private struct InstantiationParameters
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }

        public List<NodeExtraItemData> GetExtraItems()
        {
            if (!_useTarget && !_setParent)
            {
                _extraItems = null;
                return null;
            }

            if (_extraItems != null)
            {
                return _extraItems;
            }

            _extraItems = new();

            if (_useTarget)
            {
                _extraItems.Add(new(nameof(_instantiationTarget), "Instantiation Value Target", typeof(Transform)));
            }
            if (_setParent)
            {
                _extraItems.Add(new(nameof(_parent), "Instantiation Parent", typeof(Transform)));
            }

            return _extraItems;
        }

        public void SetExtraItem(string key, UnityEngine.Object target)
        {
            if (key.Equals(nameof(_instantiationTarget)))
            {
                _instantiationTarget = target as Transform;
            }
            if (key.Equals(nameof(_parent)))
            {
                _parent = target as Transform;
            }
        }

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            var instantiationParameters = new InstantiationParameters();

            if (_useTarget && _instantiationTarget == null)
            {
                throw new ArgumentNullException($"{NodeName} No target is set to reference for Instantiation");
            }

            if (_overridePosition)
            {
                instantiationParameters.Position = _instantiationPosition;
                if (_useTarget)
                {
                    instantiationParameters.Position = _instantiationTarget.position;
                }
            }
            if (_overrideRotation)
            {
                instantiationParameters.Rotation = _instantiationRotation;
                if (_useTarget)
                {
                    instantiationParameters.Rotation = _instantiationTarget.rotation.eulerAngles;
                }
            }


            onStarted?.Invoke();

            GameObject instantiatedObject;
            if (_relativeToParent)
            {
                if (_parent == null)
                {
                    throw new ArgumentNullException($"{NodeName} No Parent set to be used for relative settings");
                }
                instantiatedObject = Instantiate(_target, _parent, false);
            }
            else
            {
                instantiatedObject = Instantiate(_target, instantiationParameters.Position, Quaternion.Euler(instantiationParameters.Rotation), _parent);
            }

            if (_overrideScale)
            {
                instantiatedObject.transform.localScale = _instantiationScale;
                if (_useTarget)
                {
                    instantiatedObject.transform.localScale = _instantiationTarget.localScale;
                }
            }

            instantiatedObject.SetActive(_instantiateActivated);

            onUpdated?.Invoke();
            onCompleted?.Invoke();
            yield return null;
        }
    }
}