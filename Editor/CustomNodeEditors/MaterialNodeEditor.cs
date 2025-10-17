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
using System.Collections.Generic;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(MaterialNode))]
    public class MaterialNodeEditor : Editor
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
        private SerializedProperty _controlParameters;
        private SerializedProperty _controlOffset;
        private SerializedProperty _controlColor;
        private SerializedProperty _controlTexture;

        // Property Settings
        private SerializedProperty _parameters;

        // Texture Settings
        private SerializedProperty _useRandomTexture;
        private SerializedProperty _texturePropertyName;
        private SerializedProperty _textures;

        // Offset Settings
        private SerializedProperty _scrollSpeed;
        private SerializedProperty _scrollDirection;
        private SerializedProperty _scrollCurve;

        // Color Settings
        private SerializedProperty _startingColor;
        private SerializedProperty _endColor;
        private SerializedProperty _colorCurve;
        #endregion

        private bool _showPropertyListFoldout = true; // Renamed for clarity
        private List<bool> _foldoutStates = new List<bool>();

        private void OnEnable()
        {
            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _controlParameters = serializedObject.FindProperty(nameof(_controlParameters));
            _controlOffset = serializedObject.FindProperty(nameof(_controlOffset));
            _controlColor = serializedObject.FindProperty(nameof(_controlColor));
            _controlTexture = serializedObject.FindProperty(nameof(_controlTexture));

            // Property Settings
            _parameters = serializedObject.FindProperty(nameof(_parameters));

            // Texture Settings
            _useRandomTexture = serializedObject.FindProperty(nameof(_useRandomTexture));
            _texturePropertyName = serializedObject.FindProperty(nameof(_texturePropertyName));
            _textures = serializedObject.FindProperty(nameof(_textures));

            // Offset Settings
            _scrollSpeed = serializedObject.FindProperty(nameof(_scrollSpeed));
            _scrollDirection = serializedObject.FindProperty(nameof(_scrollDirection));
            _scrollCurve = serializedObject.FindProperty(nameof(_scrollCurve));

            // Color Settings
            _startingColor = serializedObject.FindProperty(nameof(_startingColor));
            _endColor = serializedObject.FindProperty(nameof(_endColor));
            _colorCurve = serializedObject.FindProperty(nameof(_colorCurve));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawTimeOptions();
            DrawBaseSettings();

            if (_controlParameters.boolValue)
            {
                DrawPropertySettings();
            }
            if (_controlTexture.boolValue)
            {
                DrawTextureSettings();
            }
            if (_controlOffset.boolValue)
            {
                DrawOffsetSettings();
            }
            if (_controlColor.boolValue)
            {
                DrawColorSettings();
            }

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
            EditorGUILayout.PropertyField(_controlParameters, new GUIContent("Control Parameters", "If checked, this node will animate material float and int parameters."));
            EditorGUILayout.PropertyField(_controlOffset, new GUIContent("Control Offset", "If checked, this node will animate the material's texture offset."));
            EditorGUILayout.PropertyField(_controlColor, new GUIContent("Control Color", "If checked, this node will animate the material's main color."));
            EditorGUILayout.PropertyField(_controlTexture, new GUIContent("Control Texture", "If checked, this node will change the material's texture."));
            EditorGUILayout.EndVertical();
        }

        private void DrawPropertySettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Property Settings");

            _showPropertyListFoldout = EditorGUILayout.Foldout(_showPropertyListFoldout, new GUIContent("Parameters", "A list of material parameters to animate."), true);

            if (_showPropertyListFoldout)
            {
                // Custom list header with Add button
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    _parameters.arraySize++;
                    if (_foldoutStates.Count < _parameters.arraySize)
                    {
                        _foldoutStates.Add(true); // Open the new element by default
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Keep foldout states synchronized with the array size
                while (_foldoutStates.Count > _parameters.arraySize)
                {
                    _foldoutStates.RemoveAt(_foldoutStates.Count - 1);
                }
                while (_foldoutStates.Count < _parameters.arraySize)
                {
                    _foldoutStates.Add(false);
                }

                EditorGUI.indentLevel++;
                for (int i = 0; i < _parameters.arraySize; i++)
                {
                    SerializedProperty element = _parameters.GetArrayElementAtIndex(i);

                    // Header for each element with a Remove button and a foldout
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.BeginHorizontal();

                    string elementName = string.IsNullOrEmpty(element.FindPropertyRelative("Name").stringValue)
                                             ? "New Parameter"
                                             : element.FindPropertyRelative("Name").stringValue;

                    _foldoutStates[i] = EditorGUILayout.Foldout(_foldoutStates[i], new GUIContent(elementName), true);

                    if (GUILayout.Button("-", GUILayout.Width(25)))
                    {
                        _parameters.DeleteArrayElementAtIndex(i);
                        _foldoutStates.RemoveAt(i);
                        break; // Break the loop and let OnInspectorGUI redraw to avoid index issues
                    }
                    EditorGUILayout.EndHorizontal();

                    if (_foldoutStates[i])
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("Type"), new GUIContent("Type", "The type of the material parameter (Float or Int)."));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("Name"), new GUIContent("Name", "The name of the parameter in the material shader."));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("ParameterCurve"), new GUIContent("Parameter Curve", "The curve to apply to the parameter tween."));

                        // Get the type and draw the relevant properties
                        SerializedProperty typeProp = element.FindPropertyRelative("Type");
                        ParameterType type = (ParameterType)typeProp.enumValueIndex;

                        if (type == ParameterType.Float)
                        {
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("FloatStartValue"), new GUIContent("Start Value", "The starting float value for the parameter tween."));
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("FloatEndValue"), new GUIContent("End Value", "The final float value for the parameter tween."));
                        }
                        else if (type == ParameterType.Int)
                        {
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("IntStartValue"), new GUIContent("Start Value", "The starting integer value for the parameter tween."));
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("IntEndValue"), new GUIContent("End Value", "The final integer value for the parameter tween."));
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTextureSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Texture Settings");
            EditorGUILayout.PropertyField(_useRandomTexture, new GUIContent("Use Random Texture", "If checked, a random texture from the list will be chosen. Otherwise, the first one will be used."));
            EditorGUILayout.PropertyField(_texturePropertyName, new GUIContent("Texture Property Name", "The name of the texture property in the material shader (e.g., '_MainTex')."));
            EditorGUILayout.PropertyField(_textures, new GUIContent("Textures", "A list of textures to choose from."), true);
            EditorGUILayout.EndVertical();
        }

        private void DrawOffsetSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Offset Settings");
            EditorGUILayout.PropertyField(_scrollSpeed, new GUIContent("Scroll Speed", "The speed at which the texture offset will change."));
            EditorGUILayout.PropertyField(_scrollDirection, new GUIContent("Scroll Direction", "The direction of the texture offset change."));
            EditorGUILayout.PropertyField(_scrollCurve, new GUIContent("Scroll Curve", "The curve to apply to the texture offset tween."));
            EditorGUILayout.EndVertical();
        }

        private void DrawColorSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Color Settings");
            EditorGUILayout.PropertyField(_startingColor, new GUIContent("Starting Color", "The starting color for the tween."));
            EditorGUILayout.PropertyField(_endColor, new GUIContent("End Color", "The final color for the tween."));
            EditorGUILayout.PropertyField(_colorCurve, new GUIContent("Color Curve", "The curve to apply to the color tween."));
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}