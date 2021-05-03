using Godot;

public class world_complete : Area2D
{
	[Export(PropertyHint.File, "*.tscn")]
	public string next_world_name;
	public override void _PhysicsProcess(float delta)
	{
		Godot.Collections.Array bodies = GetOverlappingBodies();
		foreach (var body in bodies)
		{
			if (body.GetType().Name == "character")
			{
				GetTree().ChangeScene(next_world_name);
			}
		}
	}
}
