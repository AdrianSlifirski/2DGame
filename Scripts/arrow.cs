using Godot;
using System;

public class arrow : KinematicBody2D
{
	// PHYSICS CONSTANTS
	private const float GRAVITY = 10.0f;
	private const float MASS = 10.0f;
	private const int OBJECT_DECAY = 10;

	// PHYSICS VARIABLES
	private Vector2 _velocity;

	private float _speed;
	private bool _is_stopped;

	// OTHER VARIABLES
	private AnimatedSprite sprite;
	private Timer timer;
	private CollisionShape2D collision_test;
	public void Init(float spd, Position2D anch)
	{
		sprite = GetNode<AnimatedSprite>("sprite");
		timer = GetNode<Timer>("timer");
		_is_stopped = false;

		if (Math.Sign(anch.Position.x) == -1)
		{
			sprite.FlipH = true;
		}
		Position = anch.GlobalPosition;
		_speed = spd * (sprite.FlipH ? -1.0f : 1.0f);
		_velocity = new Vector2(_speed, 0);
	}
	public override void _Ready()
	{
		timer.WaitTime = OBJECT_DECAY;
		timer.Start();
	}
	public void Stop()
	{
		_is_stopped = true;
		_velocity = new Vector2(0, 0);
		sprite.Stop();
		timer.Stop();
		timer.WaitTime = 1;
		timer.Start();
	}
	public void Destroy()
	{

		_is_stopped = true;
		QueueFree();
	}
	public override void _PhysicsProcess(float delta)
	{
		if (!_is_stopped)
		{
			sprite.Play("Fly");
			_velocity.y += GRAVITY * MASS * delta;
			Rotation = (float)Math.Atan2(_velocity.y, _velocity.x) + (float)(sprite.FlipH ? -Math.PI : 0);

			var collision = MoveAndCollide(_velocity);
			if (collision != null)
			{
				GetNode<CollisionShape2D>("collision").Disabled = true;
				_velocity = new Vector2(0, -1000);
				Stop();
				if (collision.Collider.HasMethod("Hit"))
				{
					collision.Collider.Call("Hit",10.0);
					

				}
			}
		}
	}
}
