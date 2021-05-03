using Godot;
using System;

public class enemy : KinematicBody2D
{
	const float gravity = 20.0f;
	const float max_speed = 200.0f;
	float _health = 30;
	Vector2 velocity;
	bool is_dead;
	float atack = 15;
	AnimatedSprite sprite;
	RayCast2D ground_ray;
	Timer timer;
	Timer Timer2;
	bool is_hit;
	float max_health = 30;
	private character player;
	public override void _Ready()
	{
		_health = max_health;
		sprite = GetNode<AnimatedSprite>("Sprite");
		player = (character)GetNode("/root/world_1/character");
		ground_ray = GetNode<RayCast2D>("Ray");
		timer = GetNode<Timer>("Timer");
		Timer2 = GetNode<Timer>("Timer2");
		is_dead = false;
		is_hit = false;
	}
	public override void _PhysicsProcess(float delta)
	{
		velocity.y += gravity;
		if (Timer2.IsStopped())
		{
			is_hit = false;
		}
		if (!is_dead && !is_hit) {
			if (IsOnFloor() && (IsOnWall() || !ground_ray.IsColliding()))
			{
				sprite.FlipH = !sprite.FlipH;
				ground_ray.Position = new Vector2(-ground_ray.Position.x, ground_ray.Position.y);
			}
			velocity.x = max_speed * (sprite.FlipH ? -1.0f : 1.0f);
			sprite.Play("Walk");
		}
		for (int i = 0; i < GetSlideCount(); i++)
		{
			Console.WriteLine(GetSlideCollision(i).Collider.GetType().Name);
			switch (GetSlideCollision(i).Collider.GetType().Name)
			{
				case "character":
					player.damage(atack);
					break;
			}
		}
		velocity = MoveAndSlide(velocity, new Vector2(0, -1));
	}
	public void Hit(float damage_taken)
	{
		damage(damage_taken);
	}
	public void _on_Timer_timeout()
	{
		QueueFree();
	}
	public void damage(float amount)
	{
		set_health(_health - amount);
	}
	public void set_health(float health)
	{
		var prev_health = _health;
		Got_Hit();

		if ((health >= 0) && (health <= max_health))
		{
			_health = health;
		}
		if (_health == 0)
		{
			is_dead = true;
			velocity.x = 0;
			SetCollisionLayerBit(2, false);
			SetCollisionLayerBit(5, true);
			SetCollisionMaskBit(0, false);
			SetCollisionMaskBit(2, false);
			SetCollisionMaskBit(3, false);
			sprite.Play("Death");
			timer.Start();
		}

	}
	public void Got_Hit()
	{
		if (Timer2.IsStopped())
		{
			Timer2.Start();
			is_hit = true;
			sprite.Play("Get Hit");
			velocity.x = 0;
		}
		
	}


}





