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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.PhysicNodes;


namespace JuiceTwee.CustomNodeEditors
{
    [CustomEditor(typeof(RigidBody3DNode))]
    public class RigidBody3DNodeEditor : Editor
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
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;

        // Base Settings
        private SerializedProperty _controlMass;
        private SerializedProperty _controlDrag;
        private SerializedProperty _controlAngularDrag;
        private SerializedProperty _controlGravity;
        private SerializedProperty _controlIsKinematic;
        private SerializedProperty _controlConstraints;
        private SerializedProperty _controlLayers;

        // Mass Settings
        private SerializedProperty _startingMass;
        private SerializedProperty _endMass;
        private SerializedProperty _useCurrentAsStartingMass;
        private SerializedProperty _useTargetForEndMass;
        private SerializedProperty _massCurve;

        // Drag Settings
        private SerializedProperty _startingDrag;
        private SerializedProperty _endDrag;
        private SerializedProperty _useCurrentAsStartingDrag;
        private SerializedProperty _useTargetForEndDrag;
        private SerializedProperty _dragCurve;

        // Angular Drag Settings
        private SerializedProperty _startingAngularDrag;
        private SerializedProperty _endAngularDrag;
        private SerializedProperty _useCurrentAsStartingAngularDrag;
        private SerializedProperty _useTargetForAngularDrag;
        private SerializedProperty _angularDragCurve;

        // Gravity Settings
        private SerializedProperty _useGravity;

        // Kinematic Settings
        private SerializedProperty _isKinematic;

        // Constraints Settings
        private SerializedProperty _positionConstraints;
        private SerializedProperty _rotationConstraints;

