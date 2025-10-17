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
using TMPro;
using UnityEngine;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.UINodes
{
    [Serializable]
    [NodeMenu("UI", "TextMeshProUGUI")]
    public class TextMeshProUGUINode : EffectNode
    {
        public override Type TargetType => typeof(TextMeshProUGUI);

        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;

        [SerializeField] private bool _controlText = true;
        [SerializeField] private bool _controlColor = false;
        [SerializeField] private bool _controlFontSize = false;

        [SerializeField, Multiline] private string _text;
        [SerializeField] private bool _useTypewriterEffect = false;

        [SerializeField] private bool _useCurrentFontSizeAsStart = true;
        [SerializeField] private float _startFontSize = 14f;
        [SerializeField] private float _endFontSize = 14f;
        [SerializeField] private AnimationCurve _fontSizeCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool _useCurrentColorAsStart = true;
        [SerializeField] private bool _useGradientInstead = false;
        [SerializeField] private AnimationCurve _colorBlendCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private Color _startColor = Color.white;
        [SerializeField] private Color _endColor = Color.white;

        [SerializeField] private VertexGradient _startGradient;
        [SerializeField] private VertexGradient _endGradient;

        private TextMeshProUGUI _target => originTarget as TextMeshProUGUI;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        public override IEnumerator Perform()
        {
            if (_controlText && _useTypewriterEffect)
            {
                _player.StartCoroutine(AnimateTextCharacters());
            }

            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            float startTime = _currentTime;

            if (_controlFontSize)
            {
                if (_useCurrentFontSizeAsStart)
                {
                    _startFontSize = _target.fontSize;
                }
                _target.fontSize = _startFontSize;
            }

            if (_useCurrentColorAsStart)
            {
                _startColor = _target.color;
                _startGradient = _target.colorGradient;
            }
            if (_useGradientInstead)
            {
                _target.enableVertexGradient = true;
            }

            onStarted?.Invoke();

            float elapsedTime = 0;
            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_controlFontSize)
                {
                    float currentTextSize = Mathf.Lerp(_startFontSize, _endFontSize, _fontSizeCurve.Evaluate(t));
                    _target.fontSize = currentTextSize;
                }

                if (_controlColor)
                {
                    float curveT = _colorBlendCurve.Evaluate(t);

                    if (_useGradientInstead)
                    {
                        Color topLeft = Color.Lerp(_startGradient.topLeft, _endGradient.topLeft, curveT);
                        Color topRight = Color.Lerp(_startGradient.topRight, _endGradient.topRight, curveT);
                        Color bottomLeft = Color.Lerp(_startGradient.bottomLeft, _endGradient.bottomLeft, curveT);
                        Color bottomRight = Color.Lerp(_startGradient.bottomRight, _endGradient.bottomRight, curveT);

                        _target.colorGradient = new VertexGradient(topLeft, topRight, bottomLeft, bottomRight);
                    }
                    else
                    {
                        _target.color = Color.Lerp(_startColor, _endColor, curveT);
                    }
                }

                onUpdated?.Invoke();
                yield return null;
            }

            if (_controlFontSize)
            {
                _target.fontSize = _endFontSize;
            }

            if (_controlColor)
            {
                if (_useGradientInstead)
                {
                    _target.colorGradient = _endGradient;
                }
                else
                {
                    _target.color = _endColor;
                }
            }

            if (_controlText && !_useTypewriterEffect)
            {
                _target.text = _text;
            }

            onCompleted?.Invoke();
        }

        private IEnumerator AnimateTextCharacters()
        {
            _target.text = "";
            if (string.IsNullOrEmpty(_text))
            {
                yield break;
            }

            int visibleCharCount = 0;
            bool inTag = false;
            foreach (char c in _text)
            {
                if (c == '<')
                {
                    inTag = true;
                }

                if (!inTag)
                {
                    visibleCharCount++;
                }

                if (c == '>')
                {
                    inTag = false;
                }
            }

            if (_duration <= 0 || visibleCharCount == 0)
            {
                _target.text = _text;
                yield break;
            }

            float timePerChar = _duration / visibleCharCount;
            WaitForSeconds delay = new WaitForSeconds(timePerChar);

            var stringBuilder = new System.Text.StringBuilder();
            int i = 0;
            while (i < _text.Length)
            {
                if (_text[i] == '<')
                {
                    int endIndex = _text.IndexOf('>', i);
                    if (endIndex == -1)
                    {
                        stringBuilder.Append(_text[i]);
                        _target.text = stringBuilder.ToString();
                        yield return delay;
                        i++;
                    }
                    else
                    {
                        stringBuilder.Append(_text.Substring(i, endIndex - i + 1));
                        _target.text = stringBuilder.ToString();
                        i = endIndex + 1;
                    }
                }
                else
                {
                    stringBuilder.Append(_text[i]);
                    _target.text = stringBuilder.ToString();
                    yield return delay;
                    i++;
                }
            }

            _target.text = _text;
        }

    }
}