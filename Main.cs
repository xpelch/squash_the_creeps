using Godot;

public partial class Main : Node
{
    private const string RetryControlNodePath = "UserInterface/Retry";
    private const string MobTimerNodePath = "MobTimer";
    private const string PlayerNodePath = "Player";
    private const string ScoreLabelNodePath = "UserInterface/ScoreLabel";
    private const string SpawnPathNodePath = "SpawnPath/SpawnLocation";

    [Export]
    public PackedScene MobScene { get; set; }

    private Control RetryControl => GetNode<Control>(RetryControlNodePath);
    private Timer MobTimer => GetNode<Timer>(MobTimerNodePath);
    private Player Player => GetNode<Player>(PlayerNodePath);
    private ScoreLabel ScoreLabel => GetNode<ScoreLabel>(ScoreLabelNodePath);
    private PathFollow3D SpawnLocation => GetNode<PathFollow3D>(SpawnPathNodePath);

    public override void _Ready()
    {
        RetryControl.Hide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && RetryControl.Visible)
        {
            // This restarts the current scene.
            GetTree().ReloadCurrentScene();
        }
    }

    private void OnMobTimerTimeout()
    {
        // Create a new instance of the Mob scene.
        Mob mob = MobScene.Instantiate<Mob>();

        // Choose a random location on the SpawnPath.
        // And give it a random offset.
        SpawnLocation.ProgressRatio = GD.Randf();

        mob.Initialize(SpawnLocation.Position, Player.Position);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
        mob.Squashed += ScoreLabel.OnMobSquashed;
    }

    private void OnPlayerHit()
    {
        MobTimer.Stop();
        RetryControl.Show();
    }
}