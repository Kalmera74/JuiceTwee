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
using System;
using System.Collections.Generic;
using System.Linq;
using JuiceTwee.CustomGraphObjects;
using JuiceTwee.Runtime.ScriptableObjects.Nodes;
using JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = JuiceTwee.Runtime.ScriptableObjects.Nodes.Node;

namespace JuiceTwee.View.JuiceTweeView.NodeViews
{
/// <summary>
/// Represents a visual node in the JuiceTweeGraphView, handling UI, ports, and interactions for a graph node.
/// </summary>
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    /// <summary>
    /// Invoked when this node is selected.
    /// </summary>
    public Action<NodeView> OnNodeSelected;
    /// <summary>
    /// Invoked when this node is deselected.
    /// </summary>
    public Action<NodeView> OnNodeDeselected;

    /// <summary>
    /// The underlying data node associated with this view.
    /// </summary>
    public Node Node => _node;
    /// <summary>
    /// The output port of this node view.
    /// </summary>
    public Port Output => _outputPort;
    /// <summary>
    /// The input port of this node view.
    /// </summary>
    public Port Input => _inputPort;

    private Node _node;
    private Port _inputPort;
    private Port _outputPort;
    private Label _title;
    private VisualElement _inputContainer;
    private VisualElement _outputContainer;

    /// <summary>
    /// Constructs a new NodeView for the given node and UI assets.
    /// </summary>
    /// <param name="node">The data node to represent.</param>
    /// <param name="nodeTree">The VisualTreeAsset for the node UI.</param>
    /// <param name="nodeStyle">The StyleSheet for the node UI.</param>
    /// <param name="graphView">The parent graph view.</param>
    public NodeView(Node node, VisualTreeAsset nodeTree, StyleSheet nodeStyle, JuiceTweeGraphView graphView)
    {
        _node = node;

        if (_node is not RootNode)
        {
            _node.OnUpdate += OnNodeValuesChanged;
        }

        SetViewTitle();

        viewDataKey = _node.Id;

        style.left = _node.Position.x;
        style.top = _node.Position.y;


        var nodeView = nodeTree.CloneTree();

        nodeView.styleSheets.Add(nodeStyle);

        _title = nodeView.Q<Label>("title");

        _title.text = string.IsNullOrEmpty(_node.NodeName) ? _node.GetType().Name : _node.NodeName;


        _inputContainer = nodeView.Q<VisualElement>("in");
        _outputContainer = nodeView.Q<VisualElement>("out");



        mainContainer.Clear();
        mainContainer.Add(nodeView);



        CreateInputPorts();
        CreateOutputPorts();

        var listener = new EdgeConnector<Edge>(new JuiceTweeEdgeConnectorListener(graphView));
        _outputPort.AddManipulator(listener);
    }

    /// <summary>
    /// Callback for when the node's values change, updates the view title.
    /// </summary>
    /// <param name="node">The node whose values changed.</param>
    private void OnNodeValuesChanged(Node node)
    {

        SetViewTitle();
    }

    /// <summary>
    /// Activates and displays the duration label on all outgoing edges.
    /// </summary>
    public void ActivateEdgeDuration()
    {
        foreach (var edge in _outputPort.connections)
        {
            var feelEdge = edge as JuiceTweeEdge;
            if (feelEdge == null)
            {
                continue;
            }

            var effectNode = Node as EffectNode;
            if (effectNode == null)
            {
                feelEdge.SetLabelText("");
                continue;
            }

            var field = effectNode.GetType().GetField("_duration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(float))
            {
                float duration = (float)field.GetValue(effectNode);
                if (duration == 0)
                {
                    feelEdge.SetLabelText("Instant");
                    continue;
                }
                feelEdge.SetLabelText(duration.ToString("0.##") + "s");
            }
            else
            {
                feelEdge.SetLabelText("Instant");
            }
            schedule.Execute(() => feelEdge.UpdateDurationLabelPosition()).ExecuteLater(1);
        }
    }

    /// <summary>
    /// Deactivates and hides the duration label on all outgoing edges.
    /// </summary>
    public void DeactivateEdgeDuration()
    {
        foreach (var edge in _outputPort.connections)
        {
            var feelEdge = edge as JuiceTweeEdge;
            if (feelEdge == null)
            {
                continue;
            }
            feelEdge.SetLabelText("");
        }
    }

    /// <summary>
    /// Sets the title of the node view based on the node's name or type.
    /// </summary>
    private void SetViewTitle()
    {
        var t = string.IsNullOrEmpty(_node.NodeName) ? _node.GetType().Name : _node.NodeName;
        if (_title != null)
        {

            _title.text = t;
        }
    }

    /// <summary>
    /// Creates the input port for this node view, if applicable.
    /// </summary>
    private void CreateInputPorts()
    {
        if (_node is RootNode)
        {
            return;
        }

        _inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));

