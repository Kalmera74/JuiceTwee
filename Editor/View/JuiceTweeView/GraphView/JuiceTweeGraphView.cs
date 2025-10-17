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
using System;
using System.Collections.Generic;
using System.Linq;
using JuiceTwee.View.JuiceTweeView.NodeViews;
using JuiceTwee.Runtime.ScriptableObjects;
using JuiceTwee.CustomGraphObjects;
using JuiceTwee.Runtime.ScriptableObjects.Nodes;
using JuiceTwee.Runtime.Attributes;
using JuiceTwee.View.JuiceTweeView;

/// <summary>
/// The main graph view for the JuiceTwee editor, responsible for displaying, managing, and interacting with nodes, edges, and groups.
/// </summary>
public class JuiceTweeGraphView : GraphView
{
#pragma warning disable CS0618 // Type or member is obsolete
    /// <summary>
    /// UXML factory for creating JuiceTweeGraphView instances.
    /// </summary>
    public new class UxmlFactory : UxmlFactory<JuiceTweeGraphView, UxmlTraits> { }
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Invoked when a node is selected in the graph.
    /// </summary>
    public Action<NodeView> OnNodeSelected;

    private EffectTree _tree;
    private Vector2 _mousePositionOnDown;
    private StyleSheet _graphStyleSheet;
    private VisualTreeAsset _nodeTree;
    private StyleSheet _nodeStyle;

    /// <summary>
    /// Initializes a new instance of the <see cref="JuiceTweeGraphView"/> class.
    /// Sets up the grid, zoom, manipulators, and event callbacks.
    /// </summary>
    public JuiceTweeGraphView()
    {
        Insert(0, new GridBackground());

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        RegisterCallback<MouseDownEvent>(OnMouseDown);

        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        Undo.undoRedoPerformed += OnUndoRedoPerformed;

        EditorApplication.update -= UpdateEdgeFlow;
        EditorApplication.update += UpdateEdgeFlow;
    }

    /// <summary>
    /// Updates the flow animation on all edges.
    /// </summary>
    private void UpdateEdgeFlow()
    {
        foreach (var edge in edges)
        {
            if (edge is JuiceTweeEdge JuiceTweeEdge)
            {
                JuiceTweeEdge.UpdateFlow();
            }
        }
    }

    /// <summary>
    /// Disables the flow animation on all edges.
    /// </summary>
    public void DeactivateEdgeFlow()
    {
        foreach (JuiceTweeEdge edge in edges)
        {
            edge.EnableFlow = false;
        }
    }

    /// <summary>
    /// Enables the flow animation on all edges.
    /// </summary>
    public void ActivateEdgeFlow()
    {
        foreach (Edge edge in edges)
        {
            if (edge is JuiceTweeEdge)
            {
                var feelEdge = edge as JuiceTweeEdge;
                feelEdge.EnableFlow = true;
            }
        }
    }

    /// <summary>
    /// Activates the duration label on all outgoing edges for all nodes.
    /// </summary>
    public void ActiveEdgeDuration()
    {
        foreach (var node in _tree.EffectNodes)
        {
            var nodeView = GetNodeByGuid(node.Id) as NodeView;
            if (nodeView == null)
            {
                continue;
            }
            nodeView.ActivateEdgeDuration();
        }
    }

    /// <summary>
    /// Deactivates the duration label on all outgoing edges for all nodes.
    /// </summary>
    public void DeactivateEdgeDuration()
    {
        foreach (var node in _tree.EffectNodes)
        {
            var nodeView = GetNodeByGuid(node.Id) as NodeView;
            if (nodeView == null)
            {
                continue;
            }
            nodeView.DeactivateEdgeDuration();
        }
    }

