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

    [CustomEditor(typeof(LimitCounterNode))]
    public class LimitCounterNodeEditor : Editor
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
        private SerializedProperty _maxActivationCount;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _maxActivationCount = serializedObject.FindProperty(nameof(_maxActivationCount));
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

            EditorGUILayout.PropertyField(_maxActivationCount, new GUIContent("Number of times this node allowed to be activated before being disabled"));

            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}