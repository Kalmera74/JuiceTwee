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
    [CustomEditor(typeof(RotationNode))]
    public class RotationNodeEditor : Editor
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

        // Options
        private SerializedProperty _useRelativeRotation;
        private SerializedProperty _useLocalRotation;
        private SerializedProperty _snapToRotation;
        private SerializedProperty _useSeparateAxisCurves;
        private SerializedProperty _useSpeedInsteadOfDuration;

        // Rotation Settings
        private SerializedProperty _startRotation;
        private SerializedProperty _endRotation;
        private SerializedProperty _rotationSpeed;
        private SerializedProperty _rotationCurve;

        // Separate Axis Curves
        private SerializedProperty _xRotationCurve;
        private SerializedProperty _yRotationCurve;
        private SerializedProperty _zRotationCurve;
        #endregion

        void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            // Cache all serialized properties
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));

            _useRelativeRotation = serializedObject.FindProperty(nameof(_useRelativeRotation));
            _useLocalRotation = serializedObject.FindProperty(nameof(_useLocalRotation));
            _snapToRotation = serializedObject.FindProperty(nameof(_snapToRotation));
            _useSeparateAxisCurves = serializedObject.FindProperty(nameof(_useSeparateAxisCurves));
            _useSpeedInsteadOfDuration = serializedObject.FindProperty(nameof(_useSpeedInsteadOfDuration));

            _startRotation = serializedObject.FindProperty(nameof(_startRotation));
            _endRotation = serializedObject.FindProperty(nameof(_endRotation));
            _rotationSpeed = serializedObject.FindProperty(nameof(_rotationSpeed));
            _rotationCurve = serializedObject.FindProperty(nameof(_rotationCurve));

            _xRotationCurve = serializedObject.FindProperty(nameof(_xRotationCurve));
            _yRotationCurve = serializedObject.FindProperty(nameof(_yRotationCurve));
            _zRotationCurve = serializedObject.FindProperty(nameof(_zRotationCurve));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawOptions();
            DrawRotationSettings();

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
                EditorGUILayout.PropertyField(_rotationSpeed, new GUIContent("Rotation Speed", "The speed at which the object rotates per second."));
            }
            else
            {
                EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds for the tween to complete."));
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_useRelativeRotation, new GUIContent("Use Relative Rotation", "If checked, the rotation will be applied relative to the object's current rotation."));
            EditorGUILayout.PropertyField(_useLocalRotation, new GUIContent("Use Local Rotation", "If checked, the rotation will be applied to the object's local transform. Otherwise, it will use the world transform."));
            EditorGUILayout.PropertyField(_snapToRotation, new GUIContent("Snap to Rotation", "If checked, the rotation will snap to the final value without any tweening."));
            EditorGUILayout.PropertyField(_useSeparateAxisCurves, new GUIContent("Use Separate Axis Curves", "If checked, you can define a different animation curve for each axis (X, Y, Z)."));
            EditorGUILayout.EndVertical();
        }

        private void DrawRotationSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Rotation Settings");
            EditorGUILayout.PropertyField(_startRotation, new GUIContent("Start Rotation", "The starting rotation (in Euler angles) for the tween."));
            EditorGUILayout.PropertyField(_endRotation, new GUIContent("End Rotation", "The final rotation (in Euler angles) for the tween."));

            if (!_useSeparateAxisCurves.boolValue)
            {
                EditorGUILayout.PropertyField(_rotationCurve, new GUIContent("Rotation Curve", "The overall curve to apply to the tween for all axes."));
            }
            else
            {
                EditorGUILayout.PropertyField(_xRotationCurve, new GUIContent("X Rotation Curve", "The curve to apply to the X-axis rotation tween."));
                EditorGUILayout.PropertyField(_yRotationCurve, new GUIContent("Y Rotation Curve", "The curve to apply to the Y-axis rotation tween."));
                EditorGUILayout.PropertyField(_zRotationCurve, new GUIContent("Z Rotation Curve", "The curve to apply to the Z-axis rotation tween."));
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