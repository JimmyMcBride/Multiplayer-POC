# Common Tasks

> **Read when:** Performing routine development tasks. Step-by-step workflows for frequent operations.

## Adding a New Input Action

### 1. Add to project.godot

Open in Godot Editor: Project → Project Settings → Input Map, or edit `project.godot`:

```ini
new_action={
"deadzone": 0.5,
"events": [Object(InputEventKey,...)]
}
```

### 2. Add to InputAction.cs

```csharp
// Game/Globals/Constants/InputAction.cs
public static class InputAction
{
    // ... existing actions ...
    public const string NewAction = "new_action";  // Must match project.godot
}
```

### 3. Use in Code

```csharp
if (Input.IsActionJustPressed(InputAction.NewAction))
{
    // Handle action
}
```

---

## Adding a New Character State

See [State Machine Guide](./state-machine-guide.md) for detailed instructions.

**Quick checklist:**
1. Create `States/NewState.cs` extending `PlayerState`
2. Override `Enter`, `Exit`, `PhysicsUpdate` as needed
3. Set capability flags (`IsInAirState`, `CanJump`, etc.)
4. Add node to StateMachine in `player_character.tscn`
5. Add transitions from/to other states

---

## Adding a New Component

### 1. Create the Component Class

```csharp
// Game/Scenes/Characters/PlayerCharacter/Components/NewComponent.cs
using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

public partial class NewComponent : Node  // Or Node3D if spatial
{
    private PlayerCharacter _controller;

    public void Initialize(PlayerCharacter controller)
    {
        _controller = controller;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Component logic...
    }
}
```

### 2. Add to Scene

Add as child of PlayerCharacter in `player_character.tscn`.

### 3. Wire Up in PlayerCharacter.cs

```csharp
public NewComponent NewComponent { get; private set; }

public override void _Ready()
{
    // ... existing code ...
    NewComponent = GetNode<NewComponent>("NewComponent");
    NewComponent.Initialize(this);
}
```

---

## Adding a New Method to PlayerCharacter

### 1. Add to PlayerCharacter

```csharp
// PlayerCharacter.cs
public void NewMethod(SomeType param)
{
    // Implementation, usually delegates to a component
    SomeComponent.NewMethod(param);
}
```

---

## Creating a New Level

### 1. Create Scene File

Create `Game/Scenes/Levels/NewLevel/new_level.tscn` with:
- Root Node3D named "NewLevel"
- Environment node with lighting and WorldEnvironment
- Level geometry
- PlayerCharacter instance

### 2. Basic Structure

```
NewLevel (Node3D)
├── Environment
│   ├── DirectionalLight3D
│   └── WorldEnvironment
├── Level
│   └── [geometry nodes]
└── PlayerCharacter (instance of player_character.tscn)
```

### 3. Set as Main Scene (Optional)

Project → Project Settings → Application → Run → Main Scene

---

## Adding a New Engine Utility

### 1. Create Utility Class

```csharp
// Engine/Core/NewUtility.cs
namespace MultiplayerPOC.Engine.Core;

public static class NewUtility
{
    public static ReturnType Method(Params)
    {
        // Implementation
    }
}
```

### 2. Use in Game Code

```csharp
using MultiplayerPOC.Engine.Core;

// Usage
NewUtility.Method(args);
```

---

## Adding a Godot Extension Method

```csharp
// Engine/Godot/Extensions.cs
public static class Extensions
{
    // Add to existing class
    public static Vector3 NewExtension(this Vector3 vector, float param)
    {
        return new Vector3(/* ... */);
    }
}
```

Usage: `myVector.NewExtension(value)`

---

## Debugging Workflow

### Enable Verbose Logging
```csharp
Log.Debug($"Variable: {variable}");
Log.Info($"State changed to: {state.Name}");
Log.Warning($"Unexpected condition: {condition}");
Log.Error($"Critical failure: {error}");
```

### Check State Machine
```csharp
Log.Debug($"Current state: {_stateMachine.CurrentState.Name}");
Log.Debug($"Is in air: {_stateMachine.CurrentState.IsInAirState}");
```

### Inspect Physics
```csharp
Log.Debug($"Velocity: {body.Velocity}");
Log.Debug($"On floor: {body.IsOnFloor()}");
Log.Debug($"Position: {body.GlobalPosition}");
```

---

## Build and Run

```bash
# From project root
godot --path . --editor  # Open in editor
godot --path .           # Run game directly
dotnet build             # Build C# only
```

---

## Common Gotchas

| Issue | Solution |
|-------|----------|
| State not found | Ensure state node is child of StateMachine in scene |
| Null controller | Check Initialize() is called in _Ready() |
| Input not working | Verify action name matches InputAction constant |
| Movement wrong direction | Check camera pivot is set in MovementComponent |
| Script not recognized | Ensure class is `partial` and namespace matches path |
