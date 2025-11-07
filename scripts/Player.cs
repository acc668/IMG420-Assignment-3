using Godot;

public partial class Player : Area2D
{
	[Export] public float Speed = 300f;
	
	private bool isAlive = true;
	private float survivalTime = 0f;
	private Label uiLabel;
	
	public override void _Ready()
	{
		AddToGroup("player");
		AreaEntered += OnAreaEntered;
		
		ProcessMode = ProcessModeEnum.Always;
		
		uiLabel = GetNode<Label>("../Label");
		
		GD.Print("Player is ready!");
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("boids") && isAlive)
		{
			isAlive = false;
			GD.Print($"Game Over! Survived for {survivalTime:F1} seconds!");
			
			GetTree().Paused = true;
			
			if (uiLabel != null)
			{
				uiLabel.Text = $"GAME OVER!\nSurvived: {survivalTime:F1}s\nPress SPACE to restart";
			}
		}
	}
	
	public override void _Process(double delta)
	{
		if (!isAlive && Input.IsKeyPressed(Key.Space))
		{
			GD.Print("Restarting game...");
			GetTree().Paused = false;
			GetTree().ReloadCurrentScene();
			return;
		}
		
		if (isAlive && uiLabel != null)
		{
			uiLabel.Text = $"WASD to move\nAvoid the red flocking enemies!\nTime: {survivalTime:F1}s";
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;
		
		survivalTime += (float)delta;
		
		Vector2 velocity = Vector2.Zero;
		
		if (Input.IsKeyPressed(Key.W))
			velocity.Y -= 1;
		if (Input.IsKeyPressed(Key.S))
			velocity.Y += 1;
		if (Input.IsKeyPressed(Key.A))
			velocity.X -= 1;
		if (Input.IsKeyPressed(Key.D))
			velocity.X += 1;
		
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			GlobalPosition += velocity * (float)delta;
		}
		
		var screenSize = GetViewportRect().Size;
		GlobalPosition = new Vector2(
			Mathf.Clamp(GlobalPosition.X, 0, screenSize.X),
			Mathf.Clamp(GlobalPosition.Y, 0, screenSize.Y)
		);
	}
}
