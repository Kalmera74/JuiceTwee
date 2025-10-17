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
using JuiceTwee.Runtime.Attributes;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.RenderNodes
{
    [Serializable]
    [NodeMenu("Render", "Material")]
    public class MaterialNode : EffectNode
    {
        public override Type TargetType => typeof(Material);

        [SerializeField] private float _duration = 1f;
        [SerializeField] private bool _useUnscaledTime = false;

        [SerializeField] private bool _controlParameters;
        [SerializeField] private bool _controlOffset;
        [SerializeField] private bool _controlColor;
        [SerializeField] private bool _controlTexture;

        [SerializeField] private List<MaterialParameter> _parameters;


        [SerializeField] private bool _useRandomTexture;
        [SerializeField] private string _texturePropertyName = "_MainTexture";
        [SerializeField] private List<Texture2D> _textures;


        [SerializeField] private float _scrollSpeed = 1f;
        [SerializeField] private Vector2 _scrollDirection;
        [SerializeField] private AnimationCurve _scrollCurve = AnimationCurve.Linear(0, 0, 1, 1);


        [SerializeField] private Color _startingColor;
        [SerializeField] private Color _endColor;
        [SerializeField] private AnimationCurve _colorCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Material _target => originTarget as Material;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            if (_target == null)
            {
                Debug.LogWarning($"{NodeName}: Target material is null.");
                yield break;
            }

            float startTime = _currentTime;
            float elapsedTime = _currentTime - startTime;

            onStarted?.Invoke();
            if (_controlTexture && _textures.Count > 0)
            {
                int textureIndex = _useRandomTexture ? UnityEngine.Random.Range(0, _textures.Count) : 0;

                if (_textures[textureIndex] != null)
                {
                    _target.mainTexture = _textures[textureIndex];
                    _target.SetTexture(_texturePropertyName, _textures[textureIndex]);
                }
            }

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_controlColor)
                {
                    _target.color = Color.Lerp(_startingColor, _endColor, _colorCurve.Evaluate(t));
                }

                if (_controlOffset)
                {
                    Vector2 offset = _scrollDirection.normalized * (_scrollSpeed * _scrollCurve.Evaluate(t) * elapsedTime);
                    _target.SetTextureOffset(_texturePropertyName, offset);
                }

                if (_controlParameters)
                {
                    foreach (var param in _parameters)
                    {
                        if (string.IsNullOrEmpty(param.Name)) { continue; }
                        float curveT = param.ParameterCurve.Evaluate(t);
                        switch (param.Type)
                        {
                            case ParameterType.Float:
                                float floatValue = Mathf.Lerp(param.FloatStartValue, param.FloatEndValue, curveT);
                                _target.SetFloat(param.Name, floatValue);
                                break;
                            case ParameterType.Int:
                                int intValue = Mathf.RoundToInt(Mathf.Lerp(param.IntStartValue, param.IntEndValue, curveT));
                                _target.SetInt(param.Name, intValue);
                                break;
                        }
                    }
                }

                yield return null;
                onUpdated?.Invoke();
            }

            SetFinalValues();
            onCompleted?.Invoke();
        }

        private void SetFinalValues()
        {
            if (_target == null) return;

            if (_controlColor)
            {
                _target.color = _endColor;
            }

            if (_controlParameters)
            {
                foreach (var param in _parameters)
                {
                    switch (param.Type)
                    {
                        case ParameterType.Float:
                            _target.SetFloat(param.Name, param.FloatEndValue);
                            break;
                        case ParameterType.Int:
                            _target.SetInt(param.Name, param.IntEndValue);
                            break;
                    }
                }
            }
        }
    }

    public enum ParameterType
    {
        Int,
        Float
    }
    [Serializable]
    public struct MaterialParameter
    {
        public ParameterType Type;
        public string Name;
        public AnimationCurve ParameterCurve;
        public int IntStartValue;
        public int IntEndValue;
        public float FloatStartValue;
        public float FloatEndValue;

    }
}