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
using JuiceTwee.Runtime.ScriptableObjects;
using JuiceTwee.View.JuiceTweeView;
using JuiceTwee.View.JuiceTweeView.NodeViews;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public class JuiceTweeEditorWindow : EditorWindow
{

    [Header("Editor Assets")]
    [SerializeField] private VisualTreeAsset _rootTree;
    [SerializeField] private StyleSheet _rootStyle;

    [Header("Graph Assets")]
    [SerializeField] private StyleSheet _graphStyle;

    [Header("Node Assets")]
    [SerializeField] private VisualTreeAsset _nodeTree;
    [SerializeField] private StyleSheet _nodeStyle;

    protected EffectTree _tree;
    protected JuiceTweeGraphView _graphView;
    protected JuiceTweeInspectorView _inspectorView;
    public static void Open(EffectTree tree)
    {

        JuiceTweeEditorWindow wnd = GetWindow<JuiceTweeEditorWindow>();

        wnd.titleContent = new GUIContent(tree.name);

        wnd.SetTree(tree);
    }

    private void SetTree(EffectTree tree)
    {
        _tree = tree;
        _graphView.Setup(_tree, _graphStyle, _nodeTree, _nodeStyle);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        _rootTree.CloneTree(root);
        root.styleSheets.Add(_rootStyle);

        _graphView = root.Q<JuiceTweeGraphView>();
        _inspectorView = root.Q<JuiceTweeInspectorView>();


        var flowToggle = root.Q<ToolbarToggle>("flow-toggle");
        flowToggle.value = SessionState.GetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, true);
        SessionState.SetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, flowToggle.value);

        var durationToggle = root.Q<ToolbarToggle>("duration-toggle");
        durationToggle.value = SessionState.GetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, true);
        SessionState.SetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, durationToggle.value);


        flowToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
            {
                _graphView.ActivateEdgeFlow();
            }
            else
            {
                _graphView.DeactivateEdgeFlow();
            }
            SessionState.SetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, evt.newValue);
        });

        durationToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
            {
                _graphView.ActiveEdgeDuration();
            }
            else
            {
                _graphView.DeactivateEdgeDuration();
            }
            SessionState.SetBool(SessionStateKeys.JUICE_TWEE_EDGE_FLOW_STATE_KEY, evt.newValue);
        });


        _graphView.OnNodeSelected = OnNodeSelected;

    }


    private void OnNodeSelected(NodeView nodeView)
    {
        _inspectorView.UpdateSelection(nodeView);
    }
}
#endif