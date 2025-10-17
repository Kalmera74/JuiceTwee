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
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic; 

namespace JuiceTwee.CustomGraphObjects
{   

public class JuiceTweeEdge : Edge
{
    public bool EnableFlow
    {
        get => _isEnableFlow;
        set
        {
            if (_isEnableFlow == value) return;
            _isEnableFlow = value;

            foreach (var circle in _flowCircles)
            {
                if (_isEnableFlow)
                {
                    Add(circle);
                }
                else
                {
                    Remove(circle);
                }
            }

            if (_isEnableFlow)
            {
                _flowStartTime = EditorApplication.timeSinceStartup;
            }
        }
    }
    private bool _isEnableFlow;

    public float flowSize
    {
        get => _flowSize;
        set
        {
            _flowSize = value;
            var radius = new Length(_flowSize / 2, LengthUnit.Pixel);

            foreach (var circle in _flowCircles)
            {
                circle.style.width = new Length(_flowSize, LengthUnit.Pixel);
                circle.style.height = new Length(_flowSize, LengthUnit.Pixel);
                circle.style.borderTopLeftRadius = radius;
                circle.style.borderTopRightRadius = radius;
                circle.style.borderBottomLeftRadius = radius;
                circle.style.borderBottomRightRadius = radius;
            }
        }
    }
    private float _flowSize = 6f;

    public float flowDuration { get; set; } = 4f; 

    public int minCircles { get; set; } = 1;
    public int maxCircles { get; set; } = 15;
    public float circleDensity { get; set; } = 0.02f; 

    private readonly List<Image> _flowCircles = new List<Image>();

    private Label _labelElement;
    private Color _labelColor = Color.white; 
    private float _labelSize = 12f;          

    public Color LabelColor
    {
        get => _labelColor;
        set
        {
            _labelColor = value;
            if (_labelElement != null)
            {
                _labelElement.style.color = _labelColor;
            }
        }
    }

    public float LabelSize
    {
        get => _labelSize;
        set
        {
            _labelSize = value;
            if (_labelElement != null)
            {
                _labelElement.style.fontSize = _labelSize;
            }
        }
    }

    public JuiceTweeEdge()
    {
        edgeControl.RegisterCallback<GeometryChangedEvent>(OnEdgeControlGeometryChanged);

        _labelElement = new Label
        {
            name = "edge-label",
            style =
            {
                position = Position.Absolute, 
                unityTextAlign = TextAnchor.MiddleCenter, 
                color = LabelColor,
                fontSize = LabelSize,
                // Optional: Add a background or border for better readability against complex backgrounds
                // backgroundColor = new Color(0, 0, 0, 0.5f), // Semi-transparent black background
                // padding = new RectOffset(4, 4, 2, 2),
                // borderTopLeftRadius = 3, borderTopRightRadius = 3,
                // borderBottomLeftRadius = 3, borderBottomRightRadius = 3
            }
        };
        Add(_labelElement);
    }

    public void SetLabelText(string text)
    {
        if (_labelElement != null)
        {
            _labelElement.text = text;
            _labelElement.MarkDirtyRepaint();
            UpdateDurationLabelPosition();
        }
    }

    #region Flow (unchanged)

    private float _totalEdgeLength;
    private double _flowStartTime;

    public void UpdateFlow()
    {
        if (!EnableFlow || _totalEdgeLength <= 0 || flowDuration <= 0 || _flowCircles.Count == 0)
        {
            return;
        }

        var elapsedTime = EditorApplication.timeSinceStartup - _flowStartTime;
        var baseProgress = (elapsedTime / flowDuration) % 1.0;

        int circleCount = _flowCircles.Count;
        float progressSpacing = 1.0f / circleCount;

        for (int i = 0; i < circleCount; i++)
        {
            Image circle = _flowCircles[i];

            float circleProgress = (float)((baseProgress + i * progressSpacing) % 1.0);
            var targetDistance = _totalEdgeLength * circleProgress;

            Vector2 flowPos = FindPositionAlongEdge(targetDistance);

            circle.transform.position = flowPos - Vector2.one * flowSize / 2;

            var startColor = edgeControl.outputColor;
            var endColor = edgeControl.inputColor;
            var flowColor = Color.Lerp(startColor, endColor, circleProgress);
            circle.style.backgroundColor = flowColor;
        }
    }

    private Vector2 FindPositionAlongEdge(float distance)
    {
        var points = edgeControl.controlPoints;
        if (points == null || points.Length < 2) return Vector2.zero;

        float distanceCovered = 0f;

        for (int i = 0; i < points.Length - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];
            var segmentLength = Vector2.Distance(p1, p2);

            if (distanceCovered + segmentLength >= distance)
            {
                float distanceIntoSegment = distance - distanceCovered;
                float segmentProgress = segmentLength > 0 ? distanceIntoSegment / segmentLength : 0;
                return Vector2.Lerp(p1, p2, segmentProgress);
            }
            distanceCovered += segmentLength;
        }

        return points[points.Length - 1];
    }

    private void OnEdgeControlGeometryChanged(GeometryChangedEvent evt)
    {
        // 1. Recalculate total length
        _totalEdgeLength = 0;
        if (edgeControl.controlPoints != null)
        {
            for (int i = 0; i < edgeControl.controlPoints.Length - 1; i++)
            {
                _totalEdgeLength += Vector2.Distance(edgeControl.controlPoints[i], edgeControl.controlPoints[i + 1]);
            }
        }

        UpdateDurationLabelPosition();

        int desiredCount = Mathf.RoundToInt(_totalEdgeLength * circleDensity);
        desiredCount = Mathf.Clamp(desiredCount, minCircles, maxCircles);

        while (_flowCircles.Count > desiredCount)
        {
            Image circleToRemove = _flowCircles[0];
            if (circleToRemove.parent == this)
            {
                Remove(circleToRemove);
            }
            _flowCircles.RemoveAt(0);
        }

        while (_flowCircles.Count < desiredCount)
        {
            var newCircle = new Image
            {
                name = "flow-image",
                style =
                {
                    position = Position.Absolute, // Essential for manual positioning
                    width = new Length(flowSize, LengthUnit.Pixel),
                    height = new Length(flowSize, LengthUnit.Pixel),
                    borderTopLeftRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                    borderTopRightRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                    borderBottomLeftRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                    borderBottomRightRadius = new Length(flowSize / 2, LengthUnit.Pixel),
                }
            };
            _flowCircles.Add(newCircle);
            if (EnableFlow)
            {
                Add(newCircle);
            }
        }

        _flowStartTime = EditorApplication.timeSinceStartup;
    }

    public void UpdateDurationLabelPosition()
    {
        if (_labelElement == null || _totalEdgeLength <= 0)
        {
            return;
        }

        Vector2 labelPos = FindPositionAlongEdge(_totalEdgeLength / 2f);

        var labelRect = _labelElement.layout;

        float verticalOffset = _labelElement.resolvedStyle.fontSize * 0.75f; 

        _labelElement.style.left = labelPos.x - labelRect.width / 2f;
        _labelElement.style.top = labelPos.y - labelRect.height / 2f - verticalOffset; 
    }

    #endregion
}
}
#endif