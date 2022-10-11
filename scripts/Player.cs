using Godot;
using System;
using System.Linq;

public class Player : KinematicBody2D
{
    [Export] public int speed = 200;
    [Export] public PackedScene BulletScene;

    [Signal] public delegate void ReStartFight(Player player);

    private bool _isAlive = true;

    public Vector2 velocity = new Vector2();
    public Vector2 lastVelocity = new Vector2();

    public void GetInput()
    {
        velocity = new Vector2();
        var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        if (_isAlive)
        {
            // Movement inputs
            if (Input.IsActionPressed("right"))
                velocity.x += 1;

            if (Input.IsActionPressed("left"))
                velocity.x -= 1;

            if (Input.IsActionPressed("down"))
                velocity.y += 1;

            if (Input.IsActionPressed("up"))
                velocity.y -= 1;


            // Fighting inputs
            if (Input.IsActionJustPressed("shoot"))
            {
                int degree = 0;
                if (lastVelocity.x != 0)
                {
                    degree = lastVelocity.x > 0 ? 0 : 180;
                }
                else if (lastVelocity.y != 0)
                {
                    degree = lastVelocity.y > 0 ? 90 : 270;
                }
                Shoot(degree);
            }
        }

        if (velocity.Length() > 0)
        {
            if (velocity.x != 0)
            {
                animatedSprite.FlipH = velocity.x < 0;
            }
            velocity = velocity.Normalized() * speed;
            lastVelocity = velocity;
            animatedSprite.Play();
        }
        else
        {
            animatedSprite.Stop();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput();
        if (_isAlive)
        {
            velocity = MoveAndSlide(velocity);
        }
        else
        {
            lastVelocity = MoveAndSlide(lastVelocity);
        }
    }

    public void KillMe()
    {
        this._isAlive = false;

        var tween = GetNode<Tween>("DeadTween");
        tween.InterpolateProperty(
            this,
            "rotation_degrees",
            RotationDegrees,
            RotationDegrees - 360,
            0.5f,
            Tween.TransitionType.Linear,
            Tween.EaseType.In
        );
        tween.Start();

        var timer = GetNode<Timer>("DeadTimer");
        timer.Start();
    }

    public void OnDeadTimerTimeout()
    {
        GetNode<Tween>("DeadTween").Stop(this);
        _isAlive = true;

        EmitSignal(nameof(ReStartFight), this);
        BlinkMe();
    }

    private async void BlinkMe()
    {
        foreach (int value in Enumerable.Range(0, 4))
        {
            var tween = GetNode<Tween>("AnimationTween");
            tween.InterpolateProperty(GetNode<AnimatedSprite>("AnimatedSprite"), "modulate:a", 1, 0, 0.3f);
            tween.Start();
            await ToSignal(tween, "tween_completed");
            tween.InterpolateProperty(GetNode<AnimatedSprite>("AnimatedSprite"), "modulate:a", 0, 1, 0.2f);
            tween.Start();
        }
    }

    private void Shoot(int degree)
    {
        var bullet = (Bullet)BulletScene.Instance();
        bullet.RotationDegrees = degree;

        AddChild(bullet);
    }
}
