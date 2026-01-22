# Multiplayer POC

> **Token-saving tip:** Only read guides in `ClaudeInstructions/` when relevant to your current task.

## Tech Stack

- **Engine:** Godot 4.5 with Forward Plus renderer
- **Language:** C# (.NET 8)
- **Branch:** `trunk`

## Quick Start

```bash
godot --path . --editor  # Open editor
dotnet build             # Build C# only
```

## Running the Game

### Dedicated Server
```bash
godot --path . --server                    # With UI (debug)
godot --path . --headless --server         # Headless (production)
godot --path . --server --port=9999        # Custom port
```

### Client
```bash
godot --path .                             # Connect to localhost:7777
godot --path . --address=192.168.1.100     # Connect to specific server
godot --path . --address=<ip> --port=9999  # Custom address and port
```

### Command-Line Arguments
| Argument | Default | Description |
|----------|---------|-------------|
| `--server` | (client mode) | Run as dedicated server |
| `--address=<ip>` | `127.0.0.1` | Server address for clients |
| `--port=<port>` | `7777` | Network port |

## Project Structure

```
Engine/           # Core utilities, Godot extensions
Game/
├── Globals/      # Constants (InputAction.cs)
├── Resources/    # Materials, assets
└── Scenes/
    ├── Characters/PlayerCharacter/  # Player system
    └── Levels/                      # Level scenes
Assets/           # External assets
ClaudeInstructions/  # Detailed guides (load on-demand)
```

## Key Files

| File | Purpose |
|------|---------|
| `Game/Scenes/Characters/PlayerCharacter/PlayerCharacter.cs` | Main character controller |
| `Game/Scenes/Characters/PlayerCharacter/StateMachine.cs` | State management |
| `Game/Scenes/Characters/PlayerCharacter/PlayerState.cs` | Base state class |
| `Game/Scenes/Characters/PlayerCharacter/States/*.cs` | Individual states |
| `Game/Globals/Constants/InputAction.cs` | Input action names |
| `Game/Globals/GameConfig.cs` | Command-line args, game mode (server/client) |
| `Game/Globals/NetworkManager.cs` | Multiplayer networking |
| `Engine/Core/Log.cs` | Logging utility |
| `project.godot` | Project settings, input mappings |

## Common Namespaces

```csharp
using Godot;
using MultiplayerPOC.Engine.Core;                    // Log
using MultiplayerPOC.Engine.Godot;                   // Extensions (SetX, SetY, etc.)
using MultiplayerPOC.Game.Globals;                   // GameConfig, NetworkManager
using MultiplayerPOC.Game.Globals.Constants;         // InputAction
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;            // PlayerState, StateMachine
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.States;     // Idle, Move, Jump, Dash...
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components; // CameraComponent, MovementComponent, etc.
```

## Quick Reference

### Logging
```csharp
Log.Debug("msg");   // Green
Log.Info("msg");    // Cyan
Log.Warning("msg"); // Yellow
Log.Error("msg");   // Red
```

### Input Actions
```csharp
InputAction.Jump              // "jump" (Space)
InputAction.Forward           // "move_forward" (W)
InputAction.Backward          // "move_backward" (S)
InputAction.Left              // "move_left" (A)
InputAction.Right             // "move_right" (D)
InputAction.Sprint            // "sprint" (Shift)
InputAction.Dash              // "dash" (Tab/B)
InputAction.Attack            // "attack" (LMB)
InputAction.Crouch            // "crouch" (C)
InputAction.Inventory         // "inventory" (I)
InputAction.Accept            // "accept" (E)
InputAction.Cancel            // "cancel" (Q)
InputAction.Pause             // "pause" (P)
InputAction.Quit              // "quit" (Esc)
InputAction.ToggleFacingMode  // "toggle_facing_mode" (V)
```

### State Transitions
```csharp
StateMachine.ChangeState<Idle>();
StateMachine.ChangeState<Move>();
StateMachine.ChangeState<Jump>();
StateMachine.ChangeState<Fall>();
StateMachine.ChangeState<Land>();
StateMachine.ChangeState<Sprint>();
StateMachine.ChangeState<Dash>();
```

### Vector Extensions
```csharp
vector.SetX(val)  vector.SetY(val)  vector.SetZ(val)
vector.SetXy(x,y) vector.SetXz(x,z) vector.SetYz(y,z)
```

### Smooth Interpolation
```csharp
var t = 1f - Mathf.Exp(-speed * (float)delta);
value = Mathf.Lerp(value, target, t);
```

## Detailed Guides

| Guide | Read When |
|-------|-----------|
| [Architecture Overview](ClaudeInstructions/architecture-overview.md) | Understanding system design, component relationships, planning features |
| [Coding Standards](ClaudeInstructions/coding-standards.md) | Writing new code, checking conventions, organizing files |
| [State Machine Guide](ClaudeInstructions/state-machine-guide.md) | Creating states, modifying transitions, debugging state behavior |
| [Common Tasks](ClaudeInstructions/common-tasks.md) | Step-by-step workflows for routine development tasks |

## Guide Selection

| Task Type | Guide to Read |
|-----------|---------------|
| Add new character ability/state | `state-machine-guide.md` |
| Add new input action | `common-tasks.md` |
| Add new component | `common-tasks.md` |
| Understand codebase | `architecture-overview.md` |
| Fix styling/conventions | `coding-standards.md` |
| Debug state issues | `state-machine-guide.md` |
| Plan new feature | `architecture-overview.md` |
| Create new level | `common-tasks.md` |

## Maintenance

When making changes that affect the architecture or patterns:
1. Update the relevant guide in `ClaudeInstructions/`
2. Update this file if key files or quick references change
