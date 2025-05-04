# UI Extension – Runtime Node Editor (Add-on)
A modular runtime node editor built on top of the UI Extension System. Designed for building visual logic graphs, dialogue systems, flow editors, and more.

⚠️ Requires: UI Extension System

## ✨ Features : 
- 🧱 Modular Node System – Plug in your own node prefabs
- 🔌 Connection System – Click + drag to connect logic
- 🖱️ Context Menu – Right-click to spawn nodes
- 🖼️ Zoom & Pan – Navigate with scroll and drag
- 🧩 Serialized Data Support – Easily expand with save/load
- 🎨 Procedural Grid Background – UI-based background grid

## 📦 Components :
- NodeEditor :
Manages nodes, connections, input, and UI.
Contains prefab references and runtime management.

- Node :
Base class for custom node logic (e.g., DialogueNode, MathNode).
Handles connections and selection behavior.

- Connection :
Visual Bezier curve linking outPoint → inPoint.
Clickable with hover feedback and visual styling.

- ConnectionPoint :
Define how many connections are allowed, their types (in, out).
Auto-validates links and manages cleanup.

- ProceduralGridBackground :
Scalable background grid that pans with the view.

## 🚀 Getting Started
- Set up the UI Extension System in your scene.
- Open the demo scene, or create a prefab using the NodeEditor GameObject.
- Customize parameters and node prefabs to fit your design.

⚠️ Note: The UiExtManager component must be present in your scene for input handling.

## 💡 Use Cases
Dialogue trees
Ability editors
Skill graphs
Scripting tools
Modular behaviors

## 🧩 Extending
Nodes are prefabs with logic → fully modular
Add buttons, fields, ports, or event hooks as needed
Editor can be reset/cleared/loaded at runtime

## Creating Nodes
Design your own nodes as prefab variants of the base Node class using standard Unity UI components.
Creating a class that inherits from Node is recommended for serialization and graph reconstruction.
Example:

''' csharp
Copier
Modifier
public class SimpleText : Node
{
    public InputField text;
}

[System.Serializable]
public class SimpleTextSerialized : SerializedNode
{
    public string text;

    public SimpleTextSerialized(Node node, SimpleText simpleText) : base(node)
    {
        text = simpleText.text.text;
    }
}
'''

## 📝 Notes
Not a graph logic executor — intended for visual layout and wiring.
Serialization is stubbed but extensible via SerializedNode and SerializedConnection.

## 📂 Structure
All node classes and logic are under UiExtension.NodalEditor
Prefabs, logic, and rendering are modular and UI-driven
