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

using System.Linq;
using JuiceTwee.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace JuiceTwee.CustomNodeEditors
{
    [CustomPropertyDrawer(typeof(SortingLayerPicker))]
    public class SortingLayerPickerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var id = property.FindPropertyRelative("id");

            var layers = SortingLayer.layers.Select(layer => layer.name).ToArray();

            var index = SortingLayer.GetLayerValueFromID(id.intValue) - SortingLayer.GetLayerValueFromID(SortingLayer.layers[0].id);
            index = Mathf.Clamp(index, 0, layers.Length - 1);
            index = EditorGUI.Popup(position, label.text, index, layers);

            id.intValue = SortingLayer.layers[index].id;
        }

    }
}