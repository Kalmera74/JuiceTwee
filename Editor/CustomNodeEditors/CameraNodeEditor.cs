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

    [CustomEditor(typeof(CameraNode))]
    public class CameraNodeEditor : Editor
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
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;
        private SerializedProperty _projection;
        private SerializedProperty _useCurrentFovAsStarting;
        private SerializedProperty _startingFOV;
        private SerializedProperty _endFOV;
        private SerializedProperty _fovCurve;
        private SerializedProperty _useCurrentSizeAsStarting;
        private SerializedProperty _startingSize;
        private SerializedProperty _endSize;
        private SerializedProperty _sizeCurve;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _projection = serializedObject.FindProperty(nameof(_projection));
            _useCurrentFovAsStarting = serializedObject.FindProperty(nameof(_useCurrentFovAsStarting));
            _startingFOV = serializedObject.FindProperty(nameof(_startingFOV));
            _endFOV = serializedObject.FindProperty(nameof(_endFOV));
            _fovCurve = serializedObject.FindProperty(nameof(_fovCurve));
            _useCurrentSizeAsStarting = serializedObject.FindProperty(nameof(_useCurrentSizeAsStarting));
            _startingSize = serializedObject.FindProperty(nameof(_startingSize));
            _endSize = serializedObject.FindProperty(nameof(_endSize));
            _sizeCurve = serializedObject.FindProperty(nameof(_sizeCurve));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawCameraSettings();
            DrawConditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds over which the camera tween will complete."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawCameraSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Camera Settings");
            EditorGUILayout.PropertyField(_projection, new GUIContent("Projection", "Sets the camera's projection type to either Perspective or Orthographic."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_projection.enumValueIndex == (int)CameraProjectionType.Perspective)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Perspective Settings");
                EditorGUILayout.PropertyField(_useCurrentFovAsStarting, new GUIContent("Use Current FOV as Starting", "If checked, the tween will start from the camera's current field of view."));

                bool isFovEditable = !_useCurrentFovAsStarting.boolValue;
                EditorGUI.BeginDisabledGroup(!isFovEditable);
                EditorGUILayout.PropertyField(_startingFOV, new GUIContent("Starting FOV", "The field of view to begin the tween from."));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(_endFOV, new GUIContent("End FOV", "The field of view to tween to."));
                EditorGUILayout.PropertyField(_fovCurve, new GUIContent("FOV Curve", "The curve to apply to the field of view tween."));
                EditorGUILayout.EndVertical();
            }
            else // Orthographic
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Orthographic Settings");
                EditorGUILayout.PropertyField(_useCurrentSizeAsStarting, new GUIContent("Use Current Size as Starting", "If checked, the tween will start from the camera's current orthographic size."));

                bool isSizeEditable = !_useCurrentSizeAsStarting.boolValue;
                EditorGUI.BeginDisabledGroup(!isSizeEditable);
                EditorGUILayout.PropertyField(_startingSize, new GUIContent("Starting Size", "The orthographic size to begin the tween from."));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(_endSize, new GUIContent("End Size", "The orthographic size to tween to."));
                EditorGUILayout.PropertyField(_sizeCurve, new GUIContent("Size Curve", "The curve to apply to the orthographic size tween."));
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