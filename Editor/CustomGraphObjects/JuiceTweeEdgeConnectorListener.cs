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
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace JuiceTwee.CustomGraphObjects
{

public class JuiceTweeEdgeConnectorListener : IEdgeConnectorListener
{
    private readonly JuiceTweeGraphView _graphView;

    public JuiceTweeEdgeConnectorListener(JuiceTweeGraphView graphView)
    {
        _graphView = graphView;
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        _graphView.OnEdgeDroppedOutsidePort(edge.output, position);
    }

    public void OnDrop(GraphView graphView, Edge edge) { }
}
}
#endif