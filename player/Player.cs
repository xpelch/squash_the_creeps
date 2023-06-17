using System;
using Godot;

public partial class Player : CharacterBody3D
{
    private const string MoveRightAction = "move_right";
    private const string MoveLeftAction = "move_left";
    private const string MoveDownAction = "move_down";
    private const string MoveUpAction = "move_up";
    private const float CollisionNormalThreshold = 0.1f;

    [Signal]
    public delegate void HitEventHandler();
    [Export] public int MovementSpeed { get; set; } = 14;
    [Export] public int FallAcceleration { get; set; } = 75;
    [Export] public int JumpImpulse { get; set; } = 20;
    [Export] public int BounceImpulse { get; set; } = 16;

    private Vector3 _targetVelocity = Vector3.Zero;
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

    public override void _PhysicsProcess(double delta)
    {
        var direction = GetDirection();

        ApplyLookAt(direction);
        SetGroundVelocity(direction);
        ApplyGravity(delta);
        MoveAndSlide();
        PerformJump();
        HandleMobCollisions();
    }

    private void AnimatePlayer(Vector3 direction)
    {
        AnimationPlayer.SpeedScale = direction != Vector3.Zero ? 4 : 1;
    }

    private void PerformJump()
    {
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
            Console.Out.WriteLine("Jumping");
        }
    }

    private void HandleMobCollisions()
    {
        for (int index = 0; index < GetSlideCollisionCount(); index++)
        {
            HandleCollision(GetSlideCollision(index));
        }
    }

    private void HandleCollision(KinematicCollision3D collision)
    {
        if (!(collision.GetCollider() is Mob mob)) return;

        if (Vector3.Up.Dot(collision.GetNormal()) > CollisionNormalThreshold)
        {
            mob.Squash();
            _targetVelocity.Y = BounceImpulse;
        }
    }

    private Vector3 GetDirection()
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed(MoveRightAction))
        {
            direction.X += 1.0f;
        }

        if (Input.IsActionPressed(MoveLeftAction))
        {
            direction.X -= 1.0f;
        }

        if (Input.IsActionPressed(MoveDownAction))
        {
            direction.Z += 1.0f;
        }

        if (Input.IsActionPressed(MoveUpAction))
        {
            direction.Z -= 1.0f;
        }

        return direction != Vector3.Zero ? direction.Normalized() : direction;
    }

    private void ApplyLookAt(Vector3 direction)
    {
        if (direction != Vector3.Zero)
        {
            GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
        }
    }

    private void SetGroundVelocity(Vector3 direction)
    {
        _targetVelocity.X = direction.X * MovementSpeed;
        _targetVelocity.Z = direction.Z * MovementSpeed;
        Velocity = _targetVelocity;
    }

    private void ApplyGravity(double delta)
    {
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }
    }

    private void Despawn()
    {
        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    private void OnMobDetectorBodyEntered(Node3D body)
    {
        Despawn();
    }
}