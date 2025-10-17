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

    [CustomEditor(typeof(ScaleAnimationNode))]
    public class ScaleAnimationNodeEditor : Editor
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
        private SerializedProperty _overrideStartScale;
        private SerializedProperty _startScale;
        private SerializedProperty _returnToInitialScale;
        private SerializedProperty _smoothReturn;
        private SerializedProperty _smoothReturnDuration;
        private SerializedProperty _returnCurveType;

        // Scale Animation Settings
        private SerializedProperty _scaleAnimation;
        private SerializedProperty _lockXScale;
        private SerializedProperty _lockYScale;
        private SerializedProperty _lockZScale;

        // Squish Settings
        private SerializedProperty _squishAmount;
        private SerializedProperty _squishAxis;
        private SerializedProperty _squishCurveType;
        private SerializedProperty _useCustomSquishCurve;
        private SerializedProperty _customSquishCurve;

        // Stretch Settings
        private SerializedProperty _stretchAmount;
        private SerializedProperty _stretchAxis;
        private SerializedProperty _stretchCurveType;
        private SerializedProperty _useCustomStretchCurve;
        private SerializedProperty _customStretchCurve;
        #endregion

        void OnEnable()
        {
            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _overrideStartScale = serializedObject.FindProperty(nameof(_overrideStartScale));
            _startScale = serializedObject.FindProperty(nameof(_startScale));
            _returnToInitialScale = serializedObject.FindProperty(nameof(_returnToInitialScale));
            _smoothReturn = serializedObject.FindProperty(nameof(_smoothReturn));
            _smoothReturnDuration = serializedObject.FindProperty(nameof(_smoothReturnDuration));
            _returnCurveType = serializedObject.FindProperty(nameof(_returnCurveType));

            // Scale Animation Settings
            _scaleAnimation = serializedObject.FindProperty(nameof(_scaleAnimation));
            _lockXScale = serializedObject.FindProperty(nameof(_lockXScale));
            _lockYScale = serializedObject.FindProperty(nameof(_lockYScale));
            _lockZScale = serializedObject.FindProperty(nameof(_lockZScale));

            // Squish Settings
            _squishAmount = serializedObject.FindProperty(nameof(_squishAmount));
            _squishAxis = serializedObject.FindProperty(nameof(_squishAxis));
            _squishCurveType = serializedObject.FindProperty(nameof(_squishCurveType));
            _useCustomSquishCurve = serializedObject.FindProperty(nameof(_useCustomSquishCurve));
            _customSquishCurve = serializedObject.FindProperty(nameof(_customSquishCurve));

            // Stretch Settings
            _stretchAmount = serializedObject.FindProperty(nameof(_stretchAmount));
            _stretchAxis = serializedObject.FindProperty(nameof(_stretchAxis));
            _stretchCurveType = serializedObject.FindProperty(nameof(_stretchCurveType));
            _useCustomStretchCurve = serializedObject.FindProperty(nameof(_useCustomStretchCurve));
            _customStretchCurve = serializedObject.FindProperty(nameof(_customStretchCurve));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawScaleAnimationTypeAndLocks();

            // Conditional drawing based on the selected animation type
            if (_scaleAnimation.enumValueIndex == (int)ScaleAnimation.Squish)
            {
                DrawSquishSettings();
            }
            else if (_scaleAnimation.enumValueIndex == (int)ScaleAnimation.Stretch)
            {
                DrawStretchSettings();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds to complete the animation."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_overrideStartScale, new GUIContent("Override Start Scale", "If checked, the animation will begin from the specified Start Scale, ignoring the GameObject's current scale."));
            if (_overrideStartScale.boolValue)
            {
                EditorGUILayout.PropertyField(_startScale, new GUIContent("Start Scale", "The scale value to start the animation from."));
            }

            EditorGUILayout.PropertyField(_returnToInitialScale, new GUIContent("Return to Initial Scale", "If checked, the object will smoothly return to its initial scale after the animation completes."));
            if (_returnToInitialScale.boolValue)
            {
                EditorGUILayout.PropertyField(_smoothReturn, new GUIContent("Smooth Return", "If checked, the return animation will be a smooth tween. If unchecked, it will snap back instantly."));
                if (_smoothReturn.boolValue)
                {
                    EditorGUILayout.PropertyField(_smoothReturnDuration, new GUIContent("Smooth Return Duration", "The duration in seconds for the smooth return animation."));
                    EditorGUILayout.PropertyField(_returnCurveType, new GUIContent("Return Curve Type", "The type of animation curve to use for the smooth return."));
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawScaleAnimationTypeAndLocks()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Scale Animation Settings");
            EditorGUILayout.PropertyField(_scaleAnimation, new GUIContent("Scale Animation", "The type of animation to perform on the scale."));
            EditorGUILayout.PropertyField(_lockXScale, new GUIContent("Lock X Scale", "If checked, the X-axis scale will not change during the animation."));
            EditorGUILayout.PropertyField(_lockYScale, new GUIContent("Lock Y Scale", "If checked, the Y-axis scale will not change during the animation."));
            EditorGUILayout.PropertyField(_lockZScale, new GUIContent("Lock Z Scale", "If checked, the Z-axis scale will not change during the animation."));
            EditorGUILayout.EndVertical();
        }

        private void DrawSquishSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Squish Settings");
            EditorGUILayout.PropertyField(_squishAmount, new GUIContent("Squish Amount", "The factor by which to squish the object (e.g., 0.5 will halve the scale)."));
            EditorGUILayout.PropertyField(_squishAxis, new GUIContent("Squish Axis", "The primary axis to apply the squish effect to."));
            EditorGUILayout.PropertyField(_useCustomSquishCurve, new GUIContent("Use Custom Curve", "If checked, you can define a custom AnimationCurve for the squish effect."));
            if (_useCustomSquishCurve.boolValue)
            {
                EditorGUILayout.PropertyField(_customSquishCurve, new GUIContent("Custom Squish Curve", "The custom curve to control the squish animation."));
            }
            else
            {
                EditorGUILayout.PropertyField(_squishCurveType, new GUIContent("Squish Curve Type", "The preset curve type to use for the squish animation."));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStretchSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Stretch Settings");
            EditorGUILayout.PropertyField(_stretchAmount, new GUIContent("Stretch Amount", "The factor by which to stretch the object (e.g., 2.0 will double the scale)."));
            EditorGUILayout.PropertyField(_stretchAxis, new GUIContent("Stretch Axis", "The primary axis to apply the stretch effect to."));
            EditorGUILayout.PropertyField(_useCustomStretchCurve, new GUIContent("Use Custom Curve", "If checked, you can define a custom AnimationCurve for the stretch effect."));
            if (_useCustomStretchCurve.boolValue)
            {
                EditorGUILayout.PropertyField(_customStretchCurve, new GUIContent("Custom Stretch Curve", "The custom curve to control the stretch animation."));
            }
            else
            {
                EditorGUILayout.PropertyField(_stretchCurveType, new GUIContent("Stretch Curve Type", "The preset curve type to use for the stretch animation."));
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