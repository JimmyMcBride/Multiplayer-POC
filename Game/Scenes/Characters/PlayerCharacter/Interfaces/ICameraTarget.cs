using MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Components;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Interfaces;

/// <summary>
///     Provides access to the camera component for lock-on and camera control.
/// </summary>
/// <remarks>
///     This interface is part of the Interface Segregation refactoring.
///     States that need camera information (e.g., for lock-on checks) can depend on this
///     instead of ICharacterController.
/// </remarks>
public interface ICameraTarget
{
    /// <summary>
    ///     Gets the camera component responsible for camera control and lock-on targeting.
    /// </summary>
    CameraComponent CameraComponent { get; }
}