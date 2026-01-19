using System.Collections.Generic;
using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;

public partial class StateMachine : Node
{
    private readonly Dictionary<string, PlayerState> _states = new();

    [Export] public Node DefaultStateNode;
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerCharacter controller)
    {
        _states.Clear();

        foreach (var child in GetChildren())
        {
            if (child is not PlayerState state) continue;
            _states[child.Name] = state;
            state.Initialize(controller);
        }

        if (DefaultStateNode is not PlayerState defaultState) return;
        CurrentState = defaultState;
        CurrentState.Enter(null);
    }

    public void ChangeState<T>() where T : PlayerState
    {
        foreach (var s in _states.Values)
        {
            if (s is not T next) continue;

            var prev = CurrentState;
            CurrentState?.Exit();
            CurrentState = next;
            CurrentState.Enter(prev);
            return;
        }
    }

    public override void _Process(double d)
    {
        CurrentState?.Update(d);
    }

    public override void _PhysicsProcess(double d)
    {
        CurrentState?.PhysicsUpdate(d);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CurrentState?.SpecialInput(@event);
    }
}