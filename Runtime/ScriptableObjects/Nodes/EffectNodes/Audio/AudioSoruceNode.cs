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
using System.Collections;
using System;
using UnityEngine.Audio;
using System.Collections.Generic;
using JuiceTwee.Runtime.Attributes;


namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AudioNodes
{
[Serializable]
[NodeMenu("Audio", "Audio Source")]
public class AudioSourceNode : EffectNode
{
    public override Type TargetType => typeof(AudioSource);


    [SerializeField] private bool _overrideDuration = false;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private bool _useUnscaledTime = false;


    [SerializeField] private AudioAction _action = AudioAction.Play;
    [SerializeField, Range(0, 1)] private float _startPlayingAtPercentage = 0f;
    [SerializeField] private bool _useMixerGroup = false;
    [SerializeField] private bool _controlPitch = false;
    [SerializeField] private bool _controlVolume = false;
    [SerializeField] private bool _controlPriority = false;
    [SerializeField] private bool _controlStereoPan = false;
    [SerializeField] private bool _controlReverbZoneMix = false;

    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _byPassEffects = false;
    [SerializeField] private bool _byPassListenerEffects = false;
    [SerializeField] private bool _byPassReverbZones = false;
    [SerializeField] private AudioMixerGroup _mixerGroup;


    [SerializeField] private bool _playRandomClip = false;
    [SerializeField] private List<AudioClip> _audioClips;


    [SerializeField] private bool _randomizeVolume = false;
    [SerializeField] private float _radomVolumeChangeDelay = 0.15f;
    [SerializeField, Range(0, 1)] private float _startVolume = 1f;
    [SerializeField, Range(0, 1)] private float _endVolume = 1f;
    [SerializeField] private AnimationCurve _volumeCurve = AnimationCurve.Linear(0, 1, 1, 0);


    [Header("Pitch Settings")]
    [SerializeField] private bool _randomizePitch = false;
    [SerializeField] private float _radomPitchChangeDelay = 0.15f;
    [SerializeField, Range(-3, 3)] private float _startPitch = 1f;
    [SerializeField, Range(-3, 3)] private float _endPitch = 1f;
    [SerializeField] private AnimationCurve _pitchCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Priority Settings")]
    [SerializeField, Range(0, 256)] private int _startingPriority = 128;
    [SerializeField, Range(0, 256)] private int _endPriority = 128;
    [SerializeField] private AnimationCurve _priorityCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Stereo Pan Settings")]
    [SerializeField] private float _startStereoPan = 0f;
    [SerializeField] private float _endStereoPan = 0f;
    [SerializeField] private AnimationCurve _stereoPanCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Reverb Zone Mix Settings")]
    [SerializeField] private float _startReverbZoneMix = 1f;
    [SerializeField] private float _endReverbZoneMix = 1f;
    [SerializeField] private AnimationCurve _reverbZoneMixCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private AudioSource _target => originTarget as AudioSource;
    private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
    private float _lastRandomVolumeChangeTime = -1f;
    private float _lastRandomPitchChangeTime = -1f;


    public override IEnumerator Perform()
    {
        if (_target == null)
        {
            Debug.LogWarning($"{NodeName} Target AudioSource is null. Cannot perform action.");
            yield break;
        }
        if (_target.clip == null)
        {
            Debug.LogWarning($"{NodeName} has no AudioClip assigned.");
            yield break;
        }

        if (_useMixerGroup && _mixerGroup != null)
        {
            _target.outputAudioMixerGroup = _mixerGroup;
        }

        _target.loop = _loop;
        _target.bypassEffects = _byPassEffects;
        _target.bypassListenerEffects = _byPassListenerEffects;
        _target.bypassReverbZones = _byPassReverbZones;

        if (_action == AudioAction.Play || _action == AudioAction.UnPause)
        {
            if (!_target.isPlaying)
            {
                if (_playRandomClip && _audioClips.Count > 0)
                {
                    _target.clip = _audioClips[UnityEngine.Random.Range(0, _audioClips.Count)];
                }

                _target.time = _target.clip.length * _startPlayingAtPercentage;
                _target.Play();
            }
            else if (_action == AudioAction.UnPause)
            {
                _target.UnPause();
            }

            yield return Operate();
        }
        else
        {
            onStarted?.Invoke();
            if (_action == AudioAction.Stop)
            {
                _target.Stop();
            }
            else if (_action == AudioAction.Pause)
            {
                _target.Pause();
            }

            onUpdated?.Invoke();
            onCompleted?.Invoke();
        }


        yield return base.Perform();
    }

    private IEnumerator Operate()
    {
        float startTime = _currentTime;
        float effectDuration = _overrideDuration ? _duration : _target.clip.length;
        float elapsedTime = 0;


        _radomVolumeChangeDelay = Mathf.Min(_radomVolumeChangeDelay, _target.clip.length);
        _radomPitchChangeDelay = Mathf.Min(_radomPitchChangeDelay, _target.clip.length);


        onStarted?.Invoke();

        while (elapsedTime < effectDuration)
        {
            elapsedTime = _currentTime - startTime;
            float t = Mathf.Clamp01(elapsedTime / effectDuration);

            if (_controlVolume)
            {

                float volume;
                if (_randomizeVolume && _currentTime - _lastRandomVolumeChangeTime >= _radomVolumeChangeDelay)
                {
                    volume = UnityEngine.Random.Range(_startVolume, _endVolume);
                    _lastRandomVolumeChangeTime = _currentTime;
                }
                else
                {
                    volume = Mathf.Lerp(_startVolume, _endVolume, _volumeCurve.Evaluate(t));
                }

                _target.volume = volume;
            }

            if (_controlPitch)
            {
                float pitch;
                if (_randomizePitch && _currentTime - _lastRandomPitchChangeTime >= _radomPitchChangeDelay)
                {
                    pitch = UnityEngine.Random.Range(_startPitch, _endPitch);
                    _lastRandomPitchChangeTime = _currentTime;
                }
                else
                {
                    pitch = Mathf.Lerp(_startPitch, _endPitch, _pitchCurve.Evaluate(t));
                }
                _target.pitch = pitch;
            }
            if (_controlPriority)
            {
                int priority = Mathf.RoundToInt(Mathf.Lerp(_startingPriority, _endPriority, _priorityCurve.Evaluate(t)));
                _target.priority = priority;
            }
            if (_controlStereoPan)
            {
                float stereoPan = Mathf.Lerp(_startStereoPan, _endStereoPan, _stereoPanCurve.Evaluate(t));
                _target.panStereo = stereoPan;
            }
            if (_controlReverbZoneMix)
            {
                float reverbZoneMix = Mathf.Lerp(_startReverbZoneMix, _endReverbZoneMix, _reverbZoneMixCurve.Evaluate(t));
                _target.reverbZoneMix = reverbZoneMix;
            }

            yield return null;
            onUpdated?.Invoke();
        }

        _target.Stop();
        onCompleted?.Invoke();

    }
}
    public enum AudioAction
    {
        Play,
        Stop,
        Pause,
        UnPause
    }
}