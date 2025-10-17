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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.PhysicNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(ColliderNode))]
    public class ColliderNodeEditor : Editor
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
        // Node Name
        private SerializedProperty _nodeName;

        // Tween Options
        private SerializedProperty _controlEnabled;
        private SerializedProperty _controlTrigger;
        private SerializedProperty _controlMaterial;
        private SerializedProperty _controlLayer;

        // Enable Settings
        private SerializedProperty _isEnabled;

        // Collider Settings
        private SerializedProperty _isTrigger;

        // Material Settings
        private SerializedProperty _material;

        // Layer Settings
        private SerializedProperty _layerOverridePriority;
        private SerializedProperty _includeLayerMask;
        private SerializedProperty _excludeLayerMask;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));

            // Tween Options
            _controlEnabled = serializedObject.FindProperty(nameof(_controlEnabled));
            _controlTrigger = serializedObject.FindProperty(nameof(_controlTrigger));
            _controlMaterial = serializedObject.FindProperty(nameof(_controlMaterial));
            _controlLayer = serializedObject.FindProperty(nameof(_controlLayer));

            // Enable Settings
            _isEnabled = serializedObject.FindProperty(nameof(_isEnabled));

            // Collider Settings
            _isTrigger = serializedObject.FindProperty(nameof(_isTrigger));

            // Material Settings
            _material = serializedObject.FindProperty(nameof(_material));

            // Layer Settings
            _layerOverridePriority = serializedObject.FindProperty(nameof(_layerOverridePriority));
            _includeLayerMask = serializedObject.FindProperty(nameof(_includeLayerMask));
            _excludeLayerMask = serializedObject.FindProperty(nameof(_excludeLayerMask));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName);
            EditorGUILayout.Separator();

            DrawTweenOptions();
            DrawConditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawTweenOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlEnabled);
            EditorGUILayout.PropertyField(_controlTrigger);
            EditorGUILayout.PropertyField(_controlMaterial);
            EditorGUILayout.PropertyField(_controlLayer);
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_controlEnabled.boolValue || _controlTrigger.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Boolean State Settings");
                if (_controlEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(_isEnabled);
                }
                if (_controlTrigger.boolValue)
                {
                    EditorGUILayout.PropertyField(_isTrigger);
                }
                EditorGUILayout.EndVertical();

            }



            if (_controlMaterial.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Material Settings");
                EditorGUILayout.PropertyField(_material);
                EditorGUILayout.EndVertical();
            }

            if (_controlLayer.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Layer Settings");
                EditorGUILayout.PropertyField(_layerOverridePriority);
                EditorGUILayout.PropertyField(_includeLayerMask);
                EditorGUILayout.PropertyField(_excludeLayerMask);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}