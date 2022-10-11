using Godot;
using System;

public class Planet : Area2D
{
    [Signal] public delegate void OnPlayerExit(Player player);

    public void OnPlanetBodyExited(Node body)
    {
        if (body is Player)
        {
            EmitSignal(nameof(OnPlayerExit), body);
        }

    }
}
