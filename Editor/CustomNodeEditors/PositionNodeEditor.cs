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

    [CustomEditor(typeof(PositionNode))]
    public class PositionNodeEditor : Editor
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
        // Time Options
        private SerializedProperty _useUnscaledTime;
        private SerializedProperty _duration;
        private SerializedProperty _useSpeedInsteadOfDuration;
        private SerializedProperty _speedInUnitsPerSeconds;

        // Base Settings
        private SerializedProperty _usePathMovement;
        private SerializedProperty _useRelativePosition;
        private SerializedProperty _useLocalPosition;
        private SerializedProperty _snapToPosition;

        // Path Settings
        private SerializedProperty _pathPoints;
        private SerializedProperty _useBezierPath;

        // Position Settings
        private SerializedProperty _useSeparateAxisCurves;
        private SerializedProperty _useTargetForPositions;
        private SerializedProperty _startPosition;
        private SerializedProperty _endPosition;
        private SerializedProperty _positionCurve;
        private SerializedProperty _xPositionCurve;
        private SerializedProperty _yPositionCurve;
        private SerializedProperty _zPositionCurve;
        private SerializedProperty _startPositionTarget;
        private SerializedProperty _endPositionTarget;
        #endregion

        void OnEnable()
        {
            // Cache all serialized properties
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useSpeedInsteadOfDuration = serializedObject.FindProperty(nameof(_useSpeedInsteadOfDuration));
            _speedInUnitsPerSeconds = serializedObject.FindProperty(nameof(_speedInUnitsPerSeconds));

            _usePathMovement = serializedObject.FindProperty(nameof(_usePathMovement));
            _useRelativePosition = serializedObject.FindProperty(nameof(_useRelativePosition));
            _useLocalPosition = serializedObject.FindProperty(nameof(_useLocalPosition));
            _snapToPosition = serializedObject.FindProperty(nameof(_snapToPosition));

            _pathPoints = serializedObject.FindProperty(nameof(_pathPoints));
            _useBezierPath = serializedObject.FindProperty(nameof(_useBezierPath));

            _useSeparateAxisCurves = serializedObject.FindProperty(nameof(_useSeparateAxisCurves));
            _useTargetForPositions = serializedObject.FindProperty(nameof(_useTargetForPositions));
            _startPosition = serializedObject.FindProperty(nameof(_startPosition));
            _endPosition = serializedObject.FindProperty(nameof(_endPosition));
            _positionCurve = serializedObject.FindProperty(nameof(_positionCurve));

            _xPositionCurve = serializedObject.FindProperty(nameof(_xPositionCurve));
            _yPositionCurve = serializedObject.FindProperty(nameof(_yPositionCurve));
            _zPositionCurve = serializedObject.FindProperty(nameof(_zPositionCurve));

            _startPositionTarget = serializedObject.FindProperty(nameof(_startPositionTarget));
            _endPositionTarget = serializedObject.FindProperty(nameof(_endPositionTarget));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTimeOptions();
            DrawBaseSettings();

            if (_usePathMovement.boolValue)
            {
                DrawPathSettings();
            }
            else
            {
                DrawPositionSettings();
            }

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
                EditorGUILayout.PropertyField(_speedInUnitsPerSeconds, new GUIContent("Speed (Units/Sec)", "The speed at which the object moves per second."));
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
            EditorGUILayout.PropertyField(_usePathMovement, new GUIContent("Use Path Movement", "If checked, the object will follow a path defined by points rather than a single start/end position."));
            EditorGUILayout.PropertyField(_useRelativePosition, new GUIContent("Use Relative Position", "If checked, the start and end positions will be treated as relative values to the object's current position."));
            EditorGUILayout.PropertyField(_useLocalPosition, new GUIContent("Use Local Position", "If checked, the position will be applied to the object's local transform. Otherwise, it will use the world transform."));
            EditorGUILayout.PropertyField(_snapToPosition, new GUIContent("Snap to Position", "If checked, the position will snap to the final value without any tweening."));
            EditorGUILayout.EndVertical();
        }

        private void DrawPathSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Path Settings");
            EditorGUILayout.PropertyField(_useBezierPath, new GUIContent("Use Bezier Path", "If checked, the path will be smoothed using a bezier curve."));
            EditorGUILayout.PropertyField(_pathPoints, new GUIContent("Path Points", "A list of Vector3 points that define the path for the object to follow."));
            EditorGUILayout.EndVertical();
        }

        private void DrawPositionSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Position Settings");
            EditorGUILayout.PropertyField(_useSeparateAxisCurves, new GUIContent("Use Separate Axis Curves", "If checked, you can define a different animation curve for each axis (X, Y, Z)."));
            EditorGUILayout.PropertyField(_useTargetForPositions, new GUIContent("Use Target for Positions", "If checked, the start and end positions will be determined by the position of a specified Transform."));

            if (!_useTargetForPositions.boolValue)
            {
                EditorGUILayout.PropertyField(_startPosition, new GUIContent("Start Position", "The starting position for the tween."));
                EditorGUILayout.PropertyField(_endPosition, new GUIContent("End Position", "The final position for the tween."));
            }
            else
            {
                EditorGUILayout.PropertyField(_startPositionTarget, new GUIContent("Start Position Target", "The Transform whose position will be used as the starting position."));
                EditorGUILayout.PropertyField(_endPositionTarget, new GUIContent("End Position Target", "The Transform whose position will be used as the final position."));
            }

            if (_useSeparateAxisCurves.boolValue)
            {
                EditorGUILayout.PropertyField(_xPositionCurve, new GUIContent("X Position Curve", "The curve to apply to the X-axis position tween."));
                EditorGUILayout.PropertyField(_yPositionCurve, new GUIContent("Y Position Curve", "The curve to apply to the Y-axis position tween."));
                EditorGUILayout.PropertyField(_zPositionCurve, new GUIContent("Z Position Curve", "The curve to apply to the Z-axis position tween."));
            }
            else
            {
                EditorGUILayout.PropertyField(_positionCurve, new GUIContent("Position Curve", "The overall curve to apply to the tween for all axes."));
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