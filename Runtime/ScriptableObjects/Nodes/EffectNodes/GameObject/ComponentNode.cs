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
using System.Linq;
using JuiceTwee.Runtime.Attributes;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.GameObjectNodes
{

    [Serializable]
    [NodeMenu("GameObject", "Component")]
    public class ComponentNode : EffectNode
    {
        public override Type TargetType => typeof(GameObject);
        [SerializeField, HideInInspector] private string _selectedTypeName;
        public string SelectedTypeName => _selectedTypeName;

        [Header("Component Settings")]
        [SerializeField] private bool _addComponent = true;

        private GameObject _target => originTarget as GameObject;


        public void SetSelectedType(string typeName)
        {
            _selectedTypeName = typeName;
        }

        public override IEnumerator Perform()
        {
            onStarted?.Invoke();

            Type type = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(_selectedTypeName)).FirstOrDefault(type => type != null);

            if (type == null || (!type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsSubclassOf(typeof(Component))))
            {
                Debug.LogError($"{NodeName} Could not resolve type: {_selectedTypeName}");
                yield return base.Perform();
                yield break;
            }

            if (_addComponent)
            {
                try
                {
                    _target.AddComponent(type);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{NodeName} Could not add the component {_selectedTypeName}, {ex.Message}");
                }
            }
            else
            {
                var component = _target.GetComponent(type);
                if (component != null)
                {
                    Destroy(component);
                }
                else
                {
                    Debug.LogError($"{NodeName} No component of type {_selectedTypeName} found on the target to destroy.");
                }
            }

            yield return base.Perform();
        }

    }
}