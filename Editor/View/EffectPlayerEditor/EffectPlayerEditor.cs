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

#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using JuiceTwee.Runtime;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EnumerableItem;

namespace JuiceTwee.View.EffectPlayerEditor
{
    /// <summary>
    /// Custom IMGUI editor for the <see cref="EffectPlayer"/> component.
    /// Provides controls for effect tree assignment, auto start options, node configuration, and event management.
    /// </summary>
    [CustomEditor(typeof(EffectPlayer))]
    public class EffectPlayerEditorIMGUI : Editor
    {
        private EffectPlayer _effectPlayer;

        // --- Serialized Properties ---
        private SerializedProperty _effectTreeProp;
        private SerializedProperty _shouldAutoStartProp;
        private SerializedProperty _startUpOptionsProp;
        private SerializedProperty _effectDataListProp;
        private SerializedProperty _onStoppedEventProp;

        // --- Reflection for non-serialized data ---
        private FieldInfo _nodesListFieldInfo;
        private List<EffectData> _nodesList;

        // --- Editor State ---
        private bool[] _showNodeEvents;
        private bool[] _showNodes;
        private List<ExtraItemUpdateData> _extraItemsToUpdate = new();

        /// <summary>
        /// Helper struct for tracking extra item updates in the editor.
        /// </summary>
        private struct ExtraItemUpdateData
        {
            public EffectData EffectData;
            public string Key;
            public UnityEngine.Object UnityObject;
            public System.Object SystemObject;
        }

