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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(FogNode))]
    public class FogNodeEditor : Editor
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
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;

        // Base Settings
        private SerializedProperty _controlEnabledState;
        private SerializedProperty _controlFogMode;
        private SerializedProperty _tweenFogColor;

        // Tween Settings
        private SerializedProperty _useCurrentAsStart;
        private SerializedProperty _blendCurve;

        // Enabled State
        private SerializedProperty _isFogEnabled;

        // Fog Mode
        private SerializedProperty _fogMode;

        // Color Values
        private SerializedProperty _startColor;
        private SerializedProperty _endColor;

        // Density Values (For Exp/Exp2 Modes)
        private SerializedProperty _startDensity;
        private SerializedProperty _endDensity;

        // Range Values (For Linear Mode)
        private SerializedProperty _startLinearStart;
        private SerializedProperty _startLinearEnd;
        private SerializedProperty _endLinearStart;
        private SerializedProperty _endLinearEnd;
        #endregion

        private void OnEnable()
        {
            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _controlEnabledState = serializedObject.FindProperty(nameof(_controlEnabledState));
            _controlFogMode = serializedObject.FindProperty(nameof(_controlFogMode));
            _tweenFogColor = serializedObject.FindProperty(nameof(_tweenFogColor));

            // Tween Settings
            _useCurrentAsStart = serializedObject.FindProperty(nameof(_useCurrentAsStart));
            _blendCurve = serializedObject.FindProperty(nameof(_blendCurve));

            // Enabled State
            _isFogEnabled = serializedObject.FindProperty(nameof(_isFogEnabled));

            // Fog Mode
            _fogMode = serializedObject.FindProperty(nameof(_fogMode));

            // Color Values
            _startColor = serializedObject.FindProperty(nameof(_startColor));
            _endColor = serializedObject.FindProperty(nameof(_endColor));

            // Density Values
            _startDensity = serializedObject.FindProperty(nameof(_startDensity));
            _endDensity = serializedObject.FindProperty(nameof(_endDensity));

            // Range Values
            _startLinearStart = serializedObject.FindProperty(nameof(_startLinearStart));
            _startLinearEnd = serializedObject.FindProperty(nameof(_startLinearEnd));
            _endLinearStart = serializedObject.FindProperty(nameof(_endLinearStart));
            _endLinearEnd = serializedObject.FindProperty(nameof(_endLinearEnd));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawTweenSettings();
            DrawConditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds for the tween to complete."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlEnabledState, new GUIContent("Control Enabled State", "If checked, this node will enable or disable the scene fog."));
            EditorGUILayout.PropertyField(_controlFogMode, new GUIContent("Control Fog Mode", "If checked, this node will set the fog mode and its associated values."));
            EditorGUILayout.PropertyField(_tweenFogColor, new GUIContent("Tween Fog Color", "If checked, this node will tween the fog color."));
            EditorGUILayout.EndVertical();
        }

        private void DrawTweenSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Tween Settings");
            EditorGUILayout.PropertyField(_useCurrentAsStart, new GUIContent("Use Current As Start", "If checked, the tween will start from the scene's current fog color, density, or range."));
            EditorGUILayout.PropertyField(_blendCurve, new GUIContent("Blend Curve", "The curve to apply to the tween for color, density, or range."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_controlEnabledState.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Enabled State");
                EditorGUILayout.PropertyField(_isFogEnabled, new GUIContent("Is Fog Enabled", "The new state of the fog (enabled or disabled)."));
                EditorGUILayout.EndVertical();
            }

            if (_controlFogMode.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Fog Mode");
                EditorGUILayout.PropertyField(_fogMode, new GUIContent("Fog Mode", "The new fog mode to apply (Linear, Exponential, or Exponential Squared)."));
                EditorGUILayout.EndVertical();

                int currentFogMode = _fogMode.enumValueIndex;
                if (currentFogMode == 0) // Linear
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    DrawHeader("Range Values");
                    if (!_useCurrentAsStart.boolValue)
                    {
                        EditorGUILayout.PropertyField(_startLinearStart, new GUIContent("Start Linear Start", "The starting 'Start' value for the Linear fog range tween."));
                        EditorGUILayout.PropertyField(_startLinearEnd, new GUIContent("Start Linear End", "The starting 'End' value for the Linear fog range tween."));
                    }
                    EditorGUILayout.PropertyField(_endLinearStart, new GUIContent("End Linear Start", "The final 'Start' value for the Linear fog range tween."));
                    EditorGUILayout.PropertyField(_endLinearEnd, new GUIContent("End Linear End", "The final 'End' value for the Linear fog range tween."));
                    EditorGUILayout.EndVertical();
                }
                else // Exponential / Exponential Squared
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    DrawHeader("Density Values");
                    if (!_useCurrentAsStart.boolValue)
                    {
                        EditorGUILayout.PropertyField(_startDensity, new GUIContent("Start Density", "The starting density for the Exponential/Exp2 fog density tween."));
                    }
                    EditorGUILayout.PropertyField(_endDensity, new GUIContent("End Density", "The final density for the Exponential/Exp2 fog density tween."));
                    EditorGUILayout.EndVertical();
                }
            }

            if (_tweenFogColor.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Color Values");
                if (!_useCurrentAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startColor, new GUIContent("Start Color", "The starting color for the fog tween."));
                }
                EditorGUILayout.PropertyField(_endColor, new GUIContent("End Color", "The final color for the fog tween."));
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