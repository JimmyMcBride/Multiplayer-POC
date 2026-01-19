using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Interfaces;

/// <summary>
///     Provides access to physics body properties and airborne state.
/// </summary>
/// <remarks>
///     This interface is part of the Interface Segregation refactoring.
///     States that only need physics access can depend on this instead of ICharacterController.
/// </remarks>
public interface IPhysicsBody
{
    /// <summary>
    ///     Gets the Godot CharacterBody3D for physics operations.
    /// </summary>
    CharacterBody3D Body { get; }

    /// <summary>
    ///     Indicates whether the character is currently airborne (not on ground).
    /// </summary>
    bool IsInAir { get; }
}