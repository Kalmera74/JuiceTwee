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
namespace JuiceTwee.Runtime.ScriptableObjects.Nodes
{

    [Serializable]
    public class RootNode : Node
    {
        protected override void OnValidate()
        {
            _nodeName = GetType().Name;
            base.OnValidate();
        }


    }
}