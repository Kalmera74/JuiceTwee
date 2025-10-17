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
using UnityEngine;
namespace JuiceTwee.Runtime.Attributes
{

    [Serializable]
    public struct SortingLayerPicker
    {
        public int id;

        public string Name => SortingLayer.IDToName(id);

        public static implicit operator int(SortingLayerPicker layerPicker)
        {
            return layerPicker.id;
        }
    }
}