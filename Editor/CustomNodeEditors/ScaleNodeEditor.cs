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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.TransformNodes;

namespace JuiceTwee.CustomNodeEditors
{
    [CustomEditor(typeof(ScaleNode))]
    public class ScaleNodeEditor : Editor
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
        // Time Options
        private SerializedProperty _useUnscaledTime;
        private SerializedProperty _duration;
        private SerializedProperty _useSpeedInsteadOfDuration;

        // Base Settings
        private SerializedProperty _useRelativeScale;
        private SerializedProperty _snapToScale;

        // Scale Settings
        private SerializedProperty _scaleSpeed;
        private SerializedProperty _useSeparateAxisCurves;
        private SerializedProperty _startScale;
        private SerializedProperty _endScale;
        private SerializedProperty _scaleCurve;
        private SerializedProperty _xScaleCurve;
        private SerializedProperty _yScaleCurve;
        private SerializedProperty _zScaleCurve;
        #endregion

        void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            // Cache all serialized properties
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useSpeedInsteadOfDuration = serializedObject.FindProperty(nameof(_useSpeedInsteadOfDuration));

            _useRelativeScale = serializedObject.FindProperty(nameof(_useRelativeScale));
            _snapToScale = serializedObject.FindProperty(nameof(_snapToScale));

            _scaleSpeed = serializedObject.FindProperty(nameof(_scaleSpeed));
            _useSeparateAxisCurves = serializedObject.FindProperty(nameof(_useSeparateAxisCurves));
            _startScale = serializedObject.FindProperty(nameof(_startScale));
            _endScale = serializedObject.FindProperty(nameof(_endScale));
            _scaleCurve = serializedObject.FindProperty(nameof(_scaleCurve));

            _xScaleCurve = serializedObject.FindProperty(nameof(_xScaleCurve));
            _yScaleCurve = serializedObject.FindProperty(nameof(_yScaleCurve));
            _zScaleCurve = serializedObject.FindProperty(nameof(_zScaleCurve));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawScaleSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the tween will be independent of Time.timeScale."));
            EditorGUILayout.PropertyField(_useSpeedInsteadOfDuration, new GUIContent("Use Speed Instead of Duration", "If checked, the tween will complete based on a speed value rather than a fixed duration."));

            if (_useSpeedInsteadOfDuration.boolValue)
            {
                EditorGUILayout.PropertyField(_scaleSpeed, new GUIContent("Scale Speed", "The speed at which the scale changes per second."));
            }
            else
            {
                EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds for the tween to complete."));
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_useRelativeScale, new GUIContent("Use Relative Scale", "If checked, the start and end scales will be treated as relative values to the current scale."));
            EditorGUILayout.PropertyField(_snapToScale, new GUIContent("Snap to Scale", "If checked, the scale will snap to the final value without any tweening."));
            EditorGUILayout.EndVertical();
        }

        private void DrawScaleSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Scale Settings");
            EditorGUILayout.PropertyField(_startScale, new GUIContent("Start Scale", "The starting local scale for the tween."));
            EditorGUILayout.PropertyField(_endScale, new GUIContent("End Scale", "The final local scale for the tween."));

            EditorGUILayout.PropertyField(_useSeparateAxisCurves, new GUIContent("Use Separate Axis Curves", "If checked, you can define a different animation curve for each axis (X, Y, Z)."));
            if (_useSeparateAxisCurves.boolValue)
            {
                EditorGUILayout.PropertyField(_xScaleCurve, new GUIContent("X Scale Curve", "The curve to apply to the X-axis scale tween."));
                EditorGUILayout.PropertyField(_yScaleCurve, new GUIContent("Y Scale Curve", "The curve to apply to the Y-axis scale tween."));
                EditorGUILayout.PropertyField(_zScaleCurve, new GUIContent("Z Scale Curve", "The curve to apply to the Z-axis scale tween."));
            }
            else
            {
                EditorGUILayout.PropertyField(_scaleCurve, new GUIContent("Scale Curve", "The overall curve to apply to the tween for all axes."));
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