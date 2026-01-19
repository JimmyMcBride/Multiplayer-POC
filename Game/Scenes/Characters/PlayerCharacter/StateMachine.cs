using System.Collections.Generic;
using Godot;
using MultiplayerPOC.Engine.Core;
using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Interfaces;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter;

public partial class StateMachine : Node
{
    private readonly Dictionary<string, State> _states = new();

    [Export] public State DefaultState;
    public State CurrentState { get; private set; }

    public void Initialize(ICharacterController controller)
    {
        foreach (var child in GetChildren())
            if (child is State s)
            {
                _states[s.Name] = s;
                s.Initialize(controller);
            }

        if (DefaultState == null) return;

        CurrentState = DefaultState;
        CurrentState.Enter(null);
        Log.Info($"Initial state set to {DefaultState.Name}");
    }

    public T GetState<T>() where T : State
    {
        foreach (var state in _states.Values)
            if (state is T typedState)
                return typedState;

        Log.Warning($"State of type {typeof(T).Name} not found");
        return null;
    }

    public void ChangeState<T>() where T : State
    {
        var state = GetState<T>();
        if (state == null)
        {
            Log.Error($"Attempted to change to state of type {typeof(T).Name}, but it was not found");
            return;
        }

        var previousState = CurrentState;
        CurrentState?.Exit();
        CurrentState = state;
        state.Enter(previousState);

        Log.Info($"State successfully changed from {previousState.Name} to {state.Name}");
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