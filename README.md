# Assignment 3

A simple survival game demonstrating Craig Reynolds' Boids flocking algorithm.

## Game Description

Avoid the red enemy boids that chase you using realistic flocking behavior. Control the blue player triangle with WASD keys and survive as long as possible.

**Controls:**
- W/A/S/D - Move player
- Space - Restart after game over

## Flocking Algorithm Implementation

This project implements the three core rules from Craig Reynolds' Boids algorithm:

### 1. Separation
Boids steer away from nearby neighbors to avoid crowding and collision.

**Implementation:** Each boid calculates the average position of neighbors within the separation radius and steers away from that point. Closer boids have stronger influence.
```csharp
Vector2 diff = GlobalPosition - other.GlobalPosition;
diff = diff.Normalized() / distance; // Weight by distance
```

### 2. Alignment
Boids align their velocity with the average heading of nearby neighbors.

**Implementation:** Each boid averages the velocities of neighbors within the alignment radius and adjusts its own velocity to match.
```csharp
sum += other.velocity;
sum = sum.Normalized() * MaxSpeed;
steer = sum - velocity;
```

### 3. Cohesion
Boids steer towards the average position of nearby neighbors to stay together as a group.

**Implementation:** Each boid calculates the center of mass of neighbors within the cohesion radius and steers towards it.
```csharp
sum += other.GlobalPosition;
centerOfMass = sum / count;
steer towards centerOfMass;
```

## How Flocking Improves the Game

### Game Design
- **Emergent Complexity:** Three simple rules create unpredictable, lifelike group behavior
- **Dynamic Difficulty:** The flock becomes more dangerous when cohesive and easier when spread out
- **Replayability:** Each game feels different due to emergent patterns

### Feel and Experience
- **Natural Movement:** Enemies don't move robotically - they feel alive
- **Visual Appeal:** The flowing, organic movement is engaging to watch
- **Fair Challenge:** Players can predict general flock behavior but not exact movements

## Technical Details

**Files:**
- `Boid.cs` - Flocking algorithm with all three rules
- `Player.cs` - Player movement and collision detection
- `Main.tscn` - Main game scene with 15 boids

**Parameters:**
- Separation Radius: 50 units
- Alignment Radius: 75 units
- Cohesion Radius: 75 units
- Max Speed: 150
- Max Force: 5

## Running the Project

1. Open project in Godot 4.2+ (Mono version)
2. Build → Build Solution
3. Press F5 to run

## References

- [Boids Algorithm - Wikipedia](https://en.wikipedia.org/wiki/Boids)
- [Craig Reynolds' Original Paper](http://www.red3d.com/cwr/boids/)

