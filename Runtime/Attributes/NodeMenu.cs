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
namespace JuiceTwee.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class NodeMenuAttribute : Attribute
    {
        private readonly string _menuParent;
        private readonly string _nodeName;
        private int _order = -1;
        private int _innerOrder = -1;

        public string MenuParent => _menuParent;
        public string NodeName => _nodeName;
        public int Order => _order;
        public int InnerOrder => _innerOrder;
        public NodeMenuAttribute(string menuParent, string nodeName, int order = -1, int innerOrder = -1)
        {
            if (string.IsNullOrEmpty(menuParent))
            {
                throw new ArgumentException("Menu Parent cannot be null or empty.", nameof(menuParent));
            }
            if (string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentException("Node Name cannot be null or empty.", nameof(nodeName));
            }

            _menuParent = menuParent;
            _nodeName = nodeName;
            _order = order;
            _innerOrder = innerOrder;
        }
    }

}