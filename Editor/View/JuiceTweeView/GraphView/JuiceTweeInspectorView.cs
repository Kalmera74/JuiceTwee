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
using UnityEngine.UIElements;
using UnityEditor;
using JuiceTwee.View.JuiceTweeView.NodeViews;
public class JuiceTweeInspectorView : VisualElement
{

#pragma warning disable CS0618 // Type or member is obsolete
    public new class UxmlFactory : UxmlFactory<JuiceTweeInspectorView, VisualElement.UxmlTraits> { }

#pragma warning restore CS0618 // Type or member is obsolete
    private Editor _editor;
    public JuiceTweeInspectorView()
    {

    }

    public void UpdateSelection(NodeView nodeView)
    {
        Clear();

        if (nodeView == null)
        {
            return;
        }

        UnityEngine.Object.DestroyImmediate(_editor);

        _editor = Editor.CreateEditor(nodeView.Node);


        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (_editor?.target)
            {
                _editor.OnInspectorGUI();
            }
        });

        Add(container);


    }
}
#endif