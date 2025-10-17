# JuiceTwee Documentation

This document explains the internal design and technical details of **JuiceTwee**, for developers who want to understand or extend the system.

- [Design Overview](#design-overview)
- [Example Flow](#example-flow)
- [Technical Details](#technical-details)
- [Node Types](#node-types)
  - [Flow Nodes](#flow-nodes)
    - [Example Flow Node](#example-flow-node)
  - [Effect Nodes](#effect-nodes)
    - [Example Effect Node](#example-effect-node)
- [Extra Items Node](#extra-items-node)
  - [Example Implementation](#an-example-implementation)
- [Enumerable Items Node](#enumerable-items)
  - [Example Implementation](#an-example-implementation-1)

## Design Overview

**JuiceTwee** is built on Unity’s `ScriptableObject` system to represent **nodes** and **effect trees**, making them highly reusable across different projects.

- **Effect Trees** (ScriptableObjects) store the logic of your flow.
- **EffectPlayer** (MonoBehaviour) handles serialization of targets and controls runtime execution (start, stop, update).
- **Graph Editor** provides a visual workspace where nodes are placed, configured, and connected to define the flow of actions.

Because Unity cannot serialize cross-scene references, targets cannot be stored directly in `ScriptableObjects`. Instead, they are serialized through the `EffectPlayer`, which connects scene objects to nodes at runtime.

---

## Example Flow

1. The **Effect Tree** (ScriptableObject) defines a sequence of nodes.
2. An **EffectPlayer** (MonoBehaviour) references the tree and assigns scene targets.
3. Execution begins at the **RootNode** and flows through connected children.
4. **FlowNodes** manage control logic (e.g., delays, counters).
5. **EffectNodes** apply actual effects (tweening, component manipulation, engine changes).
6. Events (`OnStart`, `OnUpdate`, `OnComplete`) are triggered for hooking into gameplay code.

---

## Extending JuiceTwee

To create your own custom nodes:

1. Inherit from `EffectNode` or `FlowNode`.
2. Override `TargetType` if your node works with a specific Unity object type.
3. Implement `Perform()` to define behavior.
4. Optionally override `InitializeNodeBeforeRunning` for setup logic.
5. Add editor attributes for custom inspector fields if needed.

---

## Technical Details

JuiceTwee is designed to be **flexible** and **extensible**, allowing developers to easily add new nodes or extend functionality.

The system is composed of a few core elements:

### EffectTree (ScriptableObject)

- Holds all nodes and their relationships.
- Defines reusable logic for effects and tweening sequences.
- Can be reused across multiple scenes and objects.

### EffectPlayer (MonoBehaviour)

- Executes an `EffectTree` in the scene.
- Serializes node targets (e.g., Transforms, AudioSources) that cannot be stored in ScriptableObjects.
- Provides UnityEvents for `OnStart`, `OnUpdate`, and `OnComplete` callbacks.
- Decouples logic from scene-specific objects, enabling reuse and flexibility.

### Nodes

Nodes are the building blocks of JuiceTwee. They are represented as `ScriptableObject`s and handle both logic and flow control.

All nodes inherit from the base class `Node`, which provides:

- **Children Management** → handles flow between nodes.
- **Coroutine Execution** → runs tweens and animations.
- **Player Context** → nodes are executed through an `EffectPlayer` for coroutine handling.

---

## Node Types

### Flow Nodes

Flow Nodes handle **graph flow control** but do not operate on scene objects.

- Inherit from `FlowNode`.
- Not exposed in the `EffectPlayer` inspector, since they don’t require serialized targets.
- Provide structure and logic (delays, counters, branching).
- Currently minimal (no unique properties), but categorized separately so the editor can:

  - Hide them from the player inspector.
  - Provide a base for future shared flow-specific functionality.

#### Example Flow Node

Here is a simple delay node that delays the activation of its children for a given time

```csharp
using UnityEngine;
namespace JuiceTwee
{
    public class DelayNode : FlowNode
    {
        [SerializeField] private float _delay;

        public override IEnumerator Perform()
        {
            yield return new WaitForSeconds(_delay);
            yield return base.Perform();
        }

     }
 }
```

---

### Effect Nodes

Effect Nodes are the **primary control points** of JuiceTwee. They perform operations such as tweens, component manipulations, or global engine effects.

All effect nodes inherit from **`EffectNode`**.

#### Key Properties

- **`TargetType` (abstract)**

  - Defines the type of target the node requires (e.g., `Transform`, `AudioSource`).
  - Used by the editor to render the correct field in the inspector.
  - If overridden to return `null`, the editor will not draw a target field.
    Example: `FogNode` has no target.

- **`TargetName` (virtual, default = `"Target"`)**

  - Defines the display name of the target field.
  - Override this if your node’s semantics benefit from a more descriptive name (e.g., `"Audio Source"`).

- **`originTarget` (runtime reference)**

  - Holds the **actual assigned target** during runtime.
  - Declared as `UnityEngine.Object`, so it can point to any Unity object type.
  - Must be **cast to the expected subtype** before use.
  - Is also assigned in the Editor, but not serialized by the ScriptableObject.
  - Useful both in Editor (for preview/config) and at runtime (for execution).

- **UnityEvents**
  Each EffectNode exposes events you can hook into from the `EffectPlayer`:

  - `OnStart` → Triggered when the node begins execution.
  - `OnUpdate` → Triggered during execution.
  - `OnComplete` → Triggered when execution finishes.

- **`InitializeNodeBeforeRunning()`**

  - Called before `Perform()` begins.
  - Useful for setup, default values, or preparing references.

- **`OnTargetChangedDuringEditorPlay()`**

  - Invoked when the node’s target changes during editor play mode.
  - At this point, `originTarget` is already updated.
  - Can be used for:

    - Resetting default values.
    - Auto-configuring properties based on the target.
    - Performing calculations tied to the new target.

#### Example Effect Node

Here’s a simple example of an Effect Node that moves a Transform upwards over time:

```csharp
using System.Collections;
using UnityEngine;

namespace JuiceTwee
{
    public class PositionNode : EffectNode
    {
        public override System.Type TargetType => typeof(Transform);

        protected override IEnumerator Perform(EffectPlayer player)
        {
            Transform t = (Transform)originTarget;
            Vector3 start = t.position;
            Vector3 end = start + Vector3.up * 2;
            float duration = 1f;
            float elapsed = 0f;

            onStarted?.Invoke();
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                t.position = Vector3.Lerp(start, end, elapsed / duration);
                yield return null;
                onUpdated?.Invoke();
            }
            onCompleted?.Invoke();
        }
    }
}
```

This node:

- Restricts the target type to `Transform`.
- Animates the position over one second.
- Runs as part of the `EffectTree` coroutine system.

---

## Extra Items Node

Sometimes you may want to have additional scene targets for various need eg. reference an object for its values or simply want to effect more than one target. In that case you need to add `IExtraTargetNode` to your custom `EffectNode` to be able to expose new fields in the editor.

The interface has two method one is to provide the extra items to the editor and the other for editor to set them. In the Editor mode editor will query the nodes for extra items and draw the fields and serialized them. At run time it will call the set method with the serialized object

### IExtraItemNode:

```csharp
namespace JuiceTwee
{
    interface IExtraItemNode
    {
        public List<NodeExtraItemData> GetExtraItems();
        public void SetExtraItem(string key, UnityEngine.Object unityObject);
    }
```

### An example implementation:

```csharp

        private Image _startColorTarget;
        private Image _endColorTarget;
        private List<NodeExtraItemData> _extraItems;

        public List<NodeExtraItemData> GetExtraItems()
        {
            if (!_useTargetsForColor)
            {
                _extraItems = null;
                return null;
            }

            if (_extraItems != null)
            {
                return _extraItems;
            }

            _extraItems = new()
        {
            new(nameof(_startColorTarget), "Start Color Target",typeof(Image)),
            new(nameof(_endColorTarget), "End Color Target",typeof(Image)),
        };
            return _extraItems;
        }

        public void SetExtraItem(string key, UnityEngine.Object target)
        {

            if (key.Equals(nameof(_startColorTarget)))
            {
                _startColorTarget = target as Image;
            }
            if (key.Equals(nameof(_endColorTarget)))
            {
                _endColorTarget = target as Image;
            }
        }

```

## Enumerable Items

Enumerable items are similar to the extra items it basically provide the same functionality with one difference. It allows you to have a dynamic list of extra items without needing to explicitly specify them.

Enumerable items wil be drawn as a list and items can be added or removed from the inspector. To have this functionality in your custom nodes just implement `IEnumerableItemNode` interface. The rest is similar to the extra items implementation.

### IEnumerableItemNode:

```csharp
namespace JuiceTwee
{
   interface IEnumerableItemNode
    {
       public List<NodeEnumerableItemData> GetEnumerableItems();
       public void SetEnumerableItems(List<UnityEngine.Object> items);
    }
}
```

### An example implementation:

```csharp
    List<Transform> _spawnPoints = new();

    public override NodeEnumerableItemData GetEnumerableItems()
    {
        return new NodeEnumerableItemData("Spawn Points", typeof(Transform));
    }
    public override void SetEnumerableItems(List<UnityEngine.Object> items)
    {
        _spawnPoints.Clear();

        foreach (var item in items)
        {
            if(item is not Transform)
            {
                continue;
            }
            var spawnPoint = item as Transform;
            _spawnPoints.Add(spawnPoint);
        }
    }

```
