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

    [CustomEditor(typeof(CanvasGroupNode))]
    public class CanvasGroupNodeEditor : Editor
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
        private SerializedProperty _controlAlpha;
        private SerializedProperty _controlInteractable;
        private SerializedProperty _controlBlocksRaycasts;

        // Alpha Settings
        private SerializedProperty _useCurrentAlphaAsStart;
        private SerializedProperty _startAlpha;
        private SerializedProperty _endAlpha;
        private SerializedProperty _alphaCurve;

        // Boolean State Settings
        private SerializedProperty _isInteractable;
        private SerializedProperty _blockRayCast;
        #endregion

        void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));

            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _duration = serializedObject.FindProperty(nameof(_duration));

            _controlAlpha = serializedObject.FindProperty(nameof(_controlAlpha));
            _controlInteractable = serializedObject.FindProperty(nameof(_controlInteractable));
            _controlBlocksRaycasts = serializedObject.FindProperty(nameof(_controlBlocksRaycasts));

            _useCurrentAlphaAsStart = serializedObject.FindProperty(nameof(_useCurrentAlphaAsStart));
            _startAlpha = serializedObject.FindProperty(nameof(_startAlpha));
            _endAlpha = serializedObject.FindProperty(nameof(_endAlpha));
            _alphaCurve = serializedObject.FindProperty(nameof(_alphaCurve));

            _isInteractable = serializedObject.FindProperty(nameof(_isInteractable));
            _blockRayCast = serializedObject.FindProperty(nameof(_blockRayCast));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawAlphaSettings();
            DrawBooleanStateSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The time in seconds over which the alpha tween will complete."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlAlpha, new GUIContent("Control Alpha", "Enable to animate the Canvas Group's alpha value."));
            EditorGUILayout.PropertyField(_controlInteractable, new GUIContent("Control Interactable", "Enable to set the Canvas Group's 'interactable' property."));
            EditorGUILayout.PropertyField(_controlBlocksRaycasts, new GUIContent("Control Blocks Raycasts", "Enable to set the Canvas Group's 'blocks raycasts' property."));
            EditorGUILayout.EndVertical();
        }

        private void DrawAlphaSettings()
        {
            if (_controlAlpha.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Alpha Settings");
                EditorGUILayout.PropertyField(_useCurrentAlphaAsStart, new GUIContent("Use Current Alpha as Start", "If checked, the alpha tween will start from the Canvas Group's current alpha at the time of execution."));

                if (!_useCurrentAlphaAsStart.boolValue)
                {
                    EditorGUILayout.PropertyField(_startAlpha, new GUIContent("Start Alpha", "The starting alpha value for the tween."));
                }

                EditorGUILayout.PropertyField(_endAlpha, new GUIContent("End Alpha", "The final alpha value for the tween."));
                EditorGUILayout.PropertyField(_alphaCurve, new GUIContent("Alpha Curve", "The curve that controls the alpha tween's progression."));
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawBooleanStateSettings()
        {
            if (_controlInteractable.boolValue || _controlBlocksRaycasts.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Boolean State Settings");

                if (_controlInteractable.boolValue)
                {
                    EditorGUILayout.PropertyField(_isInteractable, new GUIContent("Is Interactable", "The new value for the Canvas Group's 'interactable' property."));
                }
                if (_controlBlocksRaycasts.boolValue)
                {
                    EditorGUILayout.PropertyField(_blockRayCast, new GUIContent("Blocks Raycasts", "The new value for the Canvas Group's 'blocks raycasts' property."));
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