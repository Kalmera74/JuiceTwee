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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AnimationNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(AnimatorNode))]
    public class AnimatorNodeEditor : Editor
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

        // Base Settings
        private SerializedProperty _useParametersInstead;

        // Animation/Parameter Name
        private SerializedProperty _name;

        // Parameter Options
        private SerializedProperty _parameterType;
        private SerializedProperty _boolValue;
        private SerializedProperty _floatValue;
        private SerializedProperty _intValue;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _useParametersInstead = serializedObject.FindProperty(nameof(_useParametersInstead));
            _name = serializedObject.FindProperty(nameof(_name));
            _parameterType = serializedObject.FindProperty(nameof(_parameterType));
            _boolValue = serializedObject.FindProperty(nameof(_boolValue));
            _floatValue = serializedObject.FindProperty(nameof(_floatValue));
            _intValue = serializedObject.FindProperty(nameof(_intValue));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawBaseSettings();
            DrawNameField();
            DrawConditionalFields();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_useParametersInstead, new GUIContent("Use Parameters Instead", "If checked, this node will control an Animator parameter. Otherwise, it will play an animation."));
            EditorGUILayout.EndVertical();
        }

        private void DrawNameField()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Name Settings");
            string nameLabel = _useParametersInstead.boolValue ? "Parameter Name" : "Animation Name";
            string tooltip = _useParametersInstead.boolValue ? "The name of the Animator parameter to control." : "The name of the animation state to play.";
            EditorGUILayout.PropertyField(_name, new GUIContent(nameLabel, tooltip));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalFields()
        {
            if (_useParametersInstead.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Parameter Options");
                EditorGUILayout.PropertyField(_parameterType, new GUIContent("Parameter Type", "The type of the Animator parameter."));
                int parameterType = _parameterType.enumValueIndex;
                switch (parameterType)
                {
                    case 0:
                        EditorGUILayout.PropertyField(_floatValue, new GUIContent("Value", "The float value to set the parameter to."));
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(_intValue, new GUIContent("Value", "The integer value to set the parameter to."));
                        break;
                    case 2:
                        EditorGUILayout.PropertyField(_boolValue, new GUIContent("Value", "The boolean value to set the parameter to."));
                        break;
                    default:
                        break;
                }
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