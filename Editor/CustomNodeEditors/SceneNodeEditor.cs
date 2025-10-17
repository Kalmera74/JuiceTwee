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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.SceneNodes;

namespace JuiceTwee.CustomNodeEditors
{
    [CustomEditor(typeof(SceneNode))]
    public class SceneNodeEditor : Editor
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
        private SerializedProperty _unloadInstead;
        private SerializedProperty _loadType;
        private SerializedProperty _sceneName;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _unloadInstead = serializedObject.FindProperty(nameof(_unloadInstead));
            _loadType = serializedObject.FindProperty(nameof(_loadType));
            _sceneName = serializedObject.FindProperty("_sceneName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawSceneSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Scene Settings");


            EditorGUILayout.PropertyField(_unloadInstead, new GUIContent("Unload Instead", "If checked, this node will unload the specified scene instead of loading it."));

            if (!_unloadInstead.boolValue)
            {
                EditorGUILayout.PropertyField(_loadType, new GUIContent("Load Type", "How the scene will be loaded (e.g., Single, Additive)."));
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