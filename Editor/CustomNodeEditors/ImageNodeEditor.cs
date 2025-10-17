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

    [CustomEditor(typeof(ImageNode))]
    public class ImageNodeEditor : Editor
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
        private SerializedProperty _controlSprites;
        private SerializedProperty _controlColor;
        private SerializedProperty _controlFillRate;

        // Color Options
        private SerializedProperty _separateChannelCurves;
        private SerializedProperty _useCurrentAsStart;
        private SerializedProperty _useTargetsForColor;
        private SerializedProperty _useGradientInstead;
        private SerializedProperty _gradient;
        private SerializedProperty _startColor;
        private SerializedProperty _endColor;
        private SerializedProperty _blendCurve;
        private SerializedProperty _rCurve;
        private SerializedProperty _gCurve;
        private SerializedProperty _bCurve;
        private SerializedProperty _aCurve;

        // Fill Settings
        private SerializedProperty _startFillAmount;
        private SerializedProperty _endFillAmount;
        private SerializedProperty _fillCurve;

        // Sprite Settings
        private SerializedProperty _sprites;
        #endregion

        void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            // Cache all serialized properties
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));

            _controlSprites = serializedObject.FindProperty(nameof(_controlSprites));
            _controlColor = serializedObject.FindProperty(nameof(_controlColor));
            _controlFillRate = serializedObject.FindProperty(nameof(_controlFillRate));

            _separateChannelCurves = serializedObject.FindProperty(nameof(_separateChannelCurves));
            _useCurrentAsStart = serializedObject.FindProperty(nameof(_useCurrentAsStart));
            _useTargetsForColor = serializedObject.FindProperty(nameof(_useTargetsForColor));
            _useGradientInstead = serializedObject.FindProperty(nameof(_useGradientInstead));

            _gradient = serializedObject.FindProperty(nameof(_gradient));
            _startColor = serializedObject.FindProperty(nameof(_startColor));
            _endColor = serializedObject.FindProperty(nameof(_endColor));

            _blendCurve = serializedObject.FindProperty(nameof(_blendCurve));
            _rCurve = serializedObject.FindProperty(nameof(_rCurve));
            _gCurve = serializedObject.FindProperty(nameof(_gCurve));
            _bCurve = serializedObject.FindProperty(nameof(_bCurve));
            _aCurve = serializedObject.FindProperty(nameof(_aCurve));

            _startFillAmount = serializedObject.FindProperty(nameof(_startFillAmount));
            _endFillAmount = serializedObject.FindProperty(nameof(_endFillAmount));
            _fillCurve = serializedObject.FindProperty(nameof(_fillCurve));

            _sprites = serializedObject.FindProperty(nameof(_sprites));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawColorOptions();
            DrawFillSettings();
            DrawSpriteSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds over which the effects will complete."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlSprites, new GUIContent("Control Sprites", "Enable to animate or change the Image component's sprite."));
            EditorGUILayout.PropertyField(_controlColor, new GUIContent("Control Color", "Enable to animate the Image component's color."));
            EditorGUILayout.PropertyField(_controlFillRate, new GUIContent("Control Fill Rate", "Enable to animate the Image component's fill amount. Only works if the Image Type is set to 'Filled'."));
            EditorGUILayout.EndVertical();
        }

        private void DrawColorOptions()
        {
            if (_controlColor.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Color Options");

                EditorGUILayout.PropertyField(_separateChannelCurves, new GUIContent("Separate Channel Curves", "If checked, you can define a different animation curve for each color channel (R, G, B, A)."));
                EditorGUILayout.PropertyField(_useCurrentAsStart, new GUIContent("Use Current as Start", "If checked, the color tween will start from the image's current color at the time of execution."));
                EditorGUILayout.PropertyField(_useTargetsForColor, new GUIContent("Use Targets for Color", "If checked, the Start and End colors/gradients will be determined by a specified target."));
                EditorGUILayout.PropertyField(_useGradientInstead, new GUIContent("Use Gradient Instead", "If checked, the color tween will use gradients instead of single colors."));

                if (!_useTargetsForColor.boolValue)
                {
                    if (_useGradientInstead.boolValue)
                    {
                        EditorGUILayout.PropertyField(_gradient, new GUIContent("Gradient", "The gradient to apply to the image's color."));
                    }
                    else
                    {
                        if (!_useCurrentAsStart.boolValue)
                        {
                            EditorGUILayout.PropertyField(_startColor, new GUIContent("Start Color", "The starting color for the tween."));
                        }
                        EditorGUILayout.PropertyField(_endColor, new GUIContent("End Color", "The final color for the tween."));
                    }
                }
                if (_separateChannelCurves.boolValue)
                {
                    EditorGUILayout.PropertyField(_rCurve, new GUIContent("R Curve", "The curve to apply to the Red channel tween."));
                    EditorGUILayout.PropertyField(_gCurve, new GUIContent("G Curve", "The curve to apply to the Green channel tween."));
                    EditorGUILayout.PropertyField(_bCurve, new GUIContent("B Curve", "The curve to apply to the Blue channel tween."));
                    EditorGUILayout.PropertyField(_aCurve, new GUIContent("A Curve", "The curve to apply to the Alpha channel tween."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_blendCurve, new GUIContent("Blend Curve", "The overall curve to apply to the color tween for all channels."));
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawFillSettings()
        {
            if (_controlFillRate.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Fill Settings");
                EditorGUILayout.PropertyField(_startFillAmount, new GUIContent("Start Fill Amount", "The starting fill amount for the tween (0.0 to 1.0)."));
                EditorGUILayout.PropertyField(_endFillAmount, new GUIContent("End Fill Amount", "The final fill amount for the tween (0.0 to 1.0)."));
                EditorGUILayout.PropertyField(_fillCurve, new GUIContent("Fill Curve", "The curve that controls the fill tween's progression."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawSpriteSettings()
        {
            if (_controlSprites.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Sprite Settings");
                EditorGUILayout.PropertyField(_sprites, new GUIContent("Sprites", "A list of sprites to be set on the target Image component. It will go from first to last in duration "));
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