using Godot;
using System;

public class Main : Node2D
{
    private Vector2 _initialPosition = new Vector2(256, 230);

    public override void _Ready()
    {
        var player = GetNode<Player>("Player");
        this.StartFight(player);
    }

    public void PlayerFallDown(Player player)
    {
        player.KillMe();
    }

    // [connection signal="ReStartFight" from="Player"]
    public void StartFight(Player player)
    {
        player.Position = _initialPosition;
        // TODO Some fight initialisations
    }
    // [connection signal="OnDying" from="Player"]
    public void PlayerDying(Player player)
    {
        GD.Print("I am dead " + player.Name);
    }
}
