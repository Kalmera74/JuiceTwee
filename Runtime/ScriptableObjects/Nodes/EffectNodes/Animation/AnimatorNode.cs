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

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AnimationNodes
{

    [Serializable]
    [NodeMenu("Animation", "Animator")]
    public class AnimatorNode : EffectNode
    {
        public override Type TargetType => typeof(Animator);


        [SerializeField] private bool _useParametersInstead = false;

        [SerializeField] private string _name;

        [SerializeField] private AnimationTriggerType _parameterType;
        [SerializeField] private bool _boolValue;
        [SerializeField] private float _floatValue;
        [SerializeField] private int _intValue;

        private Animator _target => originTarget as Animator;

        public override IEnumerator Perform()
        {
            if (_target == null)
            {
                Debug.LogError("AnimationNode: Target is null or not an Animator.", _player);
                yield break;
            }


            onStarted?.Invoke();

            if (_useParametersInstead)
            {
                switch (_parameterType)
                {
                    case AnimationTriggerType.Float:
                        _target.SetFloat(_name, _floatValue);
                        break;
                    case AnimationTriggerType.Int:
                        _target.SetInteger(_name, _intValue);
                        break;
                    case AnimationTriggerType.Bool:
                        _target.SetBool(_name, _boolValue);
                        break;
                    case AnimationTriggerType.Trigger:
                        _target.SetTrigger(_name);
                        break;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    _target.Play(_name);
                }
            }

            onCompleted?.Invoke();

            yield return base.Perform();
        }
        public enum AnimationTriggerType
        {
            Float,
            Int,
            Bool,
            Trigger
        }
    }
}