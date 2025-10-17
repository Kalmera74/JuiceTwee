![Unity](https://img.shields.io/badge/Unity-6000+-000?logo=unity)
![GitHub release](https://img.shields.io/github/v/release/Kalmera74/JuiceTwee)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

<p align="center">
  <img src="https://github.com/Kalmera74/JuiceTwee/blob/master/Documentation~/JuiceTwee.jpg?raw=true" alt="JuiceTwee icon"/>
</p>


# JuiceTwee - Game Feel & Tweening for Unity

**JuiceTwee** is a coroutine based flexible and powerful, node-based Unity graph tool for creating sophisticated **game feel** and **tweening** effects. It provides a simple and intuitive API for building smooth, dynamic animations and a variety of built-in transitions. JuiceTwee is designed to help developers add **polish, responsiveness, and life** to their games without writing complex animation code.

---

## Table of Contents

- [Features](https://www.google.com/search?q=%23features)
- [Requirements](https://www.google.com/search?q=%23requirements)
- [Installation](https://www.google.com/search?q=%23installation)
- [How to Use](https://www.google.com/search?q=%23how-to-use)
- [Available Nodes](https://www.google.com/search?q=%23available-nodes)
- [Contributing](https://www.google.com/search?q=%23contributing)
- [License](https://www.google.com/search?q=%23license)

---

## Features

- **Visual Node-Based Editor:** Design and manage complex animation and effect sequences in an intuitive graph editor.
- **Extensive Built-in Support:** A wide range of nodes for controlling core Unity systems:
  - **Transform:** Animate position, rotation, and scale. Includes dedicated nodes for squash & stretch and bounce effects.
  - **Audio:** Control `AudioSource`, `AudioMixer` parameters, and play clips.
  - **Camera:** Create camera shakes and control camera properties.
  - **GameObject:** Handle activation/deactivation, instantiation, destruction, and component manipulation.
  - **UI:** Animate `CanvasGroup`, `Image`, `RectTransform`, and `TextMeshProUGUI` elements.
  - **Rendering:** Manipulate `Material` properties, `Fog`, `Skybox`, and `SpriteRenderer`.
  - **Physics:** Interact with `Collider` and `Rigidbody` components.
  - **Particles:** Control `ParticleSystem` playback.
  - **Animation:** Interface with the `Animator` component.
  - **Scene Management:** Load and manage scenes.
  - **Time & Flow Control:** Create delays, freeze frames, and manage game speed with `Time.timeScale`. Use counter nodes to manage execution flow.
- **Modular & Decoupled:** Runtime logic is separated from the effect definitions, allowing you to reuse effect trees on different targets.
- **Easily Extensible:** Create custom nodes to meet your project's specific needs.
- **Configurable Easing:** Use easing curves to achieve smooth and natural animations.
- **Undo/Redo**: Graph editor has full undo/redo support
- **Coroutine-Based Execution:** Supports both scaled and unscaled time for animations that are independent of `Time.timeScale`.

---

## Requirements

- **Unity 6** or later
- **TextMesh Pro** package (installed automatically if missing)
- Compatible with **URP, HDRP, and the Built-in Render Pipeline**

---

## Installation

### Via Git URL

1.  In your Unity project, open the Package Manager (`Window > Package Manager`).
2.  Click the `+` icon in the top-left corner and select "**Add package from git URL...**".
3.  Enter the following URL and click **Add**:

<!-- end list -->

```
https://github.com/Kalmera74/JuiceTwee.git
```

Alternatively, you can manually edit your project’s `Packages/manifest.json` file and add this line to the `dependencies` block:

```json
{
  "dependencies": {
    "com.kalmera74.JuiceTwee": "https://github.com/Kalmera74/JuiceTwee.git"
  }
}
```

---

## How to Use

The JuiceTwee system is composed of three main parts: the **JuiceTwee Asset**, the **EffectPlayer Component**, and the **Graph Editor**.

### 1\. EffectTree Asset

This is a `ScriptableObject` that holds all the nodes and their relationships, defining the reusable logic for an effect sequence.

- **To create one**, right-click in a project folder and select **Create \> JuiceTwee**.
- After it is created, double-click the asset to open the **Graph Editor**.
- Once configured, a JuiceTwee asset can be used on any `EffectPlayer` by simply dragging and dropping it into the appropriate field.

### 2\. EffectPlayer Component

This `MonoBehaviour` is the component that executes a JuiceTwee asset in the scene. It controls the serialization of targets and the start/stop of the tree's flow. This design separates the tween logic from its targets, so they can be changed with ease.

- The `EffectPlayer`'s Inspector will display all **EffectNodes** from the assigned tree so that you can assign their targets (e.g., a specific Transform or AudioSource).
- Nodes that do not require a target (like `FogNode`) will still be listed but will not have a target field.
- The `EffectPlayer` also provides comprehensive **UnityEvents** for the start/stop of the whole tree, and for the start/stop/update/completed states for each individual node you wish to hook into.

### 3\. Graph Editor

The Graph Editor is the visual workspace where you design your reusable flow of tweening nodes.

- Execution begins at the **RootNode** and flows from parent to children. Sibling nodes (nodes at the same level) are executed at the same time.
- Use the context menu (right-click) to select from all available nodes and connect them together. Nodes can also be grouped for organization.
- After selecting a node, you can configure its specific properties from the Inspector panel on the left.

For more information see [Documentation](./Documentation~/com.kalmera.juicetwee.md)

---

## Available Nodes

Here is a list of the core nodes included in JuiceTwee, organized by category.

### Effect Nodes

These nodes perform the primary visual and audio effects. Any node that requires a scene object as a target will expose a field on the `EffectPlayer` component.

- **Animation:**
  - `AnimatorNode`: Control Animator component parameters and triggers.
- **Audio:**
  - `AudioMixerNode`: Adjust parameters on an AudioMixer.
  - `AudioSourceNode`: Modify properties of an AudioSource.
  - `ClipPlayNode`: Play an AudioClip.
- **Camera:**
  - `CameraNode`: Control general camera properties.
  - `CameraShakeNode`: Implement camera shake effects.
- **GameObject:**
  - `ActivateDeactivateNode`: Enable or disable GameObjects.
  - `ComponentNode`: Get or manipulate components on a GameObject.
  - `DestroyGameObject`: Destroy a GameObject from the scene.
  - `InstantiationNode`: Instantiate prefabs.
- **Particle:**
  - `ParticleSystemNode`: Control playback of Particle Systems.
- **Physics:**
  - `ColliderNode`: Control properties of Collider components.
  - `RigidBody3DNode`: Apply forces and modify properties of a Rigidbody.
- **Render:**
  - `FogNode`: Adjust scene fog settings.
  - `MaterialNode`: Change properties of a Material.
  - `SkyBoxNode`: Change the scene's Skybox material.
  - `SpriteRendererNode`: Modify properties of a SpriteRenderer.
- **Scene:**
  - `SceneNode`: Manage scene loading.
- **Time:**
  - `FreezeFrameNode`: Create a brief pause or "hit stop" effect.
  - `TimeScaleNode`: Animate the global `Time.timeScale`.
- **Transform:**
  - `PositionNode`: Animate the position of a Transform.
  - `RotationNode`: Animate the rotation of a Transform.
  - `ScaleNode`: Animate the scale of a Transform.
  - `ScaleAnimationNode`: A dedicated node for complex scale animations like bounce or squash & stretch.
- **UI:**
  - `CanvasGroupNode`: Animate the alpha and interactability of a CanvasGroup.
  - `ImageNode`: Control properties of a UI Image component.
  - `RectTransformNode`: Animate the position, size, and anchors of a RectTransform.
  - `TextMeshProUGUINode`: Animate properties of a TextMeshProUGUI component.

### Flow Nodes

These nodes control the logic and timing of the execution flow. They do not appear on the `EffectPlayer` inspector, as they don't require external targets.

- `DelayNode`: Waits for a specified duration before continuing execution.
- `ActivationCounterNode`: Activates its output after being triggered a certain number of times.
- `LimitCounterNode`: Limits the number of times an execution path can be triggered.

---

## Contributing

Contributions are welcome\! If you have ideas for new nodes, features, or improvements, feel free to open an issue or submit a pull request.

---

## License

This project is licensed under the **MIT License**. See the `LICENSE` file for details.
