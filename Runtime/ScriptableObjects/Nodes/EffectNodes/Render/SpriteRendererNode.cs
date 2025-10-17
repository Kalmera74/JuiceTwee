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
    [NodeMenu("Render", "Sprite Renderer")]
    public class SpriteRendererNode : EffectNode
    {
        public override Type TargetType => typeof(SpriteRenderer);

        [Header("Time Options")]
        [SerializeField] private float _duration;
        [SerializeField] protected bool _useUnscaledTime = false;

        [Header("Base Settings")]
        [SerializeField] private bool _controlColor;
        [SerializeField] private bool _controlFlip;
        [SerializeField] private bool _controlMask;
        [SerializeField] private bool _controlMaterial;
        [SerializeField] private bool _controlSorting;

        [Header("Color Settings")]
        [SerializeField] private bool _useCurrentColorAsStart;
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _endColor;

        [Header("Flip Settings")]
        [SerializeField] private bool _flipX;
        [SerializeField] private bool _flipY;

        [Header("Mask Settings")]
        [SerializeField] private SpriteMaskInteraction _maskType;

        [Header("Material Settings")]
        [SerializeField] private bool _selectRandomMaterial;
        [SerializeField] private List<Material> _materials;

        [Header("Sorting Settings")]
        [SerializeField] private SortingLayerPicker _sortingLayer;
        [SerializeField] private bool _tweenSortingOrder = false;
        [SerializeField] private int _sortingOrder;
        [SerializeField] private SpriteSortPoint _spriteSortPoint;


        private SpriteRenderer _target => originTarget as SpriteRenderer;
        private int _startingSortingOrder;
        private float _currentTime => _useUnscaledTime ? Time.unscaledTime : Time.time;

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            _startingSortingOrder = _target.sortingOrder;

            onStarted?.Invoke();
            if (_useCurrentColorAsStart)
            {
                _startColor = _target.color;
            }

            if (_controlFlip)
            {
                if (_flipX)
                {
                    _target.flipX = !_target.flipX;
                }
                if (_flipY)
                {
                    _target.flipY = !_target.flipY;
                }
            }

            if (_controlMask)
            {
                _target.maskInteraction = _maskType;
            }

            if (_controlMaterial)
            {
                if (_materials.Count == 0)
                {
                    throw new ArgumentException($"{NodeName}, There is no Sprite Material to use");
                }

                Material spriteMaterial = _materials[0];
                if (_selectRandomMaterial)
                {
                    spriteMaterial = _materials[UnityEngine.Random.Range(0, _materials.Count)];
                }

                _target.material = spriteMaterial;
            }

            if (_controlSorting)
            {
                _target.sortingLayerName = _sortingLayer.Name;
                _target.spriteSortPoint = _spriteSortPoint;
                if (!_tweenSortingOrder)
                {
                    _target.sortingOrder = _sortingOrder;
                }
            }

            float startTime = _currentTime;
            float elapsedTime = _currentTime - startTime;

            while (elapsedTime < _duration)
            {
                elapsedTime = _currentTime - startTime;
                float t = Mathf.Clamp01(elapsedTime / _duration);

                if (_controlColor)
                {
                    Color color = Color.Lerp(_startColor, _endColor, t);
                    _target.color = color;
                }
                if (_tweenSortingOrder)
                {
                    int order = Mathf.RoundToInt(Mathf.Lerp(_startingSortingOrder, _sortingOrder, t));
                    _target.sortingOrder = order;
                }

                yield return null;
                onUpdated?.Invoke();

            }
            onCompleted?.Invoke();
        }
    }
}