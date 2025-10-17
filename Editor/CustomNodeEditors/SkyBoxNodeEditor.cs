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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes;

namespace JuiceTwee.CustomNodeEditors
{
    [CustomEditor(typeof(SkyBoxNode))]
    public class SkyBoxNodeEditor : Editor
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
        private SerializedProperty _updateGIAutomatically;
        private SerializedProperty _controlMaterial;
        private SerializedProperty _controlShadows;

        // Material Settings
        private SerializedProperty _selectRandomSkyBox;
        private SerializedProperty _skyBoxMaterials;

        // Shadow Settings
        private SerializedProperty _useCurrentAsStart;
        private SerializedProperty _shadowStartColor;
        private SerializedProperty _shadowEndColor;
        #endregion

        private void OnEnable()
        {
            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _updateGIAutomatically = serializedObject.FindProperty(nameof(_updateGIAutomatically));
            _controlMaterial = serializedObject.FindProperty(nameof(_controlMaterial));
            _controlShadows = serializedObject.FindProperty(nameof(_controlShadows));

            // Material Settings
            _selectRandomSkyBox = serializedObject.FindProperty(nameof(_selectRandomSkyBox));
            _skyBoxMaterials = serializedObject.FindProperty(nameof(_skyBoxMaterials));

            // Shadow Settings
            _useCurrentAsStart = serializedObject.FindProperty(nameof(_useCurrentAsStart));
            _shadowStartColor = serializedObject.FindProperty(nameof(_shadowStartColor));
            _shadowEndColor = serializedObject.FindProperty(nameof(_shadowEndColor));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTimeOptions();
            DrawBaseSettings();

            if (_controlMaterial.boolValue)
            {
                DrawMaterialSettings();
            }

            if (_controlShadows.boolValue)
            {
                DrawShadowSettings();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds over which the transition will occur."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_updateGIAutomatically, new GUIContent("Update GI Automatically", "If checked, a new GI lighting update will be triggered after the material change."));
            EditorGUILayout.PropertyField(_controlMaterial, new GUIContent("Control Material", "If checked, this node will change the scene's skybox material."));
            EditorGUILayout.PropertyField(_controlShadows, new GUIContent("Control Shadows", "If checked, this node will tween the shadow color."));
            EditorGUILayout.EndVertical();
        }

        private void DrawMaterialSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Material Settings");
            EditorGUILayout.PropertyField(_selectRandomSkyBox, new GUIContent("Select Random Skybox", "If checked, a random skybox material from the list will be chosen. Otherwise, the first one will be used."));
            EditorGUILayout.PropertyField(_skyBoxMaterials, new GUIContent("Skybox Materials", "A list of skybox materials to choose from."), true);
            EditorGUILayout.EndVertical();
        }

        private void DrawShadowSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Shadow Settings");
            EditorGUILayout.PropertyField(_useCurrentAsStart, new GUIContent("Use Current As Start", "If checked, the shadow color tween will begin from the current ambient shadow color."));
            if (!_useCurrentAsStart.boolValue)
            {
                EditorGUILayout.PropertyField(_shadowStartColor, new GUIContent("Start Color", "The starting color for the shadow tween."));
            }
            EditorGUILayout.PropertyField(_shadowEndColor, new GUIContent("End Color", "The final color for the shadow tween."));
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}