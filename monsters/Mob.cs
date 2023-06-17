using Godot;

public partial class Mob : CharacterBody3D
{
    [Export]
    public int MinSpeed { get; set; } = 10;

    [Export]
    public int MaxSpeed { get; set; } = 18;

    [Signal]
    public delegate void SquashedEventHandler();

    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    /// <summary>
    /// Initializes the mob with given parameters.
    /// </summary>
    /// <param name="startPosition">The position where the mob will be placed.</param>
    /// <param name="playerPosition">The position of the player that the mob will face.</param>
    public void Initialize(Vector3 startPosition, Vector3 playerPosition)
    {
        int randomSpeed = CalculateRandomSpeed();
        SetMobPositionAndOrientation(startPosition, playerPosition);
        ApplyRandomRotation();
        SetMobVelocity(randomSpeed);
        AnimationPlayer.SpeedScale = randomSpeed / MinSpeed;
    }

    /// <summary>
    /// Sets the position and orientation of the mob based on given parameters.
    /// </summary>
    private void SetMobPositionAndOrientation(Vector3 startPosition, Vector3 playerPosition)
    {
        LookAtFromPosition(startPosition, playerPosition, Vector3.Up);
    }

    /// <summary>
    /// Applies a random rotation to the mob.
    /// </summary>
    private void ApplyRandomRotation()
    {
        RotateY((float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));
    }

    /// <summary>
    /// Sets the velocity of the mob based on the given speed.
    /// </summary>
    private void SetMobVelocity(int randomSpeed)
    {
        Velocity = CalculateVelocity(randomSpeed);
    }

    /// <summary>
    /// Calculates the velocity of the mob based on the given speed.
    /// </summary>
    private Vector3 CalculateVelocity(int randomSpeed)
    {
        Vector3 forwardVelocity = Vector3.Forward * randomSpeed;
        return forwardVelocity.Rotated(Vector3.Up, Rotation.Y);
    }

    /// <summary>
    /// Calculates a random speed within the range of MinSpeed and MaxSpeed.
    /// </summary>
    private int CalculateRandomSpeed()
    {
        return GD.RandRange(MinSpeed, MaxSpeed);
    }

    /// <summary>
    /// Triggers when the mob is no longer visible on the screen.
    /// </summary>
    private void OnVisibilityNotifierScreenExited()
    {
        QueueFree();
    }

    /// <summary>
    /// Triggers when the mob is squashed by the player and emits the Squashed signal.
    /// </summary>
    public void Squash()
    {
        EmitSignal(SignalName.Squashed);
        QueueFree();
    }
}