        // Layer Settings
        private SerializedProperty _includeLayers;
        private SerializedProperty _excludeLayers;

        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            // Time Options
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));

            // Base Settings
            _controlMass = serializedObject.FindProperty(nameof(_controlMass));
            _controlDrag = serializedObject.FindProperty(nameof(_controlDrag));
            _controlAngularDrag = serializedObject.FindProperty(nameof(_controlAngularDrag));
            _controlGravity = serializedObject.FindProperty(nameof(_controlGravity));
            _controlIsKinematic = serializedObject.FindProperty(nameof(_controlIsKinematic));
            _controlConstraints = serializedObject.FindProperty(nameof(_controlConstraints));
            _controlLayers = serializedObject.FindProperty(nameof(_controlLayers));

            // Mass Settings
            _startingMass = serializedObject.FindProperty(nameof(_startingMass));
            _endMass = serializedObject.FindProperty(nameof(_endMass));
            _useCurrentAsStartingMass = serializedObject.FindProperty(nameof(_useCurrentAsStartingMass));
            _useTargetForEndMass = serializedObject.FindProperty(nameof(_useTargetForEndMass));
            _massCurve = serializedObject.FindProperty(nameof(_massCurve));

            // Drag Settings
            _startingDrag = serializedObject.FindProperty(nameof(_startingDrag));
            _endDrag = serializedObject.FindProperty(nameof(_endDrag));
            _useCurrentAsStartingDrag = serializedObject.FindProperty(nameof(_useCurrentAsStartingDrag));
            _useTargetForEndDrag = serializedObject.FindProperty(nameof(_useTargetForEndDrag));
            _dragCurve = serializedObject.FindProperty(nameof(_dragCurve));

            // Angular Drag Settings
            _startingAngularDrag = serializedObject.FindProperty(nameof(_startingAngularDrag));
            _endAngularDrag = serializedObject.FindProperty(nameof(_endAngularDrag));
            _useCurrentAsStartingAngularDrag = serializedObject.FindProperty(nameof(_useCurrentAsStartingAngularDrag));
            _useTargetForAngularDrag = serializedObject.FindProperty(nameof(_useTargetForAngularDrag));
            _angularDragCurve = serializedObject.FindProperty(nameof(_angularDragCurve));

            // Gravity Settings
            _useGravity = serializedObject.FindProperty(nameof(_useGravity));

            // Kinematic Settings
            _isKinematic = serializedObject.FindProperty(nameof(_isKinematic));

            // Constraints Settings
            _positionConstraints = serializedObject.FindProperty(nameof(_positionConstraints));
            _rotationConstraints = serializedObject.FindProperty(nameof(_rotationConstraints));

            // Layer Settings
            _includeLayers = serializedObject.FindProperty(nameof(_includeLayers));
            _excludeLayers = serializedObject.FindProperty(nameof(_excludeLayers));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_nodeName, new GUIContent("Node Name", "The name of this node for identification purposes."));
            EditorGUILayout.Separator();

            DrawTimeOptions();
            DrawBaseSettings();
            DrawConditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Time Options");
            EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds over which the tween will complete."));
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_controlMass, new GUIContent("Control Mass", "If checked, this node will tween the Rigidbody's mass."));
            EditorGUILayout.PropertyField(_controlDrag, new GUIContent("Control Drag", "If checked, this node will tween the Rigidbody's drag."));
            EditorGUILayout.PropertyField(_controlAngularDrag, new GUIContent("Control Angular Drag", "If checked, this node will tween the Rigidbody's angular drag."));
            EditorGUILayout.PropertyField(_controlGravity, new GUIContent("Control Gravity", "If checked, this node will set the Rigidbody's useGravity state."));
            EditorGUILayout.PropertyField(_controlIsKinematic, new GUIContent("Control Is Kinematic", "If checked, this node will set the Rigidbody's isKinematic state."));
            EditorGUILayout.PropertyField(_controlConstraints, new GUIContent("Control Constraints", "If checked, this node will set the Rigidbody's constraints."));
            EditorGUILayout.PropertyField(_controlLayers, new GUIContent("Control Layers", "If checked, this node will change the Rigidbody's layer."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            if (_controlMass.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Mass Settings");
                EditorGUILayout.PropertyField(_useCurrentAsStartingMass, new GUIContent("Use Current As Starting", "If checked, the mass tween will begin from the Rigidbody's current mass."));
                EditorGUILayout.PropertyField(_useTargetForEndMass, new GUIContent("Use Target For End", "If checked, the mass tween will end at the specified target's mass value."));

                if (!_useCurrentAsStartingMass.boolValue)
                {
                    EditorGUILayout.PropertyField(_startingMass, new GUIContent("Starting Mass", "The starting mass for the tween."));
                }
                if (!_useTargetForEndMass.boolValue)
                {
                    EditorGUILayout.PropertyField(_endMass, new GUIContent("End Mass", "The final mass for the tween."));
                }
                EditorGUILayout.PropertyField(_massCurve, new GUIContent("Mass Curve", "The curve to apply to the mass tween."));
                EditorGUILayout.EndVertical();
            }

            if (_controlDrag.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Drag Settings");
                EditorGUILayout.PropertyField(_useCurrentAsStartingDrag, new GUIContent("Use Current As Starting", "If checked, the drag tween will begin from the Rigidbody's current drag."));
                EditorGUILayout.PropertyField(_useTargetForEndDrag, new GUIContent("Use Target For End", "If checked, the drag tween will end at the specified target's drag value."));

                if (!_useCurrentAsStartingDrag.boolValue)
                {
                    EditorGUILayout.PropertyField(_startingDrag, new GUIContent("Starting Drag", "The starting drag for the tween."));
                }
                if (!_useTargetForEndDrag.boolValue)
                {
                    EditorGUILayout.PropertyField(_endDrag, new GUIContent("End Drag", "The final drag for the tween."));
                }
                EditorGUILayout.PropertyField(_dragCurve, new GUIContent("Drag Curve", "The curve to apply to the drag tween."));
                EditorGUILayout.EndVertical();
            }

            if (_controlAngularDrag.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Angular Drag Settings");
                EditorGUILayout.PropertyField(_useCurrentAsStartingAngularDrag, new GUIContent("Use Current As Starting", "If checked, the angular drag tween will begin from the Rigidbody's current angular drag."));
                EditorGUILayout.PropertyField(_useTargetForAngularDrag, new GUIContent("Use Target For End", "If checked, the angular drag tween will end at the specified target's angular drag value."));

                if (!_useCurrentAsStartingAngularDrag.boolValue)
                {
                    EditorGUILayout.PropertyField(_startingAngularDrag, new GUIContent("Starting Angular Drag", "The starting angular drag for the tween."));
                }
                if (!_useTargetForAngularDrag.boolValue)
                {
                    EditorGUILayout.PropertyField(_endAngularDrag, new GUIContent("End Angular Drag", "The final angular drag for the tween."));
                }
                EditorGUILayout.PropertyField(_angularDragCurve, new GUIContent("Angular Drag Curve", "The curve to apply to the angular drag tween."));
                EditorGUILayout.EndVertical();
            }

            if (_controlGravity.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Gravity Settings");
                EditorGUILayout.PropertyField(_useGravity, new GUIContent("Use Gravity", "The new state of the Rigidbody's useGravity property."));
                EditorGUILayout.EndVertical();
            }

            if (_controlIsKinematic.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Kinematic Settings");
                EditorGUILayout.PropertyField(_isKinematic, new GUIContent("Is Kinematic", "The new state of the Rigidbody's isKinematic property."));
                EditorGUILayout.EndVertical();
            }

            if (_controlConstraints.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Constraints Settings");
                EditorGUILayout.PropertyField(_positionConstraints, new GUIContent("Position Constraints", "The new constraints to apply to the Rigidbody's position."));
                EditorGUILayout.PropertyField(_rotationConstraints, new GUIContent("Rotation Constraints", "The new constraints to apply to the Rigidbody's rotation."));
                EditorGUILayout.EndVertical();
            }

            if (_controlLayers.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Layer Settings");
                EditorGUILayout.PropertyField(_includeLayers, new GUIContent("Include Layers", "A bitmask of layers to include when setting a new layer."));
                EditorGUILayout.PropertyField(_excludeLayers, new GUIContent("Exclude Layers", "A bitmask of layers to exclude when setting a new layer."));
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