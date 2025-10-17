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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AudioNodes;


namespace JuiceTwee.CustomNodeEditors
{

    [CustomEditor(typeof(AudioSourceNode))]
    public class AudioSourceNodeEditor : Editor
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
        // Node Name
        private SerializedProperty _nodeName;

        // Time Options
        private SerializedProperty _overrideDuration;
        private SerializedProperty _duration;
        private SerializedProperty _useUnscaledTime;

        // Base Settings
        private SerializedProperty _action;
        private SerializedProperty _startPlayingAtPercentage;
        private SerializedProperty _useMixerGroup;
        private SerializedProperty _controlPitch;
        private SerializedProperty _controlVolume;
        private SerializedProperty _controlPriority;
        private SerializedProperty _controlStereoPan;
        private SerializedProperty _controlReverbZoneMix;

        // Mixer Settings
        private SerializedProperty _loop;
        private SerializedProperty _byPassEffects;
        private SerializedProperty _byPassListenerEffects;
        private SerializedProperty _byPassReverbZones;
        private SerializedProperty _mixerGroup;

        // Clip Settings
        private SerializedProperty _playRandomClip;
        private SerializedProperty _audioClips;

        // Volume Settings
        private SerializedProperty _randomizeVolume;
        private SerializedProperty _radomVolumeChangeDelay;
        private SerializedProperty _startVolume;
        private SerializedProperty _endVolume;
        private SerializedProperty _volumeCurve;

        // Pitch Settings
        private SerializedProperty _randomizePitch;
        private SerializedProperty _radomPitchChangeDelay;
        private SerializedProperty _startPitch;
        private SerializedProperty _endPitch;
        private SerializedProperty _pitchCurve;

        // Priority Settings
        private SerializedProperty _startingPriority;
        private SerializedProperty _endPriority;
        private SerializedProperty _priorityCurve;

        // Stereo Pan Settings
        private SerializedProperty _startStereoPan;
        private SerializedProperty _endStereoPan;
        private SerializedProperty _stereoPanCurve;

        // Reverb Zone Mix Settings
        private SerializedProperty _startReverbZoneMix;
        private SerializedProperty _endReverbZoneMix;
        private SerializedProperty _reverbZoneMixCurve;
        #endregion

        private void OnEnable()
        {
            _nodeName = serializedObject.FindProperty(nameof(_nodeName));
            _overrideDuration = serializedObject.FindProperty(nameof(_overrideDuration));
            _duration = serializedObject.FindProperty(nameof(_duration));
            _useUnscaledTime = serializedObject.FindProperty(nameof(_useUnscaledTime));
            _action = serializedObject.FindProperty(nameof(_action));
            _startPlayingAtPercentage = serializedObject.FindProperty(nameof(_startPlayingAtPercentage));
            _useMixerGroup = serializedObject.FindProperty(nameof(_useMixerGroup));
            _controlPitch = serializedObject.FindProperty(nameof(_controlPitch));
            _controlVolume = serializedObject.FindProperty(nameof(_controlVolume));
            _controlPriority = serializedObject.FindProperty(nameof(_controlPriority));
            _controlStereoPan = serializedObject.FindProperty(nameof(_controlStereoPan));
            _controlReverbZoneMix = serializedObject.FindProperty(nameof(_controlReverbZoneMix));
            _loop = serializedObject.FindProperty(nameof(_loop));
            _byPassEffects = serializedObject.FindProperty(nameof(_byPassEffects));
            _byPassListenerEffects = serializedObject.FindProperty(nameof(_byPassListenerEffects));
            _byPassReverbZones = serializedObject.FindProperty(nameof(_byPassReverbZones));
            _mixerGroup = serializedObject.FindProperty(nameof(_mixerGroup));
            _playRandomClip = serializedObject.FindProperty(nameof(_playRandomClip));
            _audioClips = serializedObject.FindProperty(nameof(_audioClips));
            _randomizeVolume = serializedObject.FindProperty(nameof(_randomizeVolume));
            _radomVolumeChangeDelay = serializedObject.FindProperty(nameof(_radomVolumeChangeDelay));
            _startVolume = serializedObject.FindProperty(nameof(_startVolume));
            _endVolume = serializedObject.FindProperty(nameof(_endVolume));
            _volumeCurve = serializedObject.FindProperty(nameof(_volumeCurve));
            _randomizePitch = serializedObject.FindProperty(nameof(_randomizePitch));
            _radomPitchChangeDelay = serializedObject.FindProperty(nameof(_radomPitchChangeDelay));
            _startPitch = serializedObject.FindProperty(nameof(_startPitch));
            _endPitch = serializedObject.FindProperty(nameof(_endPitch));
            _pitchCurve = serializedObject.FindProperty(nameof(_pitchCurve));
            _startingPriority = serializedObject.FindProperty(nameof(_startingPriority));
            _endPriority = serializedObject.FindProperty(nameof(_endPriority));
            _priorityCurve = serializedObject.FindProperty(nameof(_priorityCurve));
            _startStereoPan = serializedObject.FindProperty(nameof(_startStereoPan));
            _endStereoPan = serializedObject.FindProperty(nameof(_endStereoPan));
            _stereoPanCurve = serializedObject.FindProperty(nameof(_stereoPanCurve));
            _startReverbZoneMix = serializedObject.FindProperty(nameof(_startReverbZoneMix));
            _endReverbZoneMix = serializedObject.FindProperty(nameof(_endReverbZoneMix));
            _reverbZoneMixCurve = serializedObject.FindProperty(nameof(_reverbZoneMixCurve));
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
            EditorGUILayout.PropertyField(_overrideDuration, new GUIContent("Override Duration", "If checked, the node's duration will override the clip's length for tweening purposes."));
            if (_overrideDuration.boolValue)
            {
                EditorGUILayout.PropertyField(_duration, new GUIContent("Duration", "The total time in seconds over which the tween will complete."));
            }
            EditorGUILayout.PropertyField(_useUnscaledTime, new GUIContent("Use Unscaled Time", "If checked, the duration will be independent of Time.timeScale."));
            EditorGUILayout.EndVertical();
        }

