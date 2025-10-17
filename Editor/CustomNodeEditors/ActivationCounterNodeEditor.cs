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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.FlowNodes;

namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(ActivationCounterNode))]
    public class ActivationCounterNodeEditor : Editor
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
        private SerializedProperty _countToActivate;
        private SerializedProperty _limitActivationToOnce;
        private SerializedProperty _currentCount;
        private SerializedProperty _hasPerformedAction;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _countToActivate = serializedObject.FindProperty(nameof(_countToActivate));
            _limitActivationToOnce = serializedObject.FindProperty(nameof(_limitActivationToOnce));
            _currentCount = serializedObject.FindProperty("_currentCount");
            _hasPerformedAction = serializedObject.FindProperty("_hasPerformedAction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName);
            EditorGUILayout.Separator();

            DrawCounterSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCounterSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Counter Settings");

            EditorGUILayout.PropertyField(_countToActivate, new GUIContent("Amount of time this node should be activated before continuing the flow"));
            EditorGUILayout.PropertyField(_limitActivationToOnce, new GUIContent("If selected the nodes will allow flow only once and be disabled. If not it can flow as long as it is activated"));

            EditorGUILayout.Space();



            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}