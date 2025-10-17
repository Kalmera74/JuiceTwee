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

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes;

namespace JuiceTwee.CustomNodeEditors
{
    [CustomEditor(typeof(SpriteRendererNode))]
    public class SpriteRendererNodeEditor : Editor
    {
        private GUIStyle _headerStyle;
        private GUIStyle HeaderStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                return _headerStyle;
            }
        }

        #region Serialized Properties
        // Time Options
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;

        // Base Settings
        private SerializedProperty _controlColor;
        private SerializedProperty _controlFlip;
        private SerializedProperty _controlMask;
        private SerializedProperty _controlMaterial;
        private SerializedProperty _controlSorting;

        // Color Settings
        private SerializedProperty _useCurrentColorAsStart;
        private SerializedProperty _startColor;
        private SerializedProperty _endColor;

        // Flip Settings
        private SerializedProperty _flipX;
        private SerializedProperty _flipY;

        // Mask Settings
        private SerializedProperty _maskType;

        // Material Settings
        private SerializedProperty _selectRandomMaterial;
        private SerializedProperty _materials;

        // Sorting Settings
        private SerializedProperty _sortingLayer;
        private SerializedProperty _tweenSortingOrder;
        private SerializedProperty _sortingOrder;
        private SerializedProperty _spriteSortPoint;
        #endregion

        private void OnEnable()
        {
            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _controlColor = serializedObject.FindProperty(nameof(_controlColor));
            _controlFlip = serializedObject.FindProperty(nameof(_controlFlip));
            _controlMask = serializedObject.FindProperty(nameof(_controlMask));
            _controlMaterial = serializedObject.FindProperty(nameof(_controlMaterial));
            _controlSorting = serializedObject.FindProperty(nameof(_controlSorting));

            // Color Settings
            _useCurrentColorAsStart = serializedObject.FindProperty(nameof(_useCurrentColorAsStart));
            _startColor = serializedObject.FindProperty(nameof(_startColor));
            _endColor = serializedObject.FindProperty(nameof(_endColor));

            // Flip Settings
            _flipX = serializedObject.FindProperty(nameof(_flipX));
            _flipY = serializedObject.FindProperty(nameof(_flipY));

            // Mask Settings
            _maskType = serializedObject.FindProperty(nameof(_maskType));

            // Material Settings
            _selectRandomMaterial = serializedObject.FindProperty(nameof(_selectRandomMaterial));
            _materials = serializedObject.FindProperty(nameof(_materials));

            // Sorting Settings
            _sortingLayer = serializedObject.FindProperty(nameof(_sortingLayer));
            _tweenSortingOrder = serializedObject.FindProperty(nameof(_tweenSortingOrder));
            _sortingOrder = serializedObject.FindProperty(nameof(_sortingOrder));
            _spriteSortPoint = serializedObject.FindProperty(nameof(_spriteSortPoint));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTimeOptions();
            DrawBaseSettings();

            if (_controlColor.boolValue)
            {
                DrawColorSettings();
            }
            if (_controlFlip.boolValue)
            {
                DrawFlipSettings();
            }
            if (_controlMask.boolValue)
            {
                DrawMaskSettings();
            }
            if (_controlMaterial.boolValue)
            {
                DrawMaterialSettings();
            }
            if (_controlSorting.boolValue)
            {
                DrawSortingSettings();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds for the color tween to complete."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlColor, new GUIContent("Control Color", "If checked, this node will tween the sprite's color."));
            EditorGUILayout.PropertyField(_controlFlip, new GUIContent("Control Flip", "If checked, this node will set the sprite's flip state."));
            EditorGUILayout.PropertyField(_controlMask, new GUIContent("Control Mask", "If checked, this node will set the sprite's mask interaction."));
            EditorGUILayout.PropertyField(_controlMaterial, new GUIContent("Control Material", "If checked, this node will change the sprite's material."));
            EditorGUILayout.PropertyField(_controlSorting, new GUIContent("Control Sorting", "If checked, this node will change the sprite's sorting layer and order."));
            EditorGUILayout.EndVertical();
        }

        private void DrawColorSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Color Settings");
            EditorGUILayout.PropertyField(_useCurrentColorAsStart, new GUIContent("Use Current Color as Start", "If checked, the color tween will start from the sprite's current color."));
            if (!_useCurrentColorAsStart.boolValue)
            {
                EditorGUILayout.PropertyField(_startColor, new GUIContent("Start Color", "The starting color for the tween."));
            }
            EditorGUILayout.PropertyField(_endColor, new GUIContent("End Color", "The final color for the tween."));
            EditorGUILayout.EndVertical();
        }

        private void DrawFlipSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Flip Settings");
            EditorGUILayout.PropertyField(_flipX, new GUIContent("Flip X", "The new state of the sprite's X-axis flip."));
            EditorGUILayout.PropertyField(_flipY, new GUIContent("Flip Y", "The new state of the sprite's Y-axis flip."));
            EditorGUILayout.EndVertical();
        }

        private void DrawMaskSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Mask Settings");
            EditorGUILayout.PropertyField(_maskType, new GUIContent("Mask Type", "The new mask interaction type for the sprite."));
            EditorGUILayout.EndVertical();
        }

        private void DrawMaterialSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Material Settings");
            EditorGUILayout.PropertyField(_selectRandomMaterial, new GUIContent("Select Random Material", "If checked, a random material from the list will be chosen. Otherwise, the first one will be used."));
            EditorGUILayout.PropertyField(_materials, new GUIContent("Materials", "A list of materials to choose from."), true);
            EditorGUILayout.EndVertical();
        }

        private void DrawSortingSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Sorting Settings");
            EditorGUILayout.PropertyField(_sortingLayer, new GUIContent("Sorting Layer", "The new sorting layer for the sprite."));
            EditorGUILayout.PropertyField(_spriteSortPoint, new GUIContent("Sort Point", "The new sorting point for the sprite."));
            EditorGUILayout.PropertyField(_tweenSortingOrder, new GUIContent("Tween Sorting Order", "If checked, the sorting order will be tweened instead of directly setting"));
            EditorGUILayout.PropertyField(_sortingOrder, new GUIContent("Sorting Order", "The new sorting order for the sprite."));
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}