        _inputPort.portName = "";
        _inputPort.portColor = new Color(0f, 1f, 0.4392157f);

        _inputContainer.Add(_inputPort);

    }

    /// <summary>
    /// Creates the output port for this node view.
    /// </summary>
    private void CreateOutputPorts()
    {

        _outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

        _outputPort.portName = "";
        _outputPort.portColor = new Color(0.945098f, 0.3176471f, 0.3882353f);


        _outputContainer.Add(_outputPort);

    }

    /// <summary>
    /// Instantiates a port for this node view.
    /// </summary>
    /// <param name="orientation">The port orientation.</param>
    /// <param name="direction">The port direction.</param>
    /// <param name="capacity">The port capacity.</param>
    /// <param name="type">The port type.</param>
    /// <returns>The created port.</returns>
    public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
    {
        return Port.Create<JuiceTweeEdge>(orientation, direction, capacity, type);
    }

    /// <summary>
    /// Sets the position of the node view and updates the underlying node's position.
    /// </summary>
    /// <param name="newPos">The new position rectangle.</param>
    public override void SetPosition(Rect newPos)
    {



        base.SetPosition(newPos);


        var position = new Vector2(newPos.xMin, newPos.yMin);

        Undo.RecordObject(_node, "Move Node");
        _node.SetPosition(position);
        EditorUtility.SetDirty(_node);
    }

    /// <summary>
    /// Called when the node view is selected.
    /// </summary>
    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    /// <summary>
    /// Called when the node view is deselected.
    /// </summary>
    public override void OnUnselected()
    {

        base.OnUnselected();
        OnNodeDeselected?.Invoke(this);
    }

    /// <summary>
    /// Removes all edges connected to this node view and updates other connected nodes.
    /// </summary>
    public void RemoveEdges()
    {
        var outputConnections = _outputPort.connections.ToList();
        var inputConnections = _inputPort.connections.ToList();

        var otherNodes = new List<NodeView>();


        foreach (var connection in outputConnections)
        {

            otherNodes.Add((NodeView)connection.input.node);
        }

        foreach (var connection in inputConnections)
        {

            otherNodes.Add((NodeView)connection.output.node);
        }

        _outputPort.DisconnectAll();
        _inputPort.DisconnectAll();

        foreach (var connection in outputConnections)
        {

            connection.parent.Remove(connection);
        }

        foreach (var connection in inputConnections)
        {
            connection.parent.Remove(connection);

        }


        foreach (var node in otherNodes)
        {

            node.RemoveEdgesWithNode(this);
        }

        outputConnections.Clear();
        inputConnections.Clear();

    }

    /// <summary>
    /// Removes all edges between this node view and the specified node view.
    /// </summary>
    /// <param name="nodeView">The node view to disconnect from.</param>
    private void RemoveEdgesWithNode(NodeView nodeView)
    {
        if (_outputPort != null)
        {

            var outputConnections = _outputPort.connections.ToList();
            var filteredOutputConnections = outputConnections.Where(x => x.output.node == nodeView || x.input.node == nodeView).ToList();


            foreach (var connection in filteredOutputConnections)
            {
                _outputPort.Disconnect(connection);
            }

        }
        if (_inputPort != null)
        {

            var inputConnections = _inputPort.connections.ToList();
            var filteredInputConnections = inputConnections.Where(x => x.output.node == nodeView || x.input.node == nodeView).ToList();


            foreach (var connection in filteredInputConnections)
            {
                _inputPort.Disconnect(connection);
            }

        }

    }


}
}
#endif