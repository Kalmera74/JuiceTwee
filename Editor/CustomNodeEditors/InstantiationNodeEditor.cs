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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.GameObjectNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(InstantiationNode))]
    public class InstantiationNodeEditor : Editor
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
        // Node Settings
        private SerializedProperty _nodeName;

        // Instantiation Settings
        private SerializedProperty _useTarget;
        private SerializedProperty _setParent;
        private SerializedProperty _instantiateActivated;
        private SerializedProperty _overridePosition;
        private SerializedProperty _overrideRotation;
        private SerializedProperty _overrideScale;

        // Position Settings
        private SerializedProperty _relativeToParent;
        private SerializedProperty _instantiationPosition;

        // Rotation Settings
        private SerializedProperty _instantiationRotation;

        // Scale Settings
        private SerializedProperty _instantiationScale;

        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty("_nodeName");

            // Instantiation Settings
            _useTarget = serializedObject.FindProperty(nameof(_useTarget));
            _setParent = serializedObject.FindProperty(nameof(_setParent));
            _instantiateActivated = serializedObject.FindProperty(nameof(_instantiateActivated));
            _overridePosition = serializedObject.FindProperty(nameof(_overridePosition));
            _overrideRotation = serializedObject.FindProperty(nameof(_overrideRotation));
            _overrideScale = serializedObject.FindProperty(nameof(_overrideScale));

            // Position Settings
            _relativeToParent = serializedObject.FindProperty(nameof(_relativeToParent));
            _instantiationPosition = serializedObject.FindProperty(nameof(_instantiationPosition));

            // Rotation Settings
            _instantiationRotation = serializedObject.FindProperty(nameof(_instantiationRotation));

            // Scale Settings
            _instantiationScale = serializedObject.FindProperty(nameof(_instantiationScale));

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawInstantiationSettings();
            DrawConditionalFields();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInstantiationSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_useTarget, new GUIContent("Use Target Transform", "If checked, the instantiated object's position, rotation, and scale will be set to the target's transform."));
            EditorGUILayout.PropertyField(_setParent, new GUIContent("Set Parent", "If checked, the instantiated object will be a child of the target transform."));
            EditorGUILayout.PropertyField(_instantiateActivated, new GUIContent("Instantiate Activated", "If checked, the instantiated object will be active upon creation."));
            EditorGUILayout.PropertyField(_overridePosition, new GUIContent("Override Position", "If checked, the 'Instantiation Position' will override the object's position."));
            EditorGUILayout.PropertyField(_overrideRotation, new GUIContent("Override Rotation", "If checked, the 'Instantiation Rotation' will override the object's rotation."));
            EditorGUILayout.PropertyField(_overrideScale, new GUIContent("Override Scale", "If checked, the 'Instantiation Scale' will override the object's scale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalFields()
        {
            if (_overridePosition.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Position Settings");
                EditorGUILayout.PropertyField(_relativeToParent, new GUIContent("Relative to Parent", "If checked, the 'Instantiation Position' will be relative to the parent's local space."));
                EditorGUILayout.PropertyField(_instantiationPosition, new GUIContent("Instantiation Position", "The position where the object will be instantiated."));
                EditorGUILayout.EndVertical();
            }

            if (_overrideRotation.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Rotation Settings");
                EditorGUILayout.PropertyField(_instantiationRotation, new GUIContent("Instantiation Rotation", "The rotation of the instantiated object."));
                EditorGUILayout.EndVertical();
            }

            if (_overrideScale.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Scale Settings");
                EditorGUILayout.PropertyField(_instantiationScale, new GUIContent("Instantiation Scale", "The scale of the instantiated object."));
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