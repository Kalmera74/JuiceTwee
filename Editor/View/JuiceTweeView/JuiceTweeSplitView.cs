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

using UnityEngine;


public class JuiceTweeSplitView : TwoPaneSplitView
{
#pragma warning disable CS0618 // Type or member is obsolete
    public new class UxmlFactory : UxmlFactory<JuiceTweeSplitView, TwoPaneSplitView.UxmlTraits> { }
#pragma warning restore CS0618 // Type or member is obsolete

}
#endif