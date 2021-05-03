using Godot;
using System;

public class character : KinematicBody2D
{
	Vector2 velocity;

	const float gravity = 20.0f;
	const float jump_height = -1050.0f;
	const float acceleration = 50.0f;
	const float max_speed = 800.0f;
	private float _health = 100;
	private float max_health = 100;
	float attack_state = 0;

	bool is_dead;

	Timer timer;
	Timer invulnerability_timer;
	AnimatedSprite sprite;
	Position2D arrow_point_pos;
	Label health_number_bar;
	PackedScene _arrow;

	[Signal]
	public delegate void Health_updated(float _health);
	[Signal]
	public delegate void Killed();

	public override void _Ready()
	{
		_health = max_health;
		invulnerability_timer = GetNode<Timer>("InvulnerabilityTimer");
		health_number_bar = (Label)GetNode("Label");
		_arrow = (PackedScene)GD.Load("res://Scenes/arrow.tscn");
		sprite = GetNode<AnimatedSprite>("Sprite");
		arrow_point_pos = GetNode<Position2D>("ShootingPosition");
		timer = GetNode<Timer>("Timer");
		is_dead = false;
	}

	public override void _PhysicsProcess(float delta)
	{
		health_number_bar.SetText(_health.ToString() + " " + "HP");
		velocity.y += gravity;
		if (!is_dead)
		{
			bool friction = false;
			
			if (Input.IsActionJustPressed("ui_select") && attack_state == 0)
			{
				sprite.Play("Attack");
				attack_state = 30;
			}
			if (attack_state > 0)
			{
				if (attack_state == 1)
				{
					arrow shot = (arrow)_arrow.Instance();

					shot.Init(80.0f, arrow_point_pos);
					GetParent().AddChild(shot);
				}
				attack_state--;
			}
			if (Input.IsActionPressed("ui_right"))
			{
				velocity.x = Math.Min(velocity.x + acceleration, max_speed);
				sprite.FlipH = false;
				if (attack_state == 0)
				{
					sprite.Play("Run");
				}
				if (Math.Sign(arrow_point_pos.Position.x) == -1)
				{
					arrow_point_pos.Position = new Vector2(-arrow_point_pos.Position.x, arrow_point_pos.Position.y);
				}
			}
			else if (Input.IsActionPressed("ui_left"))
			{
				velocity.x = Math.Max(velocity.x - acceleration, -max_speed);
				sprite.FlipH = true;
				if (attack_state == 0)
				{
					sprite.Play("Run");
				}
				if (Math.Sign(arrow_point_pos.Position.x) == 1)
				{
					arrow_point_pos.Position = new Vector2(-arrow_point_pos.Position.x, arrow_point_pos.Position.y);
				}
			}
			else
			{
				if (attack_state == 0)
				{
					sprite.Play("Idle");
				}
				friction = true;
			}


			// PHYSICS
			if (IsOnFloor())
			{
				if (Input.IsActionPressed("ui_up"))
				{
					velocity.y = jump_height;
				}
				if (friction)
				{
					velocity.x = Lerp(velocity.x, 0.0f, 0.2f);
				}
			}
			else
			{
				if (velocity.y < 0.0f)
				{
					if (attack_state == 0)
					{
						sprite.Play("Jump");
					}
				}
				else
				{
					if (attack_state == 0)
					{
						sprite.Play("Fall");
					}
				}
				if (friction)
				{
					velocity.x = Lerp(velocity.x, 0.0f, 0.05f);
				}
			}
		}
		velocity = MoveAndSlide(velocity, new Vector2(0, -1));
	}

	public void damage(float amount)
	{
		if (invulnerability_timer.IsStopped())
		{
			invulnerability_timer.Start();
			set_health(_health - amount);

		}

	}
	public void set_health(float health)
	{
		var prev_health = _health;
		sprite.Play("Get Hit");
		if ((health >= 0) && (health <= 100))
		{
			_health = health;
		}
		if (_health != prev_health)
		{
			EmitSignal("Health_updated", _health);
		}
		if (_health == 0)
		{
			dead();
		}

	}

	public void high_jump(float height)
	{
		velocity.y = -height;
	}

	public void dead()
	{
		is_dead = true;

		SetCollisionLayerBit(0, false);
		SetCollisionLayerBit(5, true);

		SetCollisionMaskBit(2, false);
		SetCollisionMaskBit(4, false);

		velocity.x = 0;
		sprite.Play("Death");
		timer.Start();
	}

	public void _on_Timer_timeout()
	{
		GetTree().ReloadCurrentScene();
	}

	float Lerp(float firstFloat, float secondFloat, float by)
	{
		return firstFloat * (1 - by) + secondFloat * by;
	}
}
