# Boid Flocking Survival Game

A survival game demonstrating Craig Reynolds' Boids flocking algorithm implemented in Godot 4 with C#.

## Table of Contents
- [Game Overview](#game-overview)
- [Controls](#controls)
- [Technical Implementation](#technical-implementation)
- [Building and Running](#building-and-running)
- [How to Play](#how-to-play)
- [License](#license)

## Game Overview

**Boid Flocking Survival** is a survival game where the player must avoid increasingly large flocks of enemy entities (boids) that exhibit realistic flocking behavior. The player starts with 3 lives and must survive as long as possible while enemy boids chase and swarm around them. New waves of boids spawn every 20 seconds, increasing the difficulty over time.

### Key Features
- **Realistic Flocking Behavior**: Full implementation of Reynolds' three rules
- **Dynamic Difficulty**: Wave-based spawning system
- **Emergent Gameplay**: Each playthrough is unique due to emergent flock patterns
- **Score System**: Points based on survival time
- **Lives & Invulnerability**: 3 lives with temporary invulnerability after hits

## Controls

| Input | Action |
|-------|--------|
| **W** or **Up Arrow** | Move Up |
| **A** or **Left Arrow** | Move Left |
| **S** or **Down Arrow** | Move Down |
| **D** or **Right Arrow** | Move Right |
| **R** or **Enter** | Restart (after game over) |

### Gameplay Tips
- Stay mobile - stationary players are easier targets
- Watch for gaps in the flock to slip through
- Use corners strategically but avoid getting trapped
- During invulnerability, reposition for next wave
- Early waves are easier - use them to build score

## Technical Implementation

### File Structure
```
BoidFlockingSurvival/
├── Boid.cs              # Individual boid behavior and flocking logic
├── FlockManager.cs      # Spawns and manages boid populations
├── Player.cs            # Player movement and collision detection
├── Game.cs              # Game state, scoring, and UI management
├── Game.tscn            # Main game scene
├── project.godot        # Godot project configuration
├── BoidFlocking.csproj  # C# project file
└── README.md            # This file
```

## Building and Running

### Prerequisites

1. **Godot Engine 4.2+ (Mono Version)**
   - Download from: https://godotengine.org/download
   - **CRITICAL**: Must be the ".NET" or "Mono" version with C# support
   - Standard version will NOT work

2. **.NET SDK 6.0 or later**
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version` in terminal

### Setup Instructions

#### Step 1: Create Project Folder
```bash
mkdir BoidFlockingSurvival
cd BoidFlockingSurvival
```

#### Step 2: Add All Files
Copy all provided files into this folder:
- Boid.cs
- FlockManager.cs
- Player.cs
- Game.cs
- Game.tscn
- project.godot
- BoidFlocking.csproj
- README.md

#### Step 3: Open in Godot
1. Launch **Godot (Mono version)**
2. Click **"Import"** in project manager
3. Navigate to your `BoidFlockingSurvival` folder
4. Select `project.godot`
5. Click **"Import & Edit"**

#### Step 4: Build C# Project
1. In Godot editor, go to **Build** menu (top menu bar)
2. Click **Build → Build Project**
3. Wait for compilation (watch bottom panel for progress)
4. Should see "Build succeeded" message

#### Step 5: Run the Game
- Press **F5** or click **Play button (▶️)** at top-right
- Game window opens with:
  - Green circle (player) in center
  - Red triangles (boids) flying around
  - Score/Lives counter top-left
  - Instructions at bottom

### Command Line Alternative
```bash
# Build project
godot --build-solutions --path .

# Run game
godot --path . res://Game.tscn
```

### Troubleshooting

**"Build failed" Error**:
- Verify Godot Mono version installed
- Check .NET SDK: `dotnet --version`
- Try **Build → Rebuild Project**
- Restart Godot

**No Boids Appear**:
- Check console output (bottom panel) for errors
- Verify all .cs files in project folder
- Ensure Game.tscn is set as main scene

**Player Doesn't Move**:
- Check Project → Project Settings → Input Map
- Verify WASD and arrow keys mapped
- Ensure Player.cs script attached

**Performance Issues**:
- Reduce `InitialBoidCount` in FlockManager.cs
- Lower wave spawn count
- Decrease search radii

## How to Play

### Objective
Survive as long as possible while avoiding the red boid swarm. Your score increases based on survival time.

### Game Mechanics

**Player**:
- Green circle
- Controlled by WASD or arrow keys
- Has 3 lives
- 2 seconds of invulnerability after being hit (flashing effect)

**Boids (Enemies)**:
- Red triangles
- Exhibit flocking behavior (separation, alignment, cohesion)
- Attracted to player when within 300 units
- Initial spawn: 20 boids
- Wave spawns: 10 boids every 20 seconds

**Scoring**:
- Points = Survival Time × 10
- Higher score for longer survival
- No score for defeating enemies (survival focus)

**Difficulty Progression**:
- More boids = denser flock
- Harder to find gaps
- Increased collision risk
- Natural escalation without artificial difficulty spikes

### Strategy Tips

1. **Stay Mobile**: Moving targets are harder to hit
2. **Use the Edges**: Corners offer escape routes but can trap you
3. **Predict Movement**: Watch flock patterns to anticipate swarm direction
4. **Invulnerability Window**: After being hit, use 2 seconds to reposition
5. **Create Gaps**: Lead one part of flock away to split formation
6. **Don't Panic**: Smooth, calculated movements better than frantic dodging

**Three Core Rules**:
1. **Separation**: Avoid crowding neighbors (short-range repulsion)
2. **Alignment**: Steer toward average heading of neighbors
3. **Cohesion**: Steer toward average position of neighbors (long-range attraction)

## Future Enhancements

Potential improvements for extended development:

### Gameplay Features
- **Power-ups**: 
  - Repulsion field that pushes boids away
  - Speed boost for quick escapes
  - Shield for temporary invulnerability
- **Multiple Flock Types**:
  - Aggressive red boids (current)
  - Passive blue boids (ambient, non-hostile)
  - Fast purple boids (higher speed, lower cohesion)
- **Obstacles**: Static barriers that boids must avoid
- **Level Design**: Different arenas with varying layouts
- **Boss Waves**: Giant boid with special behaviors

### Visual Enhancements
- Replace procedural shapes with sprite assets
- Particle trails behind boids
- Particle effects on collision
- Screen shake on hit
- Glow/bloom effects for boids
- Animated sprites with wing flapping

### Advanced Flocking
- Predator-prey relationships between flock types
- Formation flying (V-formation for certain boids)
- Leader-following behavior
- Goal-seeking flocks (move toward specific points)
- Environmental awareness (wind, currents affecting movement)

## Known Limitations

1. **Neighbor Search**: O(n²) algorithm not suitable for very large flocks (>100 boids)
2. **2D Only**: Implementation is 2D; Reynolds' original was 3D
3. **No Obstacle Avoidance**: Boids don't avoid static obstacles (only boundaries)
4. **Simple Collision**: Uses circular collision shapes only
5. **Fixed Time Step**: Assumes constant 60 FPS; may behave differently at other frame rates

## Contributing

This is an educational project demonstrating flocking algorithms. Feel free to:
- Modify parameters and observe behavior changes
- Add new features and mechanics
- Optimize the neighbor search algorithm
- Create visual improvements
- Implement additional steering behaviors

## Author

Created as a demonstration of flocking algorithms in game development using Godot 4 and C#.

**Assignment**: Boid Flocking
**Course**: IMG-420: 2D Game Engines

## License

This project is provided as educational material. Free to use and modify for learning purposes.

---

**End of README**

*Last Updated: 2025*
*Version: 1.0*
*Godot Version: 4.2+*
*.NET Version: 6.0+*
