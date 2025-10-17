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
using UnityEditor.IMGUI.Controls;

namespace JuiceTwee.CustomNodeEditors
{
    public class ComponentNodeDropdownItem : AdvancedDropdownItem
    {
        public Type ComponentType { get; private set; }
        public ComponentNodeDropdownItem(string name, Type componentType) : base(name)
        {
            ComponentType = componentType;
        }
    }
}