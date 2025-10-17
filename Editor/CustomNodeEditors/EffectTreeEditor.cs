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

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using JuiceTwee.Runtime.ScriptableObjects;

namespace JuiceTwee.CustomNodeEditors
{

    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            EffectTree effectTree = EditorUtility.InstanceIDToObject(instanceID) as EffectTree;
            if (effectTree != null)
            {
                JuiceTweeEditorWindow.Open(effectTree);
                return true;
            }
            return false;
        }
    }

    [CustomEditor(typeof(EffectTree))]
    public class EffectTreeEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            if (GUILayout.Button("Open Effect Tree Editor"))
            {

                JuiceTweeEditorWindow.Open((EffectTree)target);
            }
        }
    }
}