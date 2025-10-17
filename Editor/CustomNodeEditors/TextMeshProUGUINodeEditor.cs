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
    [CustomEditor(typeof(TextMeshProUGUINode))]
    public class TextMeshProUGUINodeEditor : Editor
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
        // Base properties
        private SerializedProperty _nodeName;
        private SerializedProperty _useUnscaledTime;
        private SerializedProperty _duration;

        // Control properties
        private SerializedProperty _controlText;
        private SerializedProperty _controlColor;
        private SerializedProperty _controlFontSize;

        // Text Settings
        private SerializedProperty _text;
        private SerializedProperty _useTypewriterEffect;

        // Font Size Settings
        private SerializedProperty _useCurrentFontSizeAsStart;
        private SerializedProperty _startFontSize;
        private SerializedProperty _endFontSize;
        private SerializedProperty _fontSizeCurve;

        // Color Settings
        private SerializedProperty _useCurrentColorAsStart;
        private SerializedProperty _useGradientInstead;
        private SerializedProperty _colorBlendCurve;
        private SerializedProperty _startColor;
        private SerializedProperty _endColor;
        private SerializedProperty _startGradient;
        private SerializedProperty _endGradient;

        #endregion

        void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));

            _controlText = serializedObject.FindProperty(nameof(_controlText));
            _controlColor = serializedObject.FindProperty(nameof(_controlColor));
            _controlFontSize = serializedObject.FindProperty(nameof(_controlFontSize));

            _text = serializedObject.FindProperty(nameof(_text));
            _useTypewriterEffect = serializedObject.FindProperty(nameof(_useTypewriterEffect));

            _useCurrentFontSizeAsStart = serializedObject.FindProperty(nameof(_useCurrentFontSizeAsStart));
            _startFontSize = serializedObject.FindProperty(nameof(_startFontSize));
            _endFontSize = serializedObject.FindProperty(nameof(_endFontSize));
            _fontSizeCurve = serializedObject.FindProperty(nameof(_fontSizeCurve));

            _useCurrentColorAsStart = serializedObject.FindProperty(nameof(_useCurrentColorAsStart));
            _useGradientInstead = serializedObject.FindProperty(nameof(_useGradientInstead));
            _colorBlendCurve = serializedObject.FindProperty(nameof(_colorBlendCurve));

            _startColor = serializedObject.FindProperty(nameof(_startColor));
            _endColor = serializedObject.FindProperty(nameof(_endColor));

            _startGradient = serializedObject.FindProperty(nameof(_startGradient));
            _endGradient = serializedObject.FindProperty(nameof(_endGradient));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseControlProperties();
            DrawTextSettings();
            DrawFontSizeSettings();
            DrawColorSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time it takes to complete the text, font, and color effects."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseControlProperties()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlText, new GUIContent("Control Text", "Enable to change the text content."));
            EditorGUILayout.PropertyField(_controlColor, new GUIContent("Control Color", "Enable to change the text color."));
            EditorGUILayout.PropertyField(_controlFontSize, new GUIContent("Control Font Size", "Enable to change the font size."));
            EditorGUILayout.EndVertical();
        }

        private void DrawTextSettings()
        {
            if (_controlText.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Text Settings");
                EditorGUILayout.PropertyField(_text, new GUIContent("Text", "The new text to display."));
                EditorGUILayout.PropertyField(_useTypewriterEffect, new GUIContent("Use Typewriter Effect", "If checked, the text will be revealed character by character."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawFontSizeSettings()
        {
            if (_controlFontSize.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Font Size Settings");
                EditorGUILayout.PropertyField(_useCurrentFontSizeAsStart, new GUIContent("Use Current Size as Start", "If checked, the font size will start from the text's current size."));

                if (!_useCurrentFontSizeAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startFontSize, new GUIContent("Start Font Size", "The starting font size for the tween."));
                }

                EditorGUILayout.PropertyField(_endFontSize, new GUIContent("End Font Size", "The final font size for the tween."));
                EditorGUILayout.PropertyField(_fontSizeCurve, new GUIContent("Font Size Curve", "The curve that controls the tween's progression over time."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawColorSettings()
        {
            if (_controlColor.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Color Settings");
                EditorGUILayout.PropertyField(_useCurrentColorAsStart, new GUIContent("Use Current Color as Start", "If checked, the text color will start from the text's current color."));
                EditorGUILayout.PropertyField(_useGradientInstead, new GUIContent("Use Gradient Instead", "If checked, the color tween will use gradients instead of single colors."));

                if (_useGradientInstead.boolValue)
                {
                    EditorGUILayout.PropertyField(_startGradient, new GUIContent("Start Gradient", "The starting color gradient."));
                    EditorGUILayout.PropertyField(_endGradient, new GUIContent("End Gradient", "The final color gradient."));
                }
                else
                {
                    if (!_useCurrentColorAsStart.boolValue)
                    {
                        EditorGUILayout.PropertyField(_startColor, new GUIContent("Start Color", "The starting color for the tween."));
                    }
                    EditorGUILayout.PropertyField(_endColor, new GUIContent("End Color", "The final color for the tween."));
                }

                EditorGUILayout.PropertyField(_colorBlendCurve, new GUIContent("Color Blend Curve", "The curve that controls the color tween's progression over time."));
                EditorGUILayout.EndVertical();
            }
        }

        public void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}