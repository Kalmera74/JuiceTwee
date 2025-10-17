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
using System.Collections.Generic;
using System.Linq;
using JuiceTwee.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Audio;

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.AudioNodes
{
[Serializable]
[NodeMenu("Audio", "Audio Mixer")]

public class AudioMixerNode : EffectNode
{
    public override Type TargetType => typeof(AudioMixer);

    [SerializeField] private float _duration = 1;
    [SerializeField] private bool _useUnscaledTime = false;

    [SerializeField] private bool _useParametersInstead = true;

    [SerializeField] private bool _selectRandomSnapshot = false;
    [SerializeField] private bool _transitionAllSnapshots = false;
    [SerializeField] private List<AudioMixerParameter> _parameters;

    [SerializeField] private List<AudioMixerSnapShotParameter> _snapShots;

    private AudioMixer _target => originTarget as AudioMixer;
    private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
    public override IEnumerator Perform()
    {
        yield return Operate();
        yield return base.Perform();
    }

    private IEnumerator Operate()
    {
        float startTime = _currentTime;
        float elapsedTime = 0;

        onStarted?.Invoke();

        if (!_useParametersInstead)
        {
            if (_snapShots.Count == 0)
            {
                Debug.LogError($"{NodeName} No SnapShots found");
                yield break;
            }

            if (!_transitionAllSnapshots)
            {
                AudioMixerSnapShotParameter snapShot = _snapShots[0];
                if (_selectRandomSnapshot)
                {
                    snapShot = _snapShots[UnityEngine.Random.Range(0, _snapShots.Count)];
                }
                snapShot.SnapShot.TransitionTo(_duration);
            }
            else
            {
                var snapShots = _snapShots.Select(s => s.SnapShot).ToArray();
                var weights = _snapShots.Select(w => w.Weight).ToArray();
                _target.TransitionToSnapshots(snapShots, weights, _duration);
            }

            if (_useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(_duration);
            }
            else
            {
                yield return new WaitForSeconds(_duration);
            }
            onUpdated?.Invoke();
        }
        else
        {
            if (_parameters.Count == 0)
            {
                Debug.LogError($"{NodeName} No Parameters found");
                yield break;
            }

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                foreach (var parameter in _parameters)
                {
                    var parameterValue = Mathf.Lerp(parameter.StartingValue, parameter.EndValue, parameter.ParameterCurve.Evaluate(t));
                    _target.SetFloat(parameter.ParameterName, parameterValue);
                }
                yield return null;
                onUpdated?.Invoke();
            }
        }
        onCompleted?.Invoke();
    }
}

[Serializable]
public struct AudioMixerParameter
{
    public string ParameterName;
    public float StartingValue;
    public float EndValue;
    public AnimationCurve ParameterCurve;
}
    [Serializable]
    public struct AudioMixerSnapShotParameter
    {
        public AudioMixerSnapshot SnapShot;
        [Range(0, 1)] public float Weight;
    }
}