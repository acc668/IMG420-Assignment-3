using Godot;
using System;

public partial class FlockManager : Node2D
{
    [Export] public int InitialBoidCount = 20;
    [Export] public int BoidsPerWave = 10;
    [Export] public Vector2 SpawnAreaMin = new Vector2(50, 50);
    [Export] public Vector2 SpawnAreaMax = new Vector2(1230, 670);
    
    private Node2D player;
    private float waveTimer = 0.0f;
    private float waveInterval = 20.0f;
    private int currentWave = 0;

    public override void _Ready()
    {
        player = GetTree().Root.GetNode<Node2D>("Game/Player");

        SpawnFlock(InitialBoidCount);
    }

    public override void _Process(double delta)
    {
        waveTimer += (float)delta;
        
        if (waveTimer >= waveInterval)
        {
            waveTimer = 0.0f;

            currentWave++;

            SpawnFlock(BoidsPerWave);

            var game = GetTree().Root.GetNode<Game>("Game");
            
            if (game != null)
            {
                game.ShowWaveMessage(currentWave + 1);
            }
        }
    }

    public void SpawnFlock(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var boid = new Boid();
            
            var spawnPos = new Vector2(
                (float)GD.RandRange(SpawnAreaMin.X, SpawnAreaMax.X),
                (float)GD.RandRange(SpawnAreaMin.Y, SpawnAreaMax.Y));

            boid.Position = spawnPos;
            
            boid.SetBoundaries(SpawnAreaMin, SpawnAreaMax);

            boid.SetPlayer(player);

            boid.AddToGroup("boids");
            
            AddChild(boid);
        }
    }

    public void ClearAllBoids()
    {
        var boids = GetTree().GetNodesInGroup("boids");

        foreach (var node in boids)
        {
            if (node is Node godotNode)
            {
                godotNode.QueueFree();
            }
        }
    }

    public int GetBoidCount()
    {
        return GetTree().GetNodesInGroup("boids").Count;
    }
}