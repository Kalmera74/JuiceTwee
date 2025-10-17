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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace JuiceTwee.CustomNodeEditors
{
    public class ComponentNodeDropdown : AdvancedDropdown
    {
        private List<Type> _monoTypes;
        private Action<Type> _onSelect;

        public ComponentNodeDropdown(AdvancedDropdownState state, Action<Type> onSelect) : base(state)
        {
            _onSelect = onSelect;

            _monoTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(assembly =>
               {
                   Type[] types = null;
                   try { types = assembly.GetTypes(); }
                   catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }
                   return types;
               })
               .Where(t => t != null && t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MonoBehaviour)) || t.IsSubclassOf(typeof(Component)))
               .OrderBy(t => t.Name)
               .ToList();
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("MonoBehaviours");

            foreach (var type in _monoTypes)
            {
                string[] parts = type.FullName.Split('.');
                AdvancedDropdownItem parent = root;

                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var existing = parent.children.FirstOrDefault(c => c.name == parts[i]);
                    if (existing == null)
                    {
                        existing = new AdvancedDropdownItem(parts[i]);
                        parent.AddChild(existing);
                    }
                    parent = existing;
                }

                var leaf = new ComponentNodeDropdownItem(parts.Last(), type) { };
                parent.AddChild(leaf);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            // Check if the selected item is our custom type item
            if (item is ComponentNodeDropdownItem monoBehaviourItem)
            {
                _onSelect?.Invoke(monoBehaviourItem.ComponentType);
            }
            // Optional: handle cases where a parent folder (AdvancedDropdownItem) is clicked
            // else
            // {
            //     Debug.Log($"Selected a folder: {item.name}");
            // }
        }
    }
}