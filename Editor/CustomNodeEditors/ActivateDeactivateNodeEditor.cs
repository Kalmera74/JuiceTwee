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

    [CustomEditor(typeof(ActivateDeactivateNode))]
    public class ActivateDeactivateNodeEditor : Editor
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
        private SerializedProperty _nodeName;
        private SerializedProperty _targetMonobehaviourInstead;
        private SerializedProperty _activate;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _targetMonobehaviourInstead = serializedObject.FindProperty(nameof(_targetMonobehaviourInstead));
            _activate = serializedObject.FindProperty(nameof(_activate));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName);
            EditorGUILayout.Separator();

            DrawActivationSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawActivationSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Activation Settings");

            EditorGUILayout.PropertyField(_targetMonobehaviourInstead);

            GUIContent activateLabel = new GUIContent();
            if (_targetMonobehaviourInstead.boolValue)
            {
                activateLabel.text = "Enable Component";
                activateLabel.tooltip = "If checked, enables the component. If unchecked, disables it.";
            }
            else
            {
                activateLabel.text = "Activate GameObject";
                activateLabel.tooltip = "If checked, activates the GameObject. If unchecked, deactivates it.";
            }

            EditorGUILayout.PropertyField(_activate, activateLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}