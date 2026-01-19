using Godot;

namespace MultiplayerPOC.Game.Scenes.Characters.PlayerCharacter.Interfaces;

/// <summary>
///     Provides movement direction calculation and rotation control.
/// </summary>
/// <remarks>
///     This interface is part of the Interface Segregation refactoring.
///     States that need movement control can depend on this instead of ICharacterController.
/// </remarks>
public interface IMovementController
{
    /// <summary>
    ///     Rotates the character to look in the specified direction.
    /// </summary>
    /// <param name="direction">World-space direction to face.</param>
    /// <param name="delta">Frame delta time for smooth interpolation.</param>
    void LookTowardDirection(Vector3 direction, float delta);

    /// <summary>
    ///     Gets the character's current rotation basis.
    /// </summary>
    /// <returns>The character's rotation basis.</returns>
    Basis GetCharacterBasis();

    /// <summary>
    ///     Converts 2D input direction to 3D movement direction.
    /// </summary>
    /// <param name="inputDir">2D input direction from controller/keyboard.</param>
    /// <returns>3D world-space movement direction.</returns>
    Vector3 GetMovementDirection(Vector2 inputDir);
}