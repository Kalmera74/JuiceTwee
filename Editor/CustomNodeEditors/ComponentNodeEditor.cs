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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.GameObjectNodes;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(ComponentNode))]
    public class ComponentNodeEditor : Editor
    {
        private ComponentNode _node;
        private AdvancedDropdownState _dropdownState;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }
            _node = (ComponentNode)target;
            _dropdownState ??= new AdvancedDropdownState();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button(string.IsNullOrEmpty(_node.SelectedTypeName)
                ? "Select MonoBehaviour"
                : _node.SelectedTypeName))
            {
                var dropdown = new ComponentNodeDropdown(_dropdownState, OnTypeSelected);
                var rect = GUILayoutUtility.GetLastRect();
                dropdown.Show(new Rect(rect.x, rect.yMax, 0, 0));
            }
        }

        private void OnTypeSelected(Type selectedType)
        {
            _node.SetSelectedType(selectedType.FullName);
            EditorUtility.SetDirty(_node);
        }
    }
}