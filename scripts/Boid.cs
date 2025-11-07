using Godot;
using System.Collections.Generic;

public partial class Boid : Area2D
{
	[Export] public float MaxSpeed = 150f;
	[Export] public float MaxForce = 5f;
	[Export] public float SeparationRadius = 50f;
	[Export] public float AlignmentRadius = 75f;
	[Export] public float CohesionRadius = 75f;
	[Export] public float SeparationWeight = 1.5f;
	[Export] public float AlignmentWeight = 1.0f;
	[Export] public float CohesionWeight = 1.0f;
	[Export] public float SeekWeight = 1.2f;
	
	private Vector2 velocity = Vector2.Zero;
	private Vector2 acceleration = Vector2.Zero;
	
	public override void _Ready()
	{
		AddToGroup("boids");

		velocity = new Vector2(
			(float)GD.RandRange(-MaxSpeed, MaxSpeed),
			(float)GD.RandRange(-MaxSpeed, MaxSpeed)
		);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		acceleration = Vector2.Zero;
		
		var neighbors = GetNearbyBoids();
		
		if (neighbors.Count > 0)
		{
			Vector2 separation = Separation(neighbors);
			Vector2 alignment = Alignment(neighbors);
			Vector2 cohesion = Cohesion(neighbors);
			
			separation *= SeparationWeight;
			alignment *= AlignmentWeight;
			cohesion *= CohesionWeight;
			
			ApplyForce(separation);
			ApplyForce(alignment);
			ApplyForce(cohesion);
		}

		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		
		if (player != null)
		{
			Vector2 seekForce = Seek(player.GlobalPosition) * SeekWeight;
			ApplyForce(seekForce);
		}
		
		velocity += acceleration * (float)delta;
		velocity = LimitVector(velocity, MaxSpeed);
		GlobalPosition += velocity * (float)delta;
		
		WrapScreen();
		
		if (velocity.LengthSquared() > 0)
		{
			Rotation = velocity.Angle();
		}
	}
	
	private Vector2 Separation(List<Boid> neighbors)
	{
		Vector2 steer = Vector2.Zero;

		int count = 0;
		
		foreach (var other in neighbors)
		{
			float distance = GlobalPosition.DistanceTo(other.GlobalPosition);

			if (distance > 0 && distance < SeparationRadius)
			{
				Vector2 diff = GlobalPosition - other.GlobalPosition;
				diff = diff.Normalized() / distance;
				steer += diff;
				count++;
			}
		}
		
		if (count > 0)
		{
			steer /= count;
			steer = steer.Normalized() * MaxSpeed;
			steer -= velocity;
			steer = LimitVector(steer, MaxForce);
		}
		
		return steer;
	}
	
	private Vector2 Alignment(List<Boid> neighbors)
	{
		Vector2 sum = Vector2.Zero;

		int count = 0;
		
		foreach (var other in neighbors)
		{
			float distance = GlobalPosition.DistanceTo(other.GlobalPosition);

			if (distance > 0 && distance < AlignmentRadius)
			{
				sum += other.velocity;
				count++;
			}
		}
		
		if (count > 0)
		{
			sum /= count;
			sum = sum.Normalized() * MaxSpeed;
			Vector2 steer = sum - velocity;
			return LimitVector(steer, MaxForce);
		}
		
		return Vector2.Zero;
	}
	
	private Vector2 Cohesion(List<Boid> neighbors)
	{
		Vector2 sum = Vector2.Zero;

		int count = 0;
		
		foreach (var other in neighbors)
		{
			float distance = GlobalPosition.DistanceTo(other.GlobalPosition);

			if (distance > 0 && distance < CohesionRadius)
			{
				sum += other.GlobalPosition;
				count++;
			}
		}
		
		if (count > 0)
		{
			sum /= count;
			return Seek(sum);
		}
		
		return Vector2.Zero;
	}
	
	private Vector2 Seek(Vector2 target)
	{
		Vector2 desired = target - GlobalPosition;
		desired = desired.Normalized() * MaxSpeed;
		Vector2 steer = desired - velocity;
		return LimitVector(steer, MaxForce);
	}
	
	private void ApplyForce(Vector2 force)
	{
		acceleration += force;
	}
	
	private List<Boid> GetNearbyBoids()
	{
		var neighbors = new List<Boid>();
		var boids = GetTree().GetNodesInGroup("boids");
		
		foreach (var node in boids)
		{
			if (node is Boid other && other != this)
			{
				float distance = GlobalPosition.DistanceTo(other.GlobalPosition);

				if (distance < Mathf.Max(SeparationRadius, Mathf.Max(AlignmentRadius, CohesionRadius)))
				{
					neighbors.Add(other);
				}
			}
		}
		
		return neighbors;
	}
	
	private Vector2 LimitVector(Vector2 v, float max)
	{
		if (v.LengthSquared() > max * max)
		{
			return v.Normalized() * max;
		}
		
		return v;
	}
	
	private void WrapScreen()
	{
		var screenSize = GetViewportRect().Size;
		var pos = GlobalPosition;
		
		if (pos.X < 0) pos.X = screenSize.X;
		if (pos.X > screenSize.X) pos.X = 0;
		if (pos.Y < 0) pos.Y = screenSize.Y;
		if (pos.Y > screenSize.Y) pos.Y = 0;
		
		GlobalPosition = pos;
	}
}
