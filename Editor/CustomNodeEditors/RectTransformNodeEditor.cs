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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.UINodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(RectTransformNode))]
    public class RectTransformNodeEditor : Editor
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

        // Base Settings
        private SerializedProperty _controlAnchoredPosition;
        private SerializedProperty _controlSizeDelta;
        private SerializedProperty _controlRotation;
        private SerializedProperty _controlScale;
        private SerializedProperty _useTargetForValues;

        // Animation Settings
        private SerializedProperty _useCurrentAsStart;
        private SerializedProperty _useSeparateAxisCurves;
        private SerializedProperty _blendCurve;
        private SerializedProperty _xCurve;
        private SerializedProperty _yCurve;
        private SerializedProperty _zCurve;

        // Position Values
        private SerializedProperty _startAnchoredPosition;
        private SerializedProperty _endAnchoredPosition;

        // Size Delta Values
        private SerializedProperty _startSizeDelta;
        private SerializedProperty _endSizeDelta;

        // Rotation Values
        private SerializedProperty _startRotation;
        private SerializedProperty _endRotation;

        // Scale Values
        private SerializedProperty _startScale;
        private SerializedProperty _endScale;
        #endregion

        void OnEnable()
        {

            _nodeName = serializedObject.FindProperty(nameof(_nodeName));

            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));

            _controlAnchoredPosition = serializedObject.FindProperty(nameof(_controlAnchoredPosition));
            _controlSizeDelta = serializedObject.FindProperty(nameof(_controlSizeDelta));
            _controlRotation = serializedObject.FindProperty(nameof(_controlRotation));
            _controlScale = serializedObject.FindProperty(nameof(_controlScale));
            _useTargetForValues = serializedObject.FindProperty(nameof(_useTargetForValues));

            _useCurrentAsStart = serializedObject.FindProperty(nameof(_useCurrentAsStart));
            _useSeparateAxisCurves = serializedObject.FindProperty(nameof(_useSeparateAxisCurves));

            _blendCurve = serializedObject.FindProperty(nameof(_blendCurve));
            _xCurve = serializedObject.FindProperty(nameof(_xCurve));
            _yCurve = serializedObject.FindProperty(nameof(_yCurve));
            _zCurve = serializedObject.FindProperty(nameof(_zCurve));

            _startAnchoredPosition = serializedObject.FindProperty(nameof(_startAnchoredPosition));
            _endAnchoredPosition = serializedObject.FindProperty(nameof(_endAnchoredPosition));

            _startSizeDelta = serializedObject.FindProperty(nameof(_startSizeDelta));
            _endSizeDelta = serializedObject.FindProperty(nameof(_endSizeDelta));

            _startRotation = serializedObject.FindProperty(nameof(_startRotation));
            _endRotation = serializedObject.FindProperty(nameof(_endRotation));

            _startScale = serializedObject.FindProperty(nameof(_startScale));
            _endScale = serializedObject.FindProperty(nameof(_endScale));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();


            DrawTimeOptions();
            DrawBaseSettings();
            DrawAnimationSettings();
            DrawPositionValues();
            DrawSizeDeltaValues();
            DrawRotationValues();
            DrawScaleValues();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the tween duration will be independent of Time.timeScale."));
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds over which the tween will complete."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlAnchoredPosition, new GUIContent("Control Anchored Position", "If checked, this node will tween the anchored position of the RectTransform."));
            EditorGUILayout.PropertyField(_controlSizeDelta, new GUIContent("Control Size Delta", "If checked, this node will tween the size delta of the RectTransform."));
            EditorGUILayout.PropertyField(_controlRotation, new GUIContent("Control Rotation", "If checked, this node will tween the local Euler rotation of the RectTransform."));
            EditorGUILayout.PropertyField(_controlScale, new GUIContent("Control Scale", "If checked, this node will tween the local scale of the RectTransform."));
            EditorGUILayout.PropertyField(_useTargetForValues, new GUIContent("Use Target for Values", "If checked, the target's current values will be used to initialize the start or end tween values at runtime."));
            EditorGUILayout.EndVertical();
        }

        private void DrawAnimationSettings()
        {
            if (_controlAnchoredPosition.boolValue || _controlSizeDelta.boolValue || _controlRotation.boolValue || _controlScale.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Animation Settings");
                EditorGUILayout.PropertyField(_useCurrentAsStart, new GUIContent("Use Current as Start", "If checked, the tween will start from the RectTransform's current value at the time of execution."));
                EditorGUILayout.PropertyField(_useSeparateAxisCurves, new GUIContent("Use Separate Axis Curves", "If checked, you can define a different animation curve for each axis (X, Y, Z)."));

                if (_useSeparateAxisCurves.boolValue)
                {
                    EditorGUILayout.PropertyField(_xCurve, new GUIContent("X Curve", "The curve to apply to the X-axis tween."));
                    EditorGUILayout.PropertyField(_yCurve, new GUIContent("Y Curve", "The curve to apply to the Y-axis tween."));
                    EditorGUILayout.PropertyField(_zCurve, new GUIContent("Z Curve", "The curve to apply to the Z-axis tween."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_blendCurve, new GUIContent("Blend Curve", "The overall curve to apply to the tween for all axes."));
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawPositionValues()
        {
            if (_controlAnchoredPosition.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Position Values");

                if (!_useCurrentAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startAnchoredPosition, new GUIContent("Start Anchored Position", "The starting anchored position for the tween."));
                }

                EditorGUILayout.PropertyField(_endAnchoredPosition, new GUIContent("End Anchored Position", "The final anchored position for the tween."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawSizeDeltaValues()
        {
            if (_controlSizeDelta.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Size Delta Values");

                if (!_useCurrentAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startSizeDelta, new GUIContent("Start Size Delta", "The starting size delta for the tween."));
                }

                EditorGUILayout.PropertyField(_endSizeDelta, new GUIContent("End Size Delta", "The final size delta for the tween."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawRotationValues()
        {
            if (_controlRotation.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Rotation Values (Euler)");

                if (!_useCurrentAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startRotation, new GUIContent("Start Rotation", "The starting local Euler rotation for the tween."));
                }

                EditorGUILayout.PropertyField(_endRotation, new GUIContent("End Rotation", "The final local Euler rotation for the tween."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawScaleValues()
        {
            if (_controlScale.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Scale Values");

                if (!_useCurrentAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startScale, new GUIContent("Start Scale", "The starting local scale for the tween."));
                }

                EditorGUILayout.PropertyField(_endScale, new GUIContent("End Scale", "The final local scale for the tween."));
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