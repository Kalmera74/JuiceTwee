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

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.PhysicNodes
{
    [Serializable]
    [NodeMenu("Physic", "Rigid Body 3D")]
    public class RigidBody3DNode : EffectNode, IExtraItemNode
    {
        public override Type TargetType => typeof(Rigidbody);

        [SerializeField] private float _duration = 1f;
        [SerializeField] protected bool _useUnscaledTime = false;


        [SerializeField] private bool _controlMass;
        [SerializeField] private bool _controlDrag;
        [SerializeField] private bool _controlAngularDrag;
        [SerializeField] private bool _controlGravity;
        [SerializeField] private bool _controlIsKinematic;
        [SerializeField] private bool _controlConstraints;
        [SerializeField] private bool _controlLayers;

        [SerializeField] private float _startingMass;
        [SerializeField] private float _endMass;
        [SerializeField] private bool _useCurrentAsStartingMass = true;
        [SerializeField] private bool _useTargetForEndMass = false;
        [SerializeField] private AnimationCurve _massCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [SerializeField] private float _startingDrag;
        [SerializeField] private float _endDrag;
        [SerializeField] private bool _useCurrentAsStartingDrag = true;
        [SerializeField] private bool _useTargetForEndDrag = false;
        [SerializeField] private AnimationCurve _dragCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [SerializeField] private float _startingAngularDrag;
        [SerializeField] private float _endAngularDrag;
        [SerializeField] private bool _useTargetForAngularDrag = false;
        [SerializeField] private bool _useCurrentAsStartingAngularDrag = false;
        [SerializeField] private AnimationCurve _angularDragCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [SerializeField] private bool _useGravity;

        [SerializeField] private bool _isKinematic;

        [SerializeField] private RigidbodyConstraintMode _positionConstraints;
        [SerializeField] private RigidbodyConstraintMode _rotationConstraints;

        [SerializeField] private LayerMask _includeLayers;
        [SerializeField] private LayerMask _excludeLayers;


        private Rigidbody _target => originTarget as Rigidbody;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
        private Rigidbody _valueTarget;

        private List<NodeExtraItemData> _extraItems;



        [Serializable]
        public struct RigidbodyConstraintMode
        {
            public bool X;
            public bool Y;
            public bool Z;
        }




        public List<NodeExtraItemData> GetExtraItems()
        {
            if (!_useTargetForEndDrag && !_useTargetForEndMass && !_useTargetForAngularDrag)
            {
                _extraItems = null;
                return null;
            }

            if (_extraItems != null)
            {
                return _extraItems;
            }

            _extraItems = new()
            {
                new (nameof(_valueTarget), "Target Rigidbody", typeof(Rigidbody)),
            };

            return _extraItems;
        }
        public void SetExtraItem(string key, UnityEngine.Object target)
        {
            if (key.Equals(nameof(_valueTarget)))
            {
                _valueTarget = target as Rigidbody;
                if (_valueTarget == null)
                {
                    Debug.LogWarning($"{NodeName} Target Rigidbody is null. Cannot set properties.");
                    return;
                }
            }
        }

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            if (_target == null)
            {
                Debug.LogWarning($"{NodeName} Target Rigidbody is null. Cannot perform operations.");
                yield break;
            }

            float startTime = _currentTime;
            float elapsedTime = 0;
            InitializeValues();

            onStarted?.Invoke();

            SetInstantProperties();



            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_controlMass)
                {
                    _target.mass = Mathf.Lerp(_startingMass, _endMass, _massCurve.Evaluate(t));
                }

                if (_controlDrag)
                {
                    _target.linearDamping = Mathf.Lerp(_startingDrag, _endDrag, _dragCurve.Evaluate(t));
                }
                if (_controlAngularDrag)
                {
                    _target.angularDamping = Mathf.Lerp(_startingAngularDrag, _endAngularDrag, _angularDragCurve.Evaluate(t));
                }

                yield return null;
                onUpdated?.Invoke();
            }

            if (_controlDrag)
            {
                _target.linearDamping = _endDrag;
            }
            if (_controlAngularDrag)
            {
                _target.angularDamping = _endAngularDrag;
            }
            if (_controlMass)
            {
                _target.mass = _endMass;
            }

            onCompleted?.Invoke();
        }

        private void InitializeValues()
        {

            if (_useCurrentAsStartingMass)
            {
                if (_target == null)
                {
                    Debug.LogError($"{NodeName} Target Rigidbody is null. Cannot initialize values.");
                }
                else
                {

                    _startingMass = _target.mass;
                }
            }
            if (_useCurrentAsStartingDrag)
            {
                if (_target == null)
                {
                    Debug.LogError($"{NodeName} Target Rigidbody is null. Cannot initialize values.");
                }
                else
                {

                    _startingDrag = _target.linearDamping;

                }
            }
            if (_useCurrentAsStartingAngularDrag)
            {
                if (_target == null)
                {
                    Debug.LogError($"{NodeName} Target Rigidbody is null. Cannot initialize values.");
                }
                else
                {

                    _startingAngularDrag = _target.angularDamping;
                }
            }


            if (_useTargetForEndMass)
            {
                if (_valueTarget == null)
                {
                    Debug.LogWarning($"{NodeName} Value Target Rigidbody is null. Cannot set end values.");
                }
                else
                {

                    _endMass = _valueTarget.mass;
                }
            }
            if (_useTargetForEndDrag)
            {
                if (_valueTarget == null)
                {
                    Debug.LogWarning($"{NodeName} Value Target Rigidbody is null. Cannot set end values.");
                }
                else
                {

                    _endDrag = _valueTarget.linearDamping;
                }
            }
            if (_useTargetForAngularDrag)
            {
                if (_valueTarget == null)
                {
                    Debug.LogWarning($"{NodeName} Value Target Rigidbody is null. Cannot set end values.");
                }
                else
                {

                    _endAngularDrag = _valueTarget.angularDamping;
                }
            }

        }

        private void SetInstantProperties()
        {
            if (_controlGravity)
            {
                _target.useGravity = _useGravity;
            }

            if (_controlIsKinematic)
            {
                _target.isKinematic = _isKinematic;
            }

            if (_controlConstraints)
            {
                RigidbodyConstraints constraints = RigidbodyConstraints.None;
                if (_positionConstraints.X)
                {
                    constraints |= RigidbodyConstraints.FreezePositionX;
                }
                if (_positionConstraints.Y)
                {
                    constraints |= RigidbodyConstraints.FreezePositionY;
                }
                if (_positionConstraints.Z)
                {
                    constraints |= RigidbodyConstraints.FreezePositionZ;
                }
                if (_rotationConstraints.X)
                {
                    constraints |= RigidbodyConstraints.FreezeRotationX;
                }
                if (_rotationConstraints.Y)
                {
                    constraints |= RigidbodyConstraints.FreezeRotationY;
                }
                if (_rotationConstraints.Z)
                {
                    constraints |= RigidbodyConstraints.FreezeRotationZ;
                }
                _target.constraints = constraints;
            }
            if (_controlLayers)
            {
                _target.excludeLayers = _excludeLayers;
                _target.includeLayers = _includeLayers;
            }
        }
    }
}