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
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using JuiceTwee.Runtime.Attributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JuiceTwee.Runtime.ScriptableObjects.Nodes.EffectNodes.SceneNodes
{
    [Serializable]
    [NodeMenu("Scene", "Scene Transition")]
    public class SceneNode : EffectNode
    {
#if UNITY_EDITOR
    public override Type TargetType => typeof(SceneAsset);
#else
        public override Type TargetType => null;
#endif

        [SerializeField] private bool _unloadInstead = false;
        [SerializeField] private SceneLoadType _loadType = SceneLoadType.Single;

        [SerializeField, HideInInspector] private string _sceneName;

#if UNITY_EDITOR
    public override void OnTargetChangedDuringEditorPlay()
    {
        base.OnTargetChangedDuringEditorPlay();
        string scenePath = AssetDatabase.GetAssetPath(originTarget);
        _sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
    }
#endif

        public override IEnumerator Perform()
        {
            yield return Operate();
            yield return base.Perform();
        }

        private IEnumerator Operate()
        {
            if (string.IsNullOrEmpty(_sceneName))
            {
                Debug.LogWarning("SceneNode: Scene name is not set.");
                yield break;
            }

            AsyncOperation operation;
            onStarted?.Invoke();

            if (_unloadInstead)
            {
                operation = SceneManager.UnloadSceneAsync(_sceneName);
            }
            else
            {
                var mode = _loadType == SceneLoadType.Single ? LoadSceneMode.Single : LoadSceneMode.Additive;
                operation = SceneManager.LoadSceneAsync(_sceneName, mode);
            }

            if (operation == null)
            {
                Debug.LogWarning($"SceneNode: Operation for scene '{_sceneName}' failed.");
                yield break;
            }

            while (!operation.isDone)
            {
                onUpdated?.Invoke();
                yield return null;
            }

            onCompleted?.Invoke();
        }
    }

    public enum SceneLoadType
    {
        Single,
        Additive
    }
}