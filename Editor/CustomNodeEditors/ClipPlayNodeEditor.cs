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

    [CustomEditor(typeof(ClipPlayNode))]
    public class ClipPlayNodeEditor : Editor
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

        // Time Options
        private SerializedProperty _useCustomDuration;
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;

        // Base Settings
        private SerializedProperty _controlVolume;
        private SerializedProperty _controlPitch;
        private SerializedProperty _useMixerGroup;
        private SerializedProperty _autoDestroyAudioSource;

        // Mixer Settings
        private SerializedProperty _mixerGroup;

        // Volume Settings
        private SerializedProperty _randomizeVolume;
        private SerializedProperty _startVolume;
        private SerializedProperty _endVolume;
        private SerializedProperty _volumeCurve;

        // Pitch Settings
        private SerializedProperty _randomizePitch;
        private SerializedProperty _startPitch;
        private SerializedProperty _endPitch;
        private SerializedProperty _pitchCurve;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));

            // Time Options
            _useCustomDuration = serializedObject.FindProperty(nameof(_useCustomDuration));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _controlVolume = serializedObject.FindProperty(nameof(_controlVolume));
            _controlPitch = serializedObject.FindProperty(nameof(_controlPitch));
            _useMixerGroup = serializedObject.FindProperty(nameof(_useMixerGroup));
            _autoDestroyAudioSource = serializedObject.FindProperty(nameof(_autoDestroyAudioSource));

            // Mixer Settings
            _mixerGroup = serializedObject.FindProperty(nameof(_mixerGroup));

            // Volume Settings
            _randomizeVolume = serializedObject.FindProperty(nameof(_randomizeVolume));
            _startVolume = serializedObject.FindProperty(nameof(_startVolume));
            _endVolume = serializedObject.FindProperty(nameof(_endVolume));
            _volumeCurve = serializedObject.FindProperty(nameof(_volumeCurve));

            // Pitch Settings
            _randomizePitch = serializedObject.FindProperty(nameof(_randomizePitch));
            _startPitch = serializedObject.FindProperty(nameof(_startPitch));
            _endPitch = serializedObject.FindProperty(nameof(_endPitch));
            _pitchCurve = serializedObject.FindProperty(nameof(_pitchCurve));
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
            EditorGUILayout.PropertyField(_useCustomDuration, new GUIContent("Use Custom Duration", "If checked, the audio clip will play for a custom duration instead of its full length."));
            if (_useCustomDuration.boolValue)
            {
                EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The custom duration in seconds to play the audio clip."));
            }
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlVolume, new GUIContent("Control Volume", "If checked, this node will control the volume of the audio clip."));
            EditorGUILayout.PropertyField(_controlPitch, new GUIContent("Control Pitch", "If checked, this node will control the pitch of the audio clip."));
            EditorGUILayout.PropertyField(_useMixerGroup, new GUIContent("Use Mixer Group", "If checked, the audio clip will be routed to a specific audio mixer group."));
            EditorGUILayout.PropertyField(_autoDestroyAudioSource, new GUIContent("Auto Destroy AudioSource", "If checked, the AudioSource component will be automatically destroyed after the clip finishes playing."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_useMixerGroup.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Mixer Settings");
                EditorGUILayout.PropertyField(_mixerGroup, new GUIContent("Mixer Group", "The AudioMixerGroup to which the audio will be routed."));
                EditorGUILayout.EndVertical();
            }

            if (_controlVolume.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Volume Settings");
                EditorGUILayout.PropertyField(_randomizeVolume, new GUIContent("Randomize Volume", "If checked, a random volume will be selected between the min and max values."));
                if (_randomizeVolume.boolValue)
                {
                    EditorGUILayout.PropertyField(_startVolume, new GUIContent("Min Volume", "The minimum random volume value."));
                    EditorGUILayout.PropertyField(_endVolume, new GUIContent("Max Volume", "The maximum random volume value."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_startVolume, new GUIContent("Start Volume", "The starting volume for the tween."));
                    EditorGUILayout.PropertyField(_endVolume, new GUIContent("End Volume", "The ending volume for the tween."));
                    EditorGUILayout.PropertyField(_volumeCurve, new GUIContent("Volume Curve", "The curve to apply to the volume tween."));
                }
                EditorGUILayout.EndVertical();
            }

            if (_controlPitch.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Pitch Settings");
                EditorGUILayout.PropertyField(_randomizePitch, new GUIContent("Randomize Pitch", "If checked, a random pitch will be selected between the min and max values."));
                if (_randomizePitch.boolValue)
                {
                    EditorGUILayout.PropertyField(_startPitch, new GUIContent("Min Pitch", "The minimum random pitch value."));
                    EditorGUILayout.PropertyField(_endPitch, new GUIContent("Max Pitch", "The maximum random pitch value."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_startPitch, new GUIContent("Start Pitch", "The starting pitch for the tween."));
                    EditorGUILayout.PropertyField(_endPitch, new GUIContent("End Pitch", "The ending pitch for the tween."));
                    EditorGUILayout.PropertyField(_pitchCurve, new GUIContent("Pitch Curve", "The curve to apply to the pitch tween."));
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