using Godot;
using System;

public class Bullet : Area2D
{

    int speed = 250;

    public override void _Ready()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").Playing = true;
    }

    public override void _Process(float delta)
    {
        Position += Transform.x * speed * delta;
    }

    //Signals
    public void OnVisibilityNotifierScreenExited()
    {
        QueueFree();
    }

}
