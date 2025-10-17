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

using System;
using System.Collections;
using JuiceTwee.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Audio;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AudioNodes
{
    [Serializable]
    [NodeMenu("Audio", "Clip Player")]
    public class ClipPlayNode : EffectNode
    {
        public override Type TargetType => typeof(AudioClip);

        [SerializeField] private bool _useCustomDuration = false;
        [SerializeField] private float _duration = 1f;
        [SerializeField] protected bool _useUnscaledTime = false;


        [SerializeField] private bool _controlVolume = true;
        [SerializeField] private bool _controlPitch = false;
        [SerializeField] private bool _useMixerGroup = false;
        [SerializeField] private bool _autoDestroyAudioSource = true;

        [SerializeField] private AudioMixerGroup _mixerGroup;


        [SerializeField] private bool _randomizeVolume = false;
        [SerializeField, Range(0, 1)] private float _startVolume = 1f;
        [SerializeField, Range(0, 1)] private float _endVolume = 1f;
        [SerializeField] private AnimationCurve _volumeCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [SerializeField] private bool _randomizePitch = false;
        [SerializeField, Range(0, 1)] private float _startPitch = 1f;
        [SerializeField, Range(0, 1)] private float _endPitch = 1f;
        [SerializeField] private AnimationCurve _pitchCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private AudioSource _audioSource;
        private AudioClip _target => originTarget as AudioClip;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;



        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }
        private IEnumerator Operate()
        {
            if (originTarget == null)
            {
                Debug.LogWarning($"{NodeName} Target AudioClip is null. Cannot play clip.");
                yield break;
            }

            if (_audioSource == null)
            {
                var sourceGo = new GameObject($"{NodeName}_AudioSource_{_target.name}");
                _audioSource = sourceGo.AddComponent<AudioSource>();
            }
            _audioSource.clip = _target;

            if (_useMixerGroup && _mixerGroup != null)
            {
                _audioSource.outputAudioMixerGroup = _mixerGroup;

            }

            float effectDuration = _useCustomDuration ? _duration : _target.length;

            onStarted?.Invoke();
            _audioSource.Play();

            float startTime = _currentTime;
            float elapsedTime = _currentTime - startTime;
            float randomVolume = _randomizeVolume ? UnityEngine.Random.Range(_startVolume, _endVolume) : _startVolume;
            float randomPitch = _randomizePitch ? UnityEngine.Random.Range(_startPitch, _endPitch) : _startPitch;
            while (elapsedTime < effectDuration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / effectDuration);

                if (_controlVolume)
                {
                    float volume = _randomizeVolume ? randomVolume : Mathf.Lerp(_startVolume, _endVolume, _volumeCurve.Evaluate(t));
                    _audioSource.volume = volume;
                }

                if (_controlPitch)
                {
                    float pitch = _randomizePitch ? randomPitch : Mathf.Lerp(_startPitch, _endPitch, _pitchCurve.Evaluate(t));
                    _audioSource.pitch = pitch;
                }

                onUpdated?.Invoke();
                yield return null;
            }

            _audioSource.Stop();
            onCompleted?.Invoke();

            if (_autoDestroyAudioSource && _audioSource != null)
            {
                Destroy(_audioSource.gameObject);
                _audioSource = null;
            }
        }
    }
}