    /// <summary>
    /// Handles undo/redo operations by resetting up the graph view and saving assets.
    /// </summary>
    private void OnUndoRedoPerformed()
    {
        if (_tree == null) { return; }
        Setup(_tree, _graphStyleSheet, _nodeTree, _nodeStyle);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Handles mouse down events to track the mouse position for context menus and grouping.
    /// </summary>
    /// <param name="evt">The mouse down event.</param>
    private void OnMouseDown(MouseDownEvent evt)
    {
        _mousePositionOnDown = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
    }

    /// <summary>
    /// Finds the <see cref="NodeView"/> associated with a given data node.
    /// </summary>
    /// <param name="node">The data node.</param>
    /// <returns>The corresponding <see cref="NodeView"/>.</returns>
    private NodeView FindNodeView(JuiceTwee.Runtime.ScriptableObjects.Nodes.Node node)
    {
        return GetNodeByGuid(node.Id) as NodeView;
    }

    /// <summary>
    /// Sets up the graph view with the given tree, styles, and node templates.
    /// </summary>
    /// <param name="tree">The effect tree to display.</param>
    /// <param name="graphStyle">The stylesheet for the graph.</param>
    /// <param name="nodeTree">The visual tree asset for nodes.</param>
    /// <param name="nodeStyle">The stylesheet for nodes.</param>
    public void Setup(EffectTree tree, StyleSheet graphStyle, VisualTreeAsset nodeTree, StyleSheet nodeStyle)
    {
        _tree = tree;

        if (graphStyle != null && graphStyle != _graphStyleSheet)
        {
            _graphStyleSheet = graphStyle;
            styleSheets.Add(graphStyle);
        }

        if (nodeTree != null && nodeTree != _nodeTree)
        {
            _nodeTree = nodeTree;
        }
        if (nodeStyle != null && nodeStyle != _nodeStyle)
        {
            _nodeStyle = nodeStyle;
        }

        _tree.SetRootNode();

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        foreach (var node in _tree.AllNodes)
        {
            CreateNodeView(node);
        }

        foreach (var node in _tree.AllNodes)
        {
            foreach (var child in node.Children)
            {
                var parentView = FindNodeView(node);
                var childView = FindNodeView(child);

                CreateEdge(parentView, childView);
            }
        }

        foreach (var group in _tree.Groups)
        {
            var groupView = CreateGroupView(group.Title, group.Rect);
            foreach (var id in group.NodeIds)
            {
                var nodeView = GetNodeByGuid(id) as NodeView;
                if (nodeView == null)
                {
                    continue;
                }
                groupView.AddElement(nodeView);
            }
        }
    }

    /// <summary>
    /// Creates an edge between two node views and adds it to the graph.
    /// </summary>
    /// <param name="parentView">The parent node view.</param>
    /// <param name="childView">The child node view.</param>
    private void CreateEdge(NodeView parentView, NodeView childView)
    {
        Edge edge = parentView.Output.ConnectTo<JuiceTweeEdge>(childView.Input);
        AddElement(edge);
    }

    /// <summary>
    /// Returns a list of ports compatible with the given start port for edge creation.
    /// </summary>
    /// <param name="startPort">The starting port.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>A list of compatible ports.</returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && startPort.node != endPort.node).ToList();
    }

