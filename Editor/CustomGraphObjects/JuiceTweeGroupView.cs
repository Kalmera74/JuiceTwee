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
using UnityEditor.Experimental.GraphView;
namespace JuiceTwee.CustomGraphObjects
{
public class JuiceTweeGroupView : Group
{

    public event Action<GraphElement> OnItemAdded;
    public event Action<GraphElement> OnItemRemoved;
    public event Action<(string OldName, string NewName, JuiceTweeGroupView Group)> OnTitleUpdated;

    public JuiceTweeGroupView() : base()
    {
    }

    protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
    {
        base.OnElementsAdded(elements);
        foreach (var element in elements)
        {
            OnItemAdded?.Invoke(element);
        }
    }

    protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
    {
        base.OnElementsRemoved(elements);

        foreach (var element in elements)
        {
            OnItemRemoved?.Invoke(element);
        }
    }
    protected override void OnGroupRenamed(string oldName, string newName)
    {
        base.OnGroupRenamed(oldName, newName);

        OnTitleUpdated?.Invoke(new(oldName, newName, this));
    }
}
}
#endif