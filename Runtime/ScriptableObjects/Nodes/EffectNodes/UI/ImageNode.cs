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
using JuiceTwee.Runtime.ScriptableObjects.Nodes.ExtraItem;
using UnityEngine;
using UnityEngine.UI;
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.UINodes
{
    [Serializable]
    [NodeMenu("UI", "Image")]

    public class ImageNode : EffectNode, IExtraItemNode

    {

        public override Type TargetType => typeof(Image);


        [SerializeField] protected bool _useUnscaledTime = false;
        [SerializeField] private float _duration = 1f;


        [SerializeField] private bool _controlSprites;
        [SerializeField] private bool _controlColor = true;
        [SerializeField] private bool _controlFillRate = false;

        [SerializeField] private bool _separateChannelCurves = false;
        [SerializeField] private bool _useCurrentAsStart = false;
        [SerializeField] private bool _useTargetsForColor = false;
        [SerializeField] private bool _useGradientInstead = false;


        [SerializeField] private Gradient _gradient;
        [SerializeField] private Color _startColor = Color.white;
        [SerializeField] private Color _endColor = Color.white;



        [SerializeField] private AnimationCurve _blendCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _rCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _gCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _bCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _aCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField, Range(0, 1)] private float _startFillAmount = 0f;
        [SerializeField, Range(0, 1)] private float _endFillAmount = 0f;
        [SerializeField] private AnimationCurve _fillCurve = AnimationCurve.Linear(0, 0, 1, 1);


        [SerializeField] private List<Sprite> _sprites;


        private Image _startColorTarget;
        private Image _endColorTarget;
        private Image _target => originTarget as Image;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;
        private List<NodeExtraItemData> _extraItems;


        #region Extra Targets


        public List<NodeExtraItemData> GetExtraItems()
        {
            if (!_useTargetsForColor)
            {
                _extraItems = null;
                return null;
            }

            if (_extraItems != null)
            {
                return _extraItems;
            }

            _extraItems = new()
        {
            new(nameof(_startColorTarget), "Start Color Target",typeof(Image)),
            new(nameof(_endColorTarget), "End Color Target",typeof(Image)),
        };
            return _extraItems;
        }

        public void SetExtraItem(string key, UnityEngine.Object target)
        {

            if (key.Equals(nameof(_startColorTarget)))
            {
                _startColorTarget = target as Image;
            }
            if (key.Equals(nameof(_endColorTarget)))
            {
                _endColorTarget = target as Image;
            }
        }


        #endregion

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }


        private IEnumerator Operate()
        {
            if (_duration <= 0f)
            {
                _target.color = _endColor;
                yield break;
            }

            Color startColor = _startColor;
            Color endColor = _endColor;


            float startTime = _currentTime;
            int spriteCount = _sprites.Count;
            float timePerSprite = spriteCount > 1 ? _duration / (spriteCount - 1) : _duration;
            int lastSpriteIndex = -1;

            if (_useTargetsForColor)
            {
                startColor = _startColorTarget.color;
                endColor = _endColorTarget.color;
            }


            if (_useCurrentAsStart)
            {
                startColor = _target.color;
            }


            onStarted?.Invoke();

            float elapsedTime = 0;

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_controlFillRate)
                {
                    float fillAmount = Mathf.Lerp(_startFillAmount, _endFillAmount, _fillCurve.Evaluate(t));
                    _target.fillAmount = fillAmount;
                }

                if (_controlColor)
                {
                    Color currentColor;
                    if (!_useGradientInstead)
                    {
                        if (!_separateChannelCurves)
                        {
                            currentColor = Color.Lerp(startColor, endColor, _blendCurve.Evaluate(t));
                        }
                        else
                        {
                            float r = Mathf.Lerp(startColor.r, endColor.r, _rCurve.Evaluate(t));
                            float g = Mathf.Lerp(startColor.g, endColor.g, _gCurve.Evaluate(t));
                            float b = Mathf.Lerp(startColor.b, endColor.b, _bCurve.Evaluate(t));
                            float a = Mathf.Lerp(startColor.a, endColor.a, _aCurve.Evaluate(t));
                            currentColor = new Color(r, g, b, a);
                        }
                    }
                    else
                    {
                        currentColor = _gradient.Evaluate(t);
                    }
                    _target.color = currentColor;
                }


                if (_controlSprites && spriteCount > 0)
                {
                    int spriteIndex = Mathf.FloorToInt((_currentTime - startTime) / timePerSprite);
                    spriteIndex = Mathf.Clamp(spriteIndex, 0, spriteCount - 1);

                    if (spriteIndex != lastSpriteIndex)
                    {
                        _target.sprite = _sprites[spriteIndex];
                        lastSpriteIndex = spriteIndex;
                    }
                }
                onUpdated?.Invoke();
                yield return null;
            }

            if (_controlColor)
            {
                _target.color = endColor;
            }

            if (_controlFillRate)
            {
                _target.fillAmount = _endFillAmount;
            }

            if (_controlSprites && _sprites.Count > 0)
            {
                _target.sprite = _sprites[^1];
            }
            onCompleted?.Invoke();
        }
    }
}