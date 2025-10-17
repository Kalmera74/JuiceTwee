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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.ParticleNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(ParticleSystemNode))]
    public class ParticleSystemNodeEditor : Editor
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
        private SerializedProperty _play;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _play = serializedObject.FindProperty(nameof(_play));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawParticleSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawParticleSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Particle Settings");

            GUIContent buttonContent = new GUIContent();
            Color originalColor = GUI.backgroundColor;

            if (_play.boolValue)
            {
                buttonContent.text = "Stop";
                buttonContent.tooltip = "Will be in the playing the particle";
                GUI.backgroundColor = Color.red;
            }
            else
            {
                buttonContent.text = "Play";
                buttonContent.tooltip = "Will be in the stopping the particle";
                GUI.backgroundColor = Color.green;
            }

            if (GUILayout.Button(buttonContent))
            {
                _play.boolValue = !_play.boolValue;
            }

            GUI.backgroundColor = originalColor;

            EditorGUILayout.EndVertical();
        }

        private void DrawHeader(string title)
        {
            EditorGUILayout.LabelField(title, HeaderStyle);
            EditorGUILayout.Separator();
        }
    }
}