    /// <summary>
    /// Handles changes to the graph view, such as adding or removing nodes, edges, and groups.
    /// </summary>
    /// <param name="graphViewChange">The graph view change event.</param>
    /// <returns>The modified graph view change.</returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            foreach (var elem in graphViewChange.elementsToRemove)
            {
                if (elem is NodeView nodeView)
                {
                    nodeView.RemoveEdges();
                    _tree.RemoveNode(nodeView.Node);
                }
                else if (elem is JuiceTweeGroupView groupView)
                {
                    _tree.GroupManager.RemoveGroup(groupView.title);
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    var parentNode = edge.output.node as NodeView;
                    var childNode = edge.input.node as NodeView;
                    _tree.RemoveChild(parentNode.Node, childNode.Node);
                }
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                var parentNode = edge.output.node as NodeView;
                var childNode = edge.input.node as NodeView;

                if (parentNode != null && childNode != null)
                {
                    _tree.AddChild(parentNode.Node, childNode.Node);
                }
            }
            schedule.Execute(() => { if (SessionState.GetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, true)) { ActivateEdgeFlow(); } }).ExecuteLater(1);
        }
        return graphViewChange;
    }

    /// <summary>
    /// Deletes the selected nodes and edges from the graph, except for the root node.
    /// </summary>
    /// <returns>The event propagation result.</returns>
    public override EventPropagation DeleteSelection()
    {
        var nodesToDelete = selection.ToList();

        foreach (var node in selection)
        {
            if (node is NodeView nodeView && nodeView.Node is RootNode)
            {
                nodesToDelete.Remove(node);
            }
        }

        var originalSelection = selection.ToList();

        ClearSelection();

        foreach (var node in nodesToDelete)
        {
            AddToSelection(node);
        }

        var result = base.DeleteSelection();

        ClearSelection();
        foreach (var node in originalSelection)
        {
            AddToSelection(node);
        }

        return result;
    }

    /// <summary>
    /// Builds the contextual menu for the graph view, including node creation and grouping options.
    /// </summary>
    /// <param name="evt">The contextual menu event.</param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // base.BuildContextualMenu(evt);

        BuildCreateMenu(evt);
        evt.menu.AppendSeparator();
        BuildGroupingMenu(evt);
    }

    /// <summary>
    /// Builds the grouping menu for the contextual menu.
    /// </summary>
    /// <param name="evt">The contextual menu event.</param>
    private void BuildGroupingMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendSeparator();

        evt.menu.AppendAction("Make Group", action =>
        {
            CreateGroupView("New Group", new Rect(_mousePositionOnDown, new Vector2(200, 200)));
        });

        var selectedGraphElements = selection.OfType<GraphElement>().ToList();
        var selectedNodes = selection.OfType<NodeView>().ToList();
        var selectedGroups = selection.OfType<JuiceTweeGroupView>().ToList();

        bool canGroup = selectedNodes.Any() && !selectedGroups.Any();
        if (canGroup)
        {
            evt.menu.AppendAction("Group Selected", action =>
            {
                GroupSelectedElements(selectedNodes);
            }, DropdownMenuAction.AlwaysEnabled); // Always enabled if conditions met
        }
        else
        {
            evt.menu.AppendAction("Group Selected", action => { }, DropdownMenuAction.Status.Disabled);
        }
    }

    /// <summary>
    /// Builds the node creation menu for the contextual menu.
    /// </summary>
    /// <param name="evt">The contextual menu event.</param>
    private void BuildCreateMenu(ContextualMenuPopulateEvent evt)
    {
        var types = TypeCache.GetTypesWithAttribute<NodeMenuAttribute>();

        var sortedTypes = types
            .Where(type => typeof(JuiceTwee.Runtime.ScriptableObjects.Nodes.Node).IsAssignableFrom(type))
            .Select(type => new
            {
                Type = type,
                Attribute = type.GetCustomAttributes(typeof(NodeMenuAttribute), false)
                                .FirstOrDefault() as NodeMenuAttribute
            })
            .Where(x => x.Attribute != null)
            .OrderBy(x => x.Attribute.Order)
            .ThenBy(x => x.Attribute.InnerOrder)
            .ThenBy(x => $"{x.Attribute.MenuParent}/{x.Attribute.NodeName}");

        foreach (var item in sortedTypes)
        {
            evt.menu.AppendAction($"{item.Attribute.MenuParent}/{item.Attribute.NodeName}", action =>
            {
                CreateNode(item.Type, _mousePositionOnDown);
            });
        }
    }

    /// <summary>
    /// Groups the selected node views into a new group.
    /// </summary>
    /// <param name="nodesToGroup">The list of node views to group.</param>
    private void GroupSelectedElements(List<NodeView> nodesToGroup)
    {
        if (!nodesToGroup.Any()) return;

        Rect groupRect = GetElementsRect(nodesToGroup.Cast<GraphElement>().ToList());

        float padding = 30f;
        groupRect.xMin -= padding;
        groupRect.yMin -= padding;
        groupRect.width += padding * 2;
        groupRect.height += padding * 2;

        var newGroupView = CreateGroupView("New Group", groupRect);

        foreach (var nodeView in nodesToGroup)
        {
            newGroupView.AddElement(nodeView);
            _tree.GroupManager.AddNodeToGroup(newGroupView.title, nodeView.Node);
        }

        ClearSelection();
        AddToSelection(newGroupView);
    }

    /// <summary>
    /// Calculates the bounding rectangle of the given graph elements.
    /// </summary>
    /// <param name="elements">The elements to calculate the bounding rect for.</param>
    /// <returns>The bounding rectangle.</returns>
    private Rect GetElementsRect(List<GraphElement> elements)
    {
        if (!elements.Any())
        {
            return Rect.zero;
        }

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (var element in elements)
        {
            Rect elementRect = element.GetPosition();
            minX = Mathf.Min(minX, elementRect.xMin);
            minY = Mathf.Min(minY, elementRect.yMin);
            maxX = Mathf.Max(maxX, elementRect.xMax);
            maxY = Mathf.Max(maxY, elementRect.yMax);
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    /// <summary>
    /// Creates a new group view in the graph.
    /// </summary>
    /// <param name="groupTitle">The title of the group.</param>
    /// <param name="rect">The position and size of the group.</param>
    /// <returns>The created <see cref="JuiceTweeGroupView"/>.</returns>
    private JuiceTweeGroupView CreateGroupView(string groupTitle, Rect rect)
    {
        var group = new JuiceTweeGroupView
        {
            title = groupTitle,
            name = groupTitle,
        };

        group.SetPosition(rect);
        AddElement(group);
        _tree.GroupManager.AddGroup(group.GetPosition(), group.title);

        group.RegisterCallback<MouseDownEvent>(e =>
        {
            if (e.button == 1)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    _tree.GroupManager.RemoveGroup(group.title);
                    RemoveElement(group);
                });

                menu.AddItem(new GUIContent("Rename"), false, () =>
                {
                    group.FocusTitleTextField();
                });

                menu.ShowAsContext();
            }
        });

        group.OnItemAdded += item =>
        {
            if (item is not NodeView nodeView)
            {
                return;
            }
            _tree.GroupManager.AddNodeToGroup(group.title, nodeView.Node);
        };
        group.OnItemRemoved += item =>
            {
                if (item is not NodeView nodeView)
                {
                    return;
                }

                _tree.GroupManager.RemoveNodeFromGroup(group.title, nodeView.Node);
            };

        group.OnTitleUpdated += e =>
        {
            _tree.GroupManager.UpdateGroupName(e.OldName, e.NewName);
        };

        return group;
    }

    /// <summary>
    /// Creates a new node of the specified type at the given position.
    /// </summary>
    /// <param name="type">The type of node to create.</param>
    /// <param name="position">The position to place the node.</param>
    /// <returns>The created <see cref="NodeView"/>.</returns>
    private NodeView CreateNode(Type type, Vector2 position)
    {
        JuiceTwee.Runtime.ScriptableObjects.Nodes.Node node = _tree.CreateNode(type);

        node.SetPosition(position);

        var nodeView = CreateNodeView(node);

        if (_tree.AllNodes.Count == 2)
        {
            _tree.AddChild(_tree.RootNode, node);

            var rootNode = FindNodeView(_tree.RootNode);
            var childNode = FindNodeView(node);

            CreateEdge(rootNode, childNode);
        }
        return nodeView;
    }

    /// <summary>
    /// Creates a new <see cref="NodeView"/> for the given node and adds it to the graph.
    /// </summary>
    /// <param name="node">The node to create a view for.</param>
    /// <returns>The created <see cref="NodeView"/>.</returns>
    private NodeView CreateNodeView(JuiceTwee.Runtime.ScriptableObjects.Nodes.Node node)
    {
        var nodeView = new NodeView(node, _nodeTree, _nodeStyle, this)
        {
            OnNodeSelected = OnNodeSelected,
        };

        nodeView.OnNodeDeselected += OnNodeDeselected;
        if (nodeView.Node is not RootNode)
        {
            nodeView.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 1)
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete Node"), false, () =>
                    {
                        nodeView.RemoveEdges();
                        _tree.RemoveNode(nodeView.Node);
                        RemoveElement(nodeView);
                    });
                    if (_tree.GroupManager.IsNodeInAGroup(nodeView.Node))
                    {
                        menu.AddItem(new GUIContent("Detach From Group"), false, () =>
                        {
                            var group = _tree.GroupManager.RemoveNodeFromGroup(nodeView.Node);
                            if (group != null)
                            {
                                RemoveElement(nodeView);
                                AddElement(nodeView);

                                Group groupView = GetElementByGuid(group.Id) as Group;
                                if (groupView != null)
                                {
                                    groupView.RemoveElement(nodeView);
                                    groupView.UpdateGeometryFromContent();
                                }
                            }
                        });
                    }
                    menu.ShowAsContext();
                }
            });
        }
        AddElement(nodeView);

        return nodeView;
    }

    /// <summary>
    /// Handles the event when an edge is dropped outside a port, showing a menu to create a new node and connect it.
    /// </summary>
    /// <param name="outputPort">The output port where the edge started.</param>
    /// <param name="position">The position where the edge was dropped.</param>
    public void OnEdgeDroppedOutsidePort(Port outputPort, Vector2 position)
    {
        var menu = new GenericMenu();

        var types = TypeCache.GetTypesWithAttribute<NodeMenuAttribute>()
            .Where(type => typeof(JuiceTwee.Runtime.ScriptableObjects.Nodes.Node).IsAssignableFrom(type))
            .Select(type => new
            {
                Type = type,
                Attribute = type.GetCustomAttributes(typeof(NodeMenuAttribute), false)
                                .FirstOrDefault() as NodeMenuAttribute
            })
            .Where(x => x.Attribute != null)
            .GroupBy(x => x.Attribute.MenuParent)
            .Select(group => new
            {
                MenuParent = group.Key,
                Order = group.First().Attribute.Order,
                Items = group.OrderBy(x => x.Attribute.InnerOrder)
            })
            .OrderBy(g => g.Order);

        foreach (var group in types)
        {
            foreach (var item in group.Items)
            {
                string menuPath = $"{group.MenuParent}/{item.Attribute.NodeName}";
                menu.AddItem(new GUIContent(menuPath), false, () =>
                {
                    NodeView nodeView = outputPort.node as NodeView;

                    var node = CreateNode(item.Type, position);

                    _tree.AddChild(nodeView.Node, node.Node);
                    CreateEdge(nodeView, node);
                    schedule.Execute(() => { if (SessionState.GetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, true)) { ActivateEdgeFlow(); } }).ExecuteLater(1);
                });
            }
        }

        menu.ShowAsContext();
    }

    /// <summary>
    /// Called when a node is deselected, saves the node and activates edge duration if enabled.
    /// </summary>
    /// <param name="view">The deselected node view.</param>
    private void OnNodeDeselected(NodeView view)
    {
        _tree.SaveNode(view.Node);
        if (SessionState.GetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, true))
        {
            view.ActivateEdgeDuration();
        }
    }

    /// <summary>
    /// Saves the tree, updating node names.
    /// </summary>
    public void SaveTree()
    {
        _tree.UpdateNodeNames();
    }
}
#endif