        private void DrawBaseSettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawHeader("Base Settings");
            EditorGUILayout.PropertyField(_action, new GUIContent("Action", "The action to perform on the AudioSource (e.g., Play, Pause, Stop)."));
            AudioAction action = (AudioAction)_action.enumValueIndex;
            if (action == AudioAction.Play)
            {
                EditorGUILayout.PropertyField(_startPlayingAtPercentage, new GUIContent("Start Playing At %", "The percentage of the clip's length at which to start playing."));
            }
            EditorGUILayout.PropertyField(_useMixerGroup, new GUIContent("Use Mixer Group", "If checked, the audio clip will be routed to a specific audio mixer group."));
            EditorGUILayout.PropertyField(_controlVolume, new GUIContent("Control Volume", "If checked, this node will tween the AudioSource's volume."));
            EditorGUILayout.PropertyField(_controlPitch, new GUIContent("Control Pitch", "If checked, this node will tween the AudioSource's pitch."));
            EditorGUILayout.PropertyField(_controlPriority, new GUIContent("Control Priority", "If checked, this node will tween the AudioSource's priority."));
            EditorGUILayout.PropertyField(_controlStereoPan, new GUIContent("Control Stereo Pan", "If checked, this node will tween the AudioSource's stereo pan."));
            EditorGUILayout.PropertyField(_controlReverbZoneMix, new GUIContent("Control Reverb Zone Mix", "If checked, this node will tween the AudioSource's reverb zone mix."));
            EditorGUILayout.EndVertical();
        }

        private void DrawConditionalSettings()
        {
            AudioAction action = (AudioAction)_action.enumValueIndex;

            if (_useMixerGroup.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Mixer Settings");
                EditorGUILayout.PropertyField(_mixerGroup, new GUIContent("Mixer Group", "The AudioMixerGroup to which the audio will be routed."));
                EditorGUILayout.PropertyField(_loop, new GUIContent("Loop", "If checked, the audio will loop."));
                EditorGUILayout.PropertyField(_byPassEffects, new GUIContent("Bypass Effects", "If checked, the audio will bypass all effects on the mixer."));
                EditorGUILayout.PropertyField(_byPassListenerEffects, new GUIContent("Bypass Listener Effects", "If checked, the audio will bypass effects on the audio listener."));
                EditorGUILayout.PropertyField(_byPassReverbZones, new GUIContent("Bypass Reverb Zones", "If checked, the audio will bypass reverb zones."));
                EditorGUILayout.EndVertical();
            }

            if (action == AudioAction.Play || action == AudioAction.UnPause)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Clip Settings");
                EditorGUILayout.PropertyField(_playRandomClip, new GUIContent("Play Random Clip", "If checked, a random clip from the list will be played."));
                if (_playRandomClip.boolValue)
                {
                    EditorGUILayout.PropertyField(_audioClips, new GUIContent("Audio Clips", "The list of audio clips to choose from."));
                }
                EditorGUILayout.EndVertical();
            }

            if (_controlVolume.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Volume Settings");
                EditorGUILayout.PropertyField(_randomizeVolume, new GUIContent("Randomize Volume", "If checked, the volume will be a random value between min and max."));
                if (_randomizeVolume.boolValue)
                {
                    EditorGUILayout.PropertyField(_radomVolumeChangeDelay, new GUIContent("Random Change Delay", "The delay in seconds between each random volume change."));
                    EditorGUILayout.PropertyField(_startVolume, new GUIContent("Min Volume", "The minimum random volume value."));
                    EditorGUILayout.PropertyField(_endVolume, new GUIContent("Max Volume", "The maximum random volume value."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_startVolume, new GUIContent("Start Volume", "The starting volume for the tween."));
                    EditorGUILayout.PropertyField(_endVolume, new GUIContent("End Volume", "The ending volume for the tween."));
                    EditorGUILayout.PropertyField(_volumeCurve, new GUIContent("Volume Curve", "The curve to apply to the volume tween."));
                }
                EditorGUILayout.EndVertical();
            }

            if (_controlPitch.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Pitch Settings");
                EditorGUILayout.PropertyField(_randomizePitch, new GUIContent("Randomize Pitch", "If checked, the pitch will be a random value between min and max."));
                if (_randomizePitch.boolValue)
                {
                    EditorGUILayout.PropertyField(_radomPitchChangeDelay, new GUIContent("Random Change Delay", "The delay in seconds between each random pitch change."));
                    EditorGUILayout.PropertyField(_startPitch, new GUIContent("Min Pitch", "The minimum random pitch value."));
                    EditorGUILayout.PropertyField(_endPitch, new GUIContent("Max Pitch", "The maximum random pitch value."));
                }
                else
                {
                    EditorGUILayout.PropertyField(_startPitch, new GUIContent("Start Pitch", "The starting pitch for the tween."));
                    EditorGUILayout.PropertyField(_endPitch, new GUIContent("End Pitch", "The ending pitch for the tween."));
                    EditorGUILayout.PropertyField(_pitchCurve, new GUIContent("Pitch Curve", "The curve to apply to the pitch tween."));
                }
                EditorGUILayout.EndVertical();
            }

            if (_controlPriority.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Priority Settings");
                EditorGUILayout.PropertyField(_startingPriority, new GUIContent("Start Priority", "The starting priority for the tween."));
                EditorGUILayout.PropertyField(_endPriority, new GUIContent("End Priority", "The ending priority for the tween."));
                EditorGUILayout.PropertyField(_priorityCurve, new GUIContent("Priority Curve", "The curve to apply to the priority tween."));
                EditorGUILayout.EndVertical();
            }

            if (_controlStereoPan.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Stereo Pan Settings");
                EditorGUILayout.PropertyField(_startStereoPan, new GUIContent("Start Pan", "The starting stereo pan for the tween (-1 is full left, 1 is full right)."));
                EditorGUILayout.PropertyField(_endStereoPan, new GUIContent("End Pan", "The ending stereo pan for the tween."));
                EditorGUILayout.PropertyField(_stereoPanCurve, new GUIContent("Stereo Pan Curve", "The curve to apply to the stereo pan tween."));
                EditorGUILayout.EndVertical();
            }

            if (_controlReverbZoneMix.boolValue)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawHeader("Reverb Zone Mix Settings");
                EditorGUILayout.PropertyField(_startReverbZoneMix, new GUIContent("Start Mix", "The starting reverb zone mix for the tween."));
                EditorGUILayout.PropertyField(_endReverbZoneMix, new GUIContent("End Mix", "The ending reverb zone mix for the tween."));
                EditorGUILayout.PropertyField(_reverbZoneMixCurve, new GUIContent("Reverb Zone Mix Curve", "The curve to apply to the reverb zone mix tween."));
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