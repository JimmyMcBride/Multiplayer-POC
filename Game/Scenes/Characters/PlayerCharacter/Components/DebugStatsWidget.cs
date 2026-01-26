using Godot;
using MultiplayerPOC.Game.Globals;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

public partial class DebugStatsWidget : Control
{
    private Label _label;
    private double _updateTimer;
    private const double UpdateInterval = 0.25; // Update 4x per second

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
    }

    public override void _Process(double delta)
    {
        _updateTimer += delta;
        if (_updateTimer < UpdateInterval) return;
        _updateTimer = 0;

        var fps = Godot.Engine.GetFramesPerSecond();
        var ping = NetworkManager.Instance?.GetPing() ?? 0;

        _label.Text = $"FPS: {fps}\nPing: {ping}ms";
    }
}
