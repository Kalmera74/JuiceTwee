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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TimeNodes;

namespace JuiceTwee.CustomNodeEditors
{
    [CustomEditor(typeof(TimeScaleNode))]
    public class TimeScaleNodeEditor : Editor
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
        private SerializedProperty _duration;
        private SerializedProperty _ignoreDuration;
        private SerializedProperty _timeScale;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _ignoreDuration = serializedObject.FindProperty(nameof(_ignoreDuration));
            _timeScale = serializedObject.FindProperty(nameof(_timeScale));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_timeScale, new GUIContent("Time Scale", "The new value for Time.timeScale. 1.0 is normal speed, 0.5 is half speed, and 2.0 is double speed."));
            EditorGUILayout.PropertyField(_ignoreDuration, new GUIContent("Ignore Duration", "If checked, the Time.timeScale will be set instantly without a tween."));
            if (!_ignoreDuration.boolValue)
            {
                EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds over which the Time.timeScale will tween to the new value."));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}