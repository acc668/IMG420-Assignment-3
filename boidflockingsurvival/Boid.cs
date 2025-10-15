using Godot;
using System;
using System.Collections.Generic;

public partial class Boid : Area2D
{
    [Export] public float MaxSpeed = 200.0f;
    [Export] public float MaxForce = 100.0f;
    [Export] public float SeparationRadius = 50.0f;
    [Export] public float AlignmentRadius = 75.0f;
    [Export] public float CohesionRadius = 75.0f;
    [Export] public float SeparationWeight = 1.5f;
    [Export] public float AlignmentWeight = 1.0f;
    [Export] public float CohesionWeight = 1.0f;
    [Export] public float AvoidBoundaryWeight = 2.0f;
    [Export] public float ChasePlayerWeight = 0.5f;
    
    private Vector2 velocity;
    private Vector2 acceleration;
    private Sprite2D sprite;
    private CollisionShape2D collisionShape;
    private Vector2 boundaryMin;
    private Vector2 boundaryMax;
    private Node2D player;

    public override void _Ready()
    {
        sprite = new Sprite2D();

        sprite.Texture = CreateBoidTexture();

        sprite.Modulate = new Color(1.0f, 0.3f, 0.3f);

        AddChild(sprite);

        collisionShape = new CollisionShape2D();

        var shape = new CircleShape2D();

        shape.Radius = 8.0f;

        collisionShape.Shape = shape;

        AddChild(collisionShape);

        var angle = (float)GD.RandRange(0, Mathf.Tau);
        
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * MaxSpeed * 0.5f;
        
        acceleration = Vector2.Zero;

        AreaEntered += OnAreaEntered;
    }

    private ImageTexture CreateBoidTexture()
    {
        var image = Image.Create(16, 16, false, Image.Format.Rgba8);
        
        image.Fill(new Color(0, 0, 0, 0));
        
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                if (x >= 2 && x <= 14)
                {
                    float centerY = 8.0f;
                    
                    float halfHeight = (14 - x) * 0.5f;

                    if (y >= centerY - halfHeight && y <= centerY + halfHeight)
                    {
                        image.SetPixel(x, y, Colors.White);
                    }
                }
            }
        }
        
        return ImageTexture.CreateFromImage(image);
    }

    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        boundaryMin = min;

        boundaryMax = max;
    }

    public void SetPlayer(Node2D playerNode)
    {
        player = playerNode;
    }

    public override void _PhysicsProcess(double delta)
    {
        var boids = GetTree().GetNodesInGroup("boids");
        
        var neighbors = new List<Boid>();

        foreach (var node in boids)
        {
            if (node is Boid otherBoid && otherBoid != this)
            {
                neighbors.Add(otherBoid);
            }
        }

        var separation = Separate(neighbors);

        var alignment = Align(neighbors);

        var cohesion = Cohere(neighbors);

        var boundary = AvoidBoundaries();

        var chase = ChasePlayer();

        separation *= SeparationWeight;

        alignment *= AlignmentWeight;

        cohesion *= CohesionWeight;

        boundary *= AvoidBoundaryWeight;

        chase *= ChasePlayerWeight;

        ApplyForce(separation);

        ApplyForce(alignment);

        ApplyForce(cohesion);

        ApplyForce(boundary);

        ApplyForce(chase);

        velocity += acceleration * (float)delta;

        velocity = velocity.Limit(MaxSpeed);

        Position += velocity * (float)delta;

        if (velocity.LengthSquared() > 0)
        {
            Rotation = velocity.Angle();
        }

        acceleration = Vector2.Zero;
    }

    private void ApplyForce(Vector2 force)
    {
        acceleration += force;
    }

    private Vector2 Separate(List<Boid> boids)
    {
        var steer = Vector2.Zero;
        
        int count = 0;

        foreach (var other in boids)
        {
            float distance = Position.DistanceTo(other.Position);
            
            if (distance > 0 && distance < SeparationRadius)
            {
                var diff = Position - other.Position;

                diff = diff.Normalized() / distance;
                
                steer += diff;

                count++;
            }
        }

        if (count > 0)
        {
            steer /= count;
        }

        if (steer.LengthSquared() > 0)
        {
            steer = steer.Normalized() * MaxSpeed;

            steer -= velocity;

            steer = steer.Limit(MaxForce);
        }

        return steer;
    }

    private Vector2 Align(List<Boid> boids)
    {
        var sum = Vector2.Zero;
        
        int count = 0;

        foreach (var other in boids)
        {
            float distance = Position.DistanceTo(other.Position);
            
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

            var steer = sum - velocity;

            steer = steer.Limit(MaxForce);

            return steer;
        }

        return Vector2.Zero;
    }

    private Vector2 Cohere(List<Boid> boids)
    {
        var sum = Vector2.Zero;
        
        int count = 0;

        foreach (var other in boids)
        {
            float distance = Position.DistanceTo(other.Position);
            
            if (distance > 0 && distance < CohesionRadius)
            {
                sum += other.Position;

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
        var desired = target - Position;

        desired = desired.Normalized() * MaxSpeed;

        var steer = desired - velocity;

        steer = steer.Limit(MaxForce);

        return steer;
    }

    private Vector2 AvoidBoundaries()
    {
        var steer = Vector2.Zero;
        
        float margin = 100.0f;

        if (Position.X < boundaryMin.X + margin)
        {
            steer.X = MaxSpeed;
        }

        else if (Position.X > boundaryMax.X - margin)
        {
            steer.X = -MaxSpeed;
        }

        if (Position.Y < boundaryMin.Y + margin)
        {
            steer.Y = MaxSpeed;
        }

        else if (Position.Y > boundaryMax.Y - margin)
        {
            steer.Y = -MaxSpeed;
        }

        if (steer.LengthSquared() > 0)
        {
            steer = steer.Normalized() * MaxSpeed;
            
            steer -= velocity;

            steer = steer.Limit(MaxForce);
        }

        return steer;
    }

    private Vector2 ChasePlayer()
    {
        if (player != null && IsInstanceValid(player))
        {
            float distanceToPlayer = Position.DistanceTo(player.Position);

            if (distanceToPlayer < 300.0f)
            {
                return Seek(player.Position);
            }
        }

        return Vector2.Zero;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("player"))
        {
            var game = GetTree().Root.GetNode<Game>("Game");

            if (game != null)
            {
                game.OnPlayerHitByBoid();
            }
        }
    }
}

public static class Vector2Extensions
{
    public static Vector2 Limit(this Vector2 vector, float max)
    {
        if (vector.LengthSquared() > max * max)
        {
            return vector.Normalized() * max;
        }

        return vector;
    }
}