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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.CameraNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(CameraShakeNode))]
    public class CameraShakeNodeEditor : Editor
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
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;

        // Base Settings
        private SerializedProperty _returnToOriginalAfterShaking;
        private SerializedProperty _shakeRotation;
        private SerializedProperty _useWorldSpace;

        // Shake Settings
        private SerializedProperty _rotationMagnitude;
        private SerializedProperty _shakeMagnitude;
        private SerializedProperty _shakeRoughness;
        private SerializedProperty _shakeDirection;
        private SerializedProperty _magnitudeCurve;

        // Position Settings
        private SerializedProperty _lockXPosition;
        private SerializedProperty _lockYPosition;
        private SerializedProperty _lockZPosition;

        // Rotation Settings
        private SerializedProperty _lockXRotation;
        private SerializedProperty _lockYRotation;
        private SerializedProperty _lockZRotation;

        // Recovery Settings
        private SerializedProperty _smoothReturn;
        private SerializedProperty _smoothReturnDuration;
        private SerializedProperty _smoothReturnCurve;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));

            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _returnToOriginalAfterShaking = serializedObject.FindProperty(nameof(_returnToOriginalAfterShaking));
            _shakeRotation = serializedObject.FindProperty(nameof(_shakeRotation));
            _useWorldSpace = serializedObject.FindProperty(nameof(_useWorldSpace));

            // Shake Settings
            _rotationMagnitude = serializedObject.FindProperty(nameof(_rotationMagnitude));
            _shakeMagnitude = serializedObject.FindProperty(nameof(_shakeMagnitude));
            _shakeRoughness = serializedObject.FindProperty(nameof(_shakeRoughness));
            _shakeDirection = serializedObject.FindProperty(nameof(_shakeDirection));
            _magnitudeCurve = serializedObject.FindProperty(nameof(_magnitudeCurve));

            // Position Settings
            _lockXPosition = serializedObject.FindProperty(nameof(_lockXPosition));
            _lockYPosition = serializedObject.FindProperty(nameof(_lockYPosition));
            _lockZPosition = serializedObject.FindProperty(nameof(_lockZPosition));

            // Rotation Settings
            _lockXRotation = serializedObject.FindProperty(nameof(_lockXRotation));
            _lockYRotation = serializedObject.FindProperty(nameof(_lockYRotation));
            _lockZRotation = serializedObject.FindProperty(nameof(_lockZRotation));

            // Recovery Settings
            _smoothReturn = serializedObject.FindProperty(nameof(_smoothReturn));
            _smoothReturnDuration = serializedObject.FindProperty(nameof(_smoothReturnDuration));
            _smoothReturnCurve = serializedObject.FindProperty(nameof(_smoothReturnCurve));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawShakeSettings();
            DrawPositionSettings();
            DrawConditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds the camera will shake."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_returnToOriginalAfterShaking, new GUIContent("Return to Original", "If checked, the camera will smoothly return to its original position after shaking."));
            EditorGUILayout.PropertyField(_shakeRotation, new GUIContent("Shake Rotation", "If checked, the camera's rotation will also shake."));
            EditorGUILayout.PropertyField(_useWorldSpace, new GUIContent("Use World Space", "If checked, the shake effect will be calculated in world space. Otherwise, it will be in local space."));
            EditorGUILayout.EndVertical();
        }

        private void DrawShakeSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Shake Settings");
            EditorGUILayout.PropertyField(_shakeMagnitude, new GUIContent("Shake Magnitude", "The overall strength of the positional shake."));
            EditorGUILayout.PropertyField(_shakeRoughness, new GUIContent("Shake Roughness", "The frequency of the shake; a higher value means more erratic shaking."));
            EditorGUILayout.PropertyField(_shakeDirection, new GUIContent("Shake Direction", "A normalized vector representing the direction of the shake."));
            EditorGUILayout.PropertyField(_magnitudeCurve, new GUIContent("Magnitude Curve", "The curve to apply to the shake magnitude over the duration."));
            EditorGUILayout.EndVertical();
        }

        private void DrawPositionSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Position Settings");
            EditorGUILayout.PropertyField(_lockXPosition, new GUIContent("Lock X Position", "If checked, the camera's X position will not be affected by the shake."));
            EditorGUILayout.PropertyField(_lockYPosition, new GUIContent("Lock Y Position", "If checked, the camera's Y position will not be affected by the shake."));
            EditorGUILayout.PropertyField(_lockZPosition, new GUIContent("Lock Z Position", "If checked, the camera's Z position will not be affected by the shake."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_shakeRotation.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Rotation Settings");
                EditorGUILayout.PropertyField(_rotationMagnitude, new GUIContent("Rotation Magnitude", "The overall strength of the rotational shake."));
                EditorGUILayout.PropertyField(_lockXRotation, new GUIContent("Lock X Rotation", "If checked, the camera's X rotation will not be affected by the shake."));
                EditorGUILayout.PropertyField(_lockYRotation, new GUIContent("Lock Y Rotation", "If checked, the camera's Y rotation will not be affected by the shake."));
                EditorGUILayout.PropertyField(_lockZRotation, new GUIContent("Lock Z Rotation", "If checked, the camera's Z rotation will not be affected by the shake."));
                EditorGUILayout.EndVertical();
            }

            if (_returnToOriginalAfterShaking.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Recovery Settings");
                EditorGUILayout.PropertyField(_smoothReturn, new GUIContent("Smooth Return", "If checked, the camera will smoothly tween back to its original position."));
                if (_smoothReturn.boolValue)
                {
                    EditorGUILayout.PropertyField(_smoothReturnDuration, new GUIContent("Return Duration", "The time in seconds it takes for the camera to return to its original position."));
                    EditorGUILayout.PropertyField(_smoothReturnCurve, new GUIContent("Return Curve", "The curve to apply to the smooth return tween."));
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