ListView — High-Performance Virtualized UI List for Unity

## 📚 Table of Contents

1. [Overview](#overview)
2. [Installation](#installation)
3. [Data Model](#data-model)
4. [Creating ListView Cards](#creating-list-view-cards)
5. [Setting Up ListView](#setting-up-list-view)
6. [Providing Data](#providing-data)
7. [Listeners](#listeners)
8. [Manipulator](#manipulator)

<h2 id="overview">✨ Overview</h2>

ListView is a high-performance virtualized list component for Unity, built for developers who want smooth scrolling, zero garbage, and clean architecture — without fighting the engine.
Its mission is simple: turn complex UI lists into something easier than brewing your morning coffee.
No magic, no hacks — just a pleasant API and performance that doesn’t melt your profiler.

<img src="Main/Preview.gif" width="900" />

<h2 id="installation">📦 Installation (UPM via Git URL)</h2>

ListView is distributed through GitHub Releases.
We strongly recommend installing a specific tagged version instead of the default branch to ensure maximum stability and predictable behavior.

### 🔽 1. Choose a Release Version

Open the Releases section on GitHub:

👉 https://github.com/MagasDeveloper/ListView/releases

You will see a list of available versions (for example: v1.0.0).
Each release contains:

- a version number
- a description of changes
- installation instructions

We recommend using the latest stable release, unless you specifically want an older version.

### 🔧 2. Install the Package via Git URL

In Unity, open:

Window → Package Manager → Add package from Git URL

Insert the URL with the selected version tag:

```bash
https://github.com/MagasDeveloper/ListView.git#v1.0.0
```

Replace v1.0.0 with any other release tag if needed.

Unity will download that exact version and lock it, so your project will remain stable even if new updates are published.

### 🧩 Assembly Definition Setup (If Your Project Uses asmdef)

ListView is compiled into its own assembly:

```bash
Mahas.ListView
```

If your project uses Assembly Definitions, you must reference this assembly manually:

1. Open your asmdef file
2. Go to Assembly Definition References
3. Add:

   ```bash
   Mahas.ListView
   ```

### 🔄 Updating to a New Version

To update, simply:
1. Go to Releases
2. Pick the version you want
3. Replace the tag in your Git URL in Package Manager:

```bash
https://github.com/MagasDeveloper/ListView.git#v1.0.3
```
Unity will fetch the new version automatically.

<h2 id="data-model">📦 Data Model — Implementing ILisViewtData</h2>

Every item displayed in ListView must implement the ILisViewtData interface.
And before you panic — yes, it’s basically a marker interface. No methods, no properties, no boilerplate.

Why does it exist?
Because ListView needs a simple way to confirm that your data type is “valid” for the virtualized system.
Think of it as a backstage pass:
If your data implements ILisViewtData, ListView knows it can work with it.
```csharp
public class InventoryItem : ILisViewtData
{
    public string Name;
    public Sprite Icon;
    public int Amount;
}
```

<h2 id="creating-list-view-cards">🧩 Creating ListView Cards — Your UI Frontend</h2>

Each visual element inside the ListView is represented by a card — a MonoBehaviour that inherits from:
```csharp
ListViewCard<TData>
```

Where TData is your data model type (the one that implements ILisViewtData).
This makes every card strongly typed, safe, and directly connected to the data it represents.

<h2>🎛 Available Lifecycle Methods</h2>
Cards expose several overridable methods that let you control how they behave:

```csharp
/// Called when the card is created (instantiated for the first time).
/// Override to set up references, subscribe to events, etc.
protected virtual void OnCreate() { }

/// Called when new data is assigned to the card.
/// Override to update visuals or refresh UI elements.
protected virtual void OnRefresh() { }

/// Called when the card is released permanently (usually during ListView cleanup).
/// Override for custom cleanup logic.
protected virtual void OnDelete() { }

/// Called when the card enters the visible viewport and becomes active.
/// MUST be implemented. Use this to render data on screen.
protected abstract void OnSpawn();

/// Called when the card leaves the viewport and gets recycled.
/// Override to reset visuals or stop animations.
protected virtual void OnRecycle() { }
```

<h2>📌 Accessing Your Data</h2>

Every card has direct read-only access to its assigned data item:
```csharp
protected TData Data { get; private set; }
```
This ensures clean separation of data and UI, while keeping the workflow extremely simple.

A minimal example showing how to bind your UI fields to your data:
```csharp
public class MissionCard : ListViewCard<MissionData>
{
    [SerializeField] private TextMeshProUGUI _missionNameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    
    protected override void OnSpawn()
    {
        _missionNameText.text = Data.MissionName;
        _descriptionText.text = Data.Description;
    }
}
```

This is all most cards ever need.

You get clean, type-safe data access and a simple lifecycle that’s easy to reason about — no heavy abstractions, no magic events, no overcomplicated UI frameworks.

<h2 id="setting-up-list-view">🧱 Setting Up ListView in Unity</h2>

Adding ListView to your Canvas is intentionally simple — it works on top of Unity’s built-in ScrollRect, so if you’ve ever used a normal scroll view, you’re already 90% done.
Steps:
1. Create a standard UI Scroll View (GameObject → UI → Scroll View)
2. Select the root GameObject containing the ScrollRect.
3. Add the ListView component to the same object.

That’s it — ListView automatically connects to your existing ScrollRect, Content, and Viewport.
From this moment on, you should not manually instantiate UI elements into the Content.
Instead, you feed data directly to the ListView, and it handles virtualization, item recycling, and layout behind the scenes.

The result should look like this:

<img width="594" height="913" alt="image" src="https://github.com/user-attachments/assets/865ccfe9-01a8-4681-a7da-bb88a1395e70" />


> 💡 Why ScrollRect?
>
> Because Unity’s ScrollRect already handles user input, inertia, boundaries and scrolling physics nicely.
> ListView simply replaces the expensive part — the items themselves.

<h2>⚙️ListView Component Settings </h2>

1. **Prefab List Variants**  
   ListView supports multiple card types, allowing you to display different visual layouts inside one list.  
   To enable this, simply add your card prefabs to the Prefab List Variants section.

   Each variant contains:
   - **Prefab** — a UI card that inherits from `ListViewCard<TData>`
   - **Initial Pool Size** — how many instances should be pre-created at startup

   <img width="592" height="216" alt="image" src="https://github.com/user-attachments/assets/7edf9a91-20f5-4c04-afa9-7660491bcacc" />

   This helps ListView prepare and reuse cards efficiently, improving performance and reducing runtime instantiation spikes.

   Use multiple variants when your list contains different types of items (e.g., normal missions, boss missions, headers, ads, etc.).

   <img width="1121" height="717" alt="image" src="https://github.com/user-attachments/assets/62efaf25-e3f5-41fc-84e5-345ea4a08436" />


2. **Viewport Settings**
   
    The Viewport Settings section allows you to control when cards are considered visible by adjusting the visibility padding around the viewport.
   
    <img width="594" height="148" alt="image" src="https://github.com/user-attachments/assets/e035315f-8a2b-48b2-9c78-c6bc6397852a" />

    These paddings act as extra margins outside the actual viewport boundaries.
    If a card moves slightly outside the viewport but still within these paddings, it will remain spawned instead of being immediately recycled.

    This is useful when:
   - you want smoother transitions while scrolling
   - your UI animations extend slightly beyond the viewport
   - you want to avoid “pop-in” effects at the edges

   Example:
    Setting left/right to –10 means cards will stay visible until they move 10 pixels beyond the viewport’s horizontal borders.

3. **Content Settings**

   The Content Settings section controls how cards are arranged inside the ListView.

   <img width="597" height="190" alt="image" src="https://github.com/user-attachments/assets/e410efd7-e985-4862-93a9-444144824ed8" />

    • Direction
    Defines the layout direction of your list — Horizontal or Vertical.
    ListView will automatically calculate card positions based on your choice.
    
    • Paddings
    These are the internal margins applied around all cards.
    The total content size = all card sizes + paddings.
    Useful for adding breathing room at the edges of the list.
    
    • Spacing
    Controls the space between individual cards.
    Perfect for achieving clean, consistent UI gaps without modifying the card prefabs themselves.

4. **Other Settings**

   This section contains optional features that you can enable depending on your project needs.

    <img width="595" height="81" alt="image" src="https://github.com/user-attachments/assets/4cb9bada-9aab-4a62-8a55-8807a012dd7c" />

    • Keep Sibling Order
    When enabled, ListView will keep the hierarchy order of instantiated cards in sync with the order of your data.
    This uses transform.SetSiblingIndex(), so it may introduce a small performance cost and should be enabled only when you truly need consistent hierarchy order (e.g., for debugging or custom UI effects).
    
    • Is Enable Gizmo
    Draws helper gizmos in the Scene View, showing viewport bounds, padding offsets, and card visibility regions.
    Useful for debugging layout issues or tuning scroll behavior.
    
    <img width="1492" height="606" alt="image" src="https://github.com/user-attachments/assets/db5aea51-e3cf-467b-ac5e-30841cc59ee1" />

<h2 id="providing-data">📥Providing Data to the ListView</h2>

Feeding data into the ListView is intentionally simple.
All you need to do is prepare a list of items that implement ILisViewtData and pass it to the ListView using SetupData.

Here’s a minimal example:

```csharp
public class MissionsPanel : MonoBehaviour
{
    [SerializeField] private ListView _listView;

    private readonly List<ILisViewtData> _data = new();

    private void Start()
    {
        InitializeData();               // Fill your list with items that implement ILisViewtData
        _listView.SetupData(_data);     // Pass the data to the ListView
    }
}
```

And that's it.

No manual instantiation.
No layout rebuilding.
No recycling logic.

ListView takes care of everything automatically: virtualization, pooling, spawning, recycling, layout positioning — all behind the scenes.

<h2 id="listeners">🎧 Listeners — Subscribing to ListView Events</h2>

ListView exposes several useful events that allow you to react to what happens inside the list: card creation, spawning, recycling, and content movement.
These events are available through:

```csharp
_listView.Listeners
```
You can subscribe to them at any time — for example, right after SetupData():

```csharp
public class MissionsPanel : MonoBehaviour
{
    [SerializeField] private ListView _listView;

    private void Start()
    {
        InitializeData();
        _listView.SetupData(_data);

        _listView.Listeners.OnCreate.AddListener(OnCardCreated);
        _listView.Listeners.OnSpawn.AddListener(OnCardSpawned);
        _listView.Listeners.OnRecycle.AddListener(OnCardRecycled);
        _listView.Listeners.OnContentMove.AddListener(OnContentMoved);
    }

    private void OnCardCreated(ListViewElement element)
    {
        Debug.Log($"Card created: {element}");
    }

    private void OnCardSpawned(ListViewElement element)
    {
        Debug.Log($"Card spawned: {element}");
    }

    private void OnCardRecycled(ListViewElement element)
    {
        Debug.Log($"Card recycled: {element}");
    }

    private void OnContentMoved(Vector2 position)
    {
        Debug.Log($"Content moved: {position}");
    }
}
```
### 🔎 Available Events

Each event is defined inside ListListeners:
- OnCreate — triggered when a card GameObject is instantiated in the pool.
- OnSpawn — triggered when a data item becomes visible in the viewport and is assigned to a card.
- OnRecycle — triggered when a card moves outside the viewport and is recycled.
- OnContentMove — called every time the ScrollRect content moves.

### 💡Why Listeners Are Useful?

You can use them for:

- tracking when elements appear/disappear
- playing animations or sound effects
- lazy-loading resources
- analytics (e.g., “element seen by user”)
- debugging scroll behavior

<h2 id="manipulator">🎯 ListView Manipulator — Smooth, User-Friendly Navigation</h2>

The ListView Manipulator is a built-in helper that allows you to smoothly scroll to any item or position inside your ListView.
Its goal is simple: replace hard jumps with beautiful, animated navigation — perfect for menus, mission trees, inventories, and any UI that needs a polished feel.

You can access it through:

```csharp
_listView.Manipulator
```

### 🚀 Basic Usage

Scrolling to an item by index:

```csharp
_listView.Manipulator
    .ScrollTo(5, 0.35f)
    .Play();
```

Scrolling to an item by data instance:

```csharp
_listView.Manipulator
    .ScrollTo(myMissionData, 0.5f)
    .Play();
```

That’s all you need for a simple animated scroll.

### 🛠️ What the Manipulator Actually Does

Under the hood, the manipulator:

- finds the virtual card that corresponds to the item
- calculates the exact target position inside the ScrollRect
- builds a smooth animation process
- moves the content using lerp or a custom animation curve
- interrupts the previous scroll if you start a new one
- Thanks to virtualization, this works even if the item isn’t currently visible.

### 🔧 Advanced: Customizing the Scroll Animation

The manipulator uses a builder pattern, so you can chain settings:

```csharp
_listView.Manipulator
    .ScrollTo(10, 0.75f)
    .SetOffset(20f)               // scroll slightly past the element
    .SetDelay(0.1f)               // delay before movement begins
    .SetTimeScaled(false)         // ignore Time.timeScale
    .SetAlignment(AlignmentType.Center)
    .SetAnimationCurve(_animationCurve)
    .OnStart(() => Debug.Log("Scrolling..."))
    .OnComplete(() => Debug.Log("Done"))
    .Play();
```

You can even run it as a coroutine:

```csharp
yield return _listView.Manipulator
    .ScrollTo(3, 0.5f)
    .PlayCoroutine(this);
```

or as an async task:

```csharp
await _listView.Manipulator
    .ScrollTo(3, 0.5f)
    .PlayAsync(token);
```

All available thanks to the process builder in ListViewManipulatorProcessBuilder

### 📐 Alignment Options

Manipulator supports aligning the target element to:

- Start (default)
- Center
- End

```csharp
.SetAlignment(AlignmentType.Center)
```

Perfect for mission selectors, carousels, or anything where the focused item should be highlighted.

### ⚡ Interrupt-Safe

Every new scroll automatically interrupts the previous one:

```csharp
_currentProcess?.Interrupt();
```

(from ListViewManipulator implementation)
ensuring clean transitions and no stuck animations.

### 🎉 Summary

| Feature              | Description                               |
|----------------------|-------------------------------------------|
| Scroll to index      | `ScrollTo(index, duration)`               |
| Scroll to data       | `ScrollTo(data, duration)`                |
| Smooth animations    | `AnimationCurve`, delay, offset           |
| Robust control       | Start, complete, interrupt callbacks      |
| Multiple modes       | `Play()`, `PlayAsync()`, `PlayCoroutine()` |
| Auto interrupt       | Never overlaps animations                 |
| Works with virtualization | Even if the item is off-screen     |
