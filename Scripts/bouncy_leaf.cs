using Godot;

public class bouncy_leaf : Area2D
{
	public override void _PhysicsProcess(float delta)
	{
		Godot.Collections.Array bodies = GetOverlappingBodies();
		foreach (var body in bodies)
		{
			if (body.GetType().Name == "character")
			{
				((character)body).high_jump(2000.0f);
			}
		}
	}
}
