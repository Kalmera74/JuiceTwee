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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AudioNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(AudioMixerNode))]
    public class AudioMixerNodeEditor : Editor
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
        private SerializedProperty _useUnscaledTime;
        private SerializedProperty _useParametersInstead;
        private SerializedProperty _selectRandomSnapshot;
        private SerializedProperty _transitionAllSnapshots;
        private SerializedProperty _parameters;
        private SerializedProperty _snapShots;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _useParametersInstead = serializedObject.FindProperty(nameof(_useParametersInstead));
            _selectRandomSnapshot = serializedObject.FindProperty(nameof(_selectRandomSnapshot));
            _transitionAllSnapshots = serializedObject.FindProperty(nameof(_transitionAllSnapshots));
            _parameters = serializedObject.FindProperty(nameof(_parameters));
            _snapShots = serializedObject.FindProperty(nameof(_snapShots));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawConditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds to transition between states."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_useParametersInstead, new GUIContent("Use Parameters Instead", "If checked, this node will transition individual parameters. Otherwise, it will transition between snapshots."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_useParametersInstead.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Parameters Settings");
                EditorGUILayout.PropertyField(_parameters, new GUIContent("Parameters", "The list of Audio Mixer parameters to transition."));
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Snapshot Settings");
                EditorGUILayout.PropertyField(_transitionAllSnapshots, new GUIContent("Transition All Snapshots", "If checked, the node will transition between all snapshots in the list."));
                if (!_transitionAllSnapshots.boolValue)
                {
                    EditorGUILayout.PropertyField(_selectRandomSnapshot, new GUIContent("Select Random Snapshot", "If checked, a random snapshot will be selected from the list."));
                }
                EditorGUILayout.PropertyField(_snapShots, new GUIContent("Snapshots", "The list of Audio Mixer Snapshots to transition to."));
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