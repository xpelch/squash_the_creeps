using Godot;
using System;

public partial class ScoreLabel : Label
{
    private int _score;
    
    public void OnMobSquashed()
    {
        _score++;
        Text = $"Score: {_score}";
    }
}
