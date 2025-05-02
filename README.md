# Unity UI Extension System

A lightweight runtime UI utility for Unity UI (UGUI), providing:
- Advanced mouse event handling (hover, click, etc.)
- Extended buttons with click type and button ID support
- Drag-and-drop support with drop zones
- Component-based, UnityEvent-driven system

> ✅ Built on UnityEngine.UI  
> 🧩 No external dependencies  
> 🎮 Runtime focused, editor-friendly  

---

## Features

✔️ `UiExtElement` – Base class for custom UI events  
✔️ `ExtButton` – Extended buttons with click types and button index  
✔️ `MouseOver` – Add hover events to any UI element  
✔️ `DraggableElement` – Drag any UI object  
✔️ `DropZone` – Drop zones for draggable elements (slot/grid systems)  
✔️ `UiExtManager` – Central controller (auto-assigned, singleton)  

---

## Use Cases

- Inventory drag & drop
- Advanced button input (right click, hold, etc.)
- UI behaviors not handled by UnityEvents alone
- Modular visual scripting support

---

## Getting Started

1. Add `UiExtManager` to any scene with UI elements
2. Add `UiExtElement`-based components to your UI objects:
   - Use `MouseOver` for hover detection
   - Use `ExtButton` to respond to `Right-Click`, `Hold`, etc.
   - Use `DraggableElement` + `DropZone` for drag & drop

🎯 Components can be combined.

---

## Examples

### 🔘 Custom Click Behavior

```csharp
ExtButton btn = GetComponent<ExtButton>();
btn.mouseButton = 1; // Right click
btn.clickType = ClickType.Down;
btn.onClick.AddListener(() => Debug.Log("Right click!"));

👉 For an example implementation, see the [`NodeEditor_README.md`](NodeEditor_README.md)