        /// <summary>
        /// Called when the editor is enabled. Initializes references and serialized properties.
        /// </summary>
        private void OnEnable()
        {
            if (target == null || !(target is EffectPlayer))
            {
                return;
            }

            _effectPlayer = (EffectPlayer)target;

            _effectTreeProp = serializedObject.FindProperty("_effectTree");
            _shouldAutoStartProp = serializedObject.FindProperty("_autoPlay");
            _startUpOptionsProp = serializedObject.FindProperty("_autoStartOn");
            _effectDataListProp = serializedObject.FindProperty("_effectData");
            _onStoppedEventProp = serializedObject.FindProperty("OnStopped");

            _nodesListFieldInfo = typeof(EffectPlayer).GetField("_effectData", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Draws the custom inspector GUI for the EffectPlayer.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_effectTreeProp.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(_effectTreeProp);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            _effectPlayer.UpdateEffectData();

            #region Base Properties

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Control Panel", new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState() { textColor = Color.white }
            });
            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(_effectTreeProp, new GUIContent("Effect Tree"));
            EditorGUILayout.Space();
            if (_effectTreeProp.objectReferenceValue == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.PropertyField(_shouldAutoStartProp, new GUIContent("Auto Start"));
            if (_shouldAutoStartProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_startUpOptionsProp, new GUIContent("Start On"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
            {
                _effectPlayer.Play();
            }
            if (GUILayout.Button("Stop"))
            {
                _effectPlayer.Stop();
            }
            EditorGUILayout.EndHorizontal();
            bool onStopFold = SessionState.GetBool("JuiceTweeShowOnStoppedEventOnPlayer", false);
            onStopFold = EditorGUILayout.Foldout(onStopFold, "Events", true);
            if (onStopFold)
            {
                EditorGUILayout.PropertyField(_onStoppedEventProp, new GUIContent("On Stopped"));
            }
            SessionState.SetBool("JuiceTweeShowOnStoppedEventOnPlayer", onStopFold);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            #endregion


            #region Nodes 

            if (_effectTreeProp.objectReferenceValue != null)
            {
                if (_showNodeEvents == null || _showNodeEvents.Length != _effectDataListProp.arraySize)
                {
                    _showNodeEvents = new bool[_effectDataListProp.arraySize];
                }
                if (_showNodes == null || _showNodes.Length != _effectDataListProp.arraySize)
                {
                    _showNodes = new bool[_effectDataListProp.arraySize];
                    for (int i = 0; i < _effectDataListProp.arraySize; i++)
                    {
                        _showNodes[i] = true;
                    }
                }

                _nodesList = _nodesListFieldInfo.GetValue(_effectPlayer) as List<EffectData>;
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Nodes", new GUIStyle()
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState() { textColor = Color.white }
                });
                if (_nodesList != null)
                {

                    for (int i = 0; i < _effectDataListProp.arraySize; i++)
                    {
                        DrawNode(i);
                    }
                }

                EditorGUILayout.EndVertical();
            }
            #endregion
            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// Draws the inspector UI for a single node in the effect data list.
        /// </summary>
        /// <param name="index">The index of the node to draw.</param>
        private void DrawNode(int index)
        {
            SerializedProperty effectDataProp = _effectDataListProp.GetArrayElementAtIndex(index);
            if (index >= _nodesList.Count)
            {
                return;
            }
            EffectData liveEffectData = _nodesList[index];

            if (liveEffectData == null || liveEffectData.Node == null)
            {
                return;
            }


            _showNodes[index] = EditorGUILayout.Foldout(_showNodes[index], liveEffectData.Node.NodeName, true);
            if (_showNodes[index])
            {

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();

                if (liveEffectData.Node.TargetType != null)
                {
                    EditorGUI.BeginChangeCheck();
                    var targetValue = EditorGUILayout.ObjectField(liveEffectData.TargetName, liveEffectData.Target, liveEffectData.TargetType, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        liveEffectData.Target = targetValue;

                        liveEffectData.Node.SetTarget(targetValue);
                        liveEffectData.Node.OnTargetChangedDuringEditorPlay();

                        EditorUtility.SetDirty(_effectPlayer);
                    }
                }

                DrawEnumerableItems(index, liveEffectData);
                DrawExtraItems(liveEffectData);

                _showNodeEvents[index] = EditorGUILayout.Foldout(_showNodeEvents[index], "Events", true);
                if (_showNodeEvents[index])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(effectDataProp.FindPropertyRelative("OnStarted"));
                    EditorGUILayout.PropertyField(effectDataProp.FindPropertyRelative("OnUpdated"));
                    EditorGUILayout.PropertyField(effectDataProp.FindPropertyRelative("OnCompleted"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);

            }
        }

        /// <summary>
        /// Draws the inspector UI for extra items associated with a node.
        /// </summary>
        /// <param name="liveEffectData">The effect data containing extra items.</param>
        private void DrawExtraItems(EffectData liveEffectData)
        {
            var extraItems = liveEffectData.ExtraItems;
            if (extraItems == null)
            {
                return;
            }

            _extraItemsToUpdate.Clear();
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();

            EditorGUI.indentLevel += 10;
            EditorGUILayout.LabelField("Extra Items", EditorStyles.boldLabel);
            EditorGUI.indentLevel -= 10;

            foreach (var item in extraItems)
            {
                if (item.Type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    EditorGUI.BeginChangeCheck();
                    var objectFieldValue = EditorGUILayout.ObjectField(item.Title, item.UnityObject, item.Type, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _extraItemsToUpdate.Add(new ExtraItemUpdateData()
                        {
                            EffectData = liveEffectData,
                            Key = item.Key,
                            UnityObject = objectFieldValue
                        });
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var stringValue = EditorGUILayout.TextField(item.Title, item.SystemObject?.ToString() ?? string.Empty);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _extraItemsToUpdate.Add(new ExtraItemUpdateData()
                        {
                            EffectData = liveEffectData,
                            Key = item.Key,
                            SystemObject = stringValue
                        });
                    }
                }
            }
            foreach (var item in _extraItemsToUpdate)
            {
                item.EffectData.SerializeExtraItem(item.Key, item.UnityObject, item.SystemObject);
                EditorUtility.SetDirty(_effectPlayer);
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws the inspector UI for enumerable items associated with a node.
        /// </summary>
        /// <param name="index">The index of the node in the effect data list.</param>
        /// <param name="liveEffectData">The effect data containing enumerable items.</param>
        private void DrawEnumerableItems(int index, EffectData liveEffectData)
        {
            List<NodeEnumerableItemData> enumerableItems = liveEffectData.EnumerableItems;
            if (enumerableItems == null || enumerableItems.Count == 0)
            {
                return;
            }

            foreach (var itemData in enumerableItems)
            {
                EditorGUILayout.BeginVertical("box");

                string foldoutKey = $"{index}_{itemData.Title}";
                bool foldout = SessionState.GetBool(foldoutKey, false);
                foldout = EditorGUILayout.Foldout(foldout, itemData.Title, true);
                SessionState.SetBool(foldoutKey, foldout);

                if (foldout)
                {
                    EditorGUI.indentLevel++;

                    List<UnityEngine.Object> updatedList = new(itemData.Items);

                    for (int i = 0; i < updatedList.Count; i++)
                    {
                        EditorGUI.BeginChangeCheck();
                        UnityEngine.Object newObj = EditorGUILayout.ObjectField($"Item {i + 1}", updatedList[i], itemData.Type, true);
                        if (EditorGUI.EndChangeCheck())
                        {
                            updatedList[i] = newObj;
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    UnityEngine.Object addedObj = EditorGUILayout.ObjectField("Add New", null, itemData.Type, true);
                    if (EditorGUI.EndChangeCheck() && addedObj != null && !updatedList.Contains(addedObj))
                    {
                        updatedList.Add(addedObj);
                    }

                    updatedList.RemoveAll(obj => obj == null);
                    liveEffectData.SetEnumerableItems(updatedList);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif