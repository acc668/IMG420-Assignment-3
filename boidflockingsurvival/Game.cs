using Godot;
using System;

public partial class Game : Node2D
{
    private Player player;
    private FlockManager flockManager;
    private Label scoreLabel;
    private Label livesLabel;
    private Label waveLabel;
    private Label gameOverLabel;
    private Label instructionsLabel;
    
    private int score = 0;
    private int lives = 3;
    private float survivalTime = 0.0f;
    private bool gameOver = false;
    private float invulnerabilityTime = 0.0f;
    private float invulnerabilityDuration = 2.0f;
    private float waveMessageTimer = 0.0f;

    public override void _Ready()
    {
        player = new Player();
        
        AddChild(player);

        flockManager = new FlockManager();
        
        AddChild(flockManager);

        CreateUI();
    }

    private void CreateUI()
    {
        scoreLabel = new Label();
        
        scoreLabel.Position = new Vector2(10, 10);
        
        scoreLabel.AddThemeColorOverride("font_color", Colors.White);
        
        scoreLabel.AddThemeFontSizeOverride("font_size", 24);
        
        scoreLabel.Text = "Score: 0";
        
        AddChild(scoreLabel);

        livesLabel = new Label();
        
        livesLabel.Position = new Vector2(10, 40);
        
        livesLabel.AddThemeColorOverride("font_color", Colors.White);
        
        livesLabel.AddThemeFontSizeOverride("font_size", 24);
        
        livesLabel.Text = "Lives: 3";

        AddChild(livesLabel);

        waveLabel = new Label();
        
        waveLabel.Position = new Vector2(640, 360);
        
        waveLabel.AddThemeColorOverride("font_color", new Color(1, 1, 0));
       
        waveLabel.AddThemeFontSizeOverride("font_size", 48);
        
        waveLabel.Visible = false;
        
        waveLabel.HorizontalAlignment = HorizontalAlignment.Center;
        
        AddChild(waveLabel);

        gameOverLabel = new Label();
        
        gameOverLabel.Position = new Vector2(640, 300);
        
        gameOverLabel.AddThemeColorOverride("font_color", Colors.Red);
        
        gameOverLabel.AddThemeFontSizeOverride("font_size", 64);
        
        gameOverLabel.Text = "GAME OVER";
        
        gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
        
        gameOverLabel.Visible = false;
        
        AddChild(gameOverLabel);

        instructionsLabel = new Label();

        instructionsLabel.Position = new Vector2(10, 650);
        
        instructionsLabel.AddThemeColorOverride("font_color", new Color(0.8f, 0.8f, 0.8f));

        instructionsLabel.AddThemeFontSizeOverride("font_size", 16);
        
        instructionsLabel.Text = "WASD/Arrow Keys: Move | Survive and avoid the red boids! | R: Restart";
        
        AddChild(instructionsLabel);
    }

    public override void _Process(double delta)
    {
        if (gameOver)
        {
            if (Input.IsActionJustPressed("ui_accept") || Input.IsKeyPressed(Key.R))
            {
                RestartGame();
            }

            return;
        }

        survivalTime += (float)delta;

        score = (int)(survivalTime * 10);

        scoreLabel.Text = $"Score: {score}";

        if (invulnerabilityTime > 0)
        {
            invulnerabilityTime -= (float)delta;

            if (player != null && IsInstanceValid(player))
            {
                var sprite = player.GetNode<Sprite2D>("Sprite2D");
                
                if (sprite != null)
                {
                    sprite.Visible = (int)(invulnerabilityTime * 10) % 2 == 0;
                }
            }
        }

        else
        {
            if (player != null && IsInstanceValid(player))
            {
                var sprite = player.GetNode<Sprite2D>("Sprite2D");

                if (sprite != null)
                {
                    sprite.Visible = true;
                }
            }
        }

        if (waveMessageTimer > 0)
        {
            waveMessageTimer -= (float)delta;
            
            if (waveMessageTimer <= 0)
            {
                waveLabel.Visible = false;
            }
        }
    }

    public void OnPlayerHitByBoid()
    {
        if (gameOver || invulnerabilityTime > 0)
        {
            return;
        }

        lives--;

        livesLabel.Text = $"Lives: {lives}";

        if (lives <= 0)
        {
            GameOver();
        }

        else
        {
            invulnerabilityTime = invulnerabilityDuration;
        }
    }

    private void GameOver()
    {
        gameOver = true;

        gameOverLabel.Visible = true;

        var finalScoreLabel = new Label();

        finalScoreLabel.Position = new Vector2(640, 400);

        finalScoreLabel.AddThemeColorOverride("font_color", Colors.White);

        finalScoreLabel.AddThemeFontSizeOverride("font_size", 32);

        finalScoreLabel.Text = $"Final Score: {score}";

        finalScoreLabel.HorizontalAlignment = HorizontalAlignment.Center;
        
        AddChild(finalScoreLabel);

        var restartLabel = new Label();

        restartLabel.Position = new Vector2(640, 450);

        restartLabel.AddThemeColorOverride("font_color", Colors.White);

        restartLabel.AddThemeFontSizeOverride("font_size", 24);
        
        restartLabel.Text = "Press R or Enter to Restart";

        restartLabel.HorizontalAlignment = HorizontalAlignment.Center;

        AddChild(restartLabel);

        if (flockManager != null)
        {
            flockManager.SetProcess(false);
        }
    }

    private void RestartGame()
    {
        GetTree().ReloadCurrentScene();
    }

    public void ShowWaveMessage(int waveNumber)
    {
        waveLabel.Text = $"Wave {waveNumber}!";

        waveLabel.Visible = true;
        
        waveMessageTimer = 2.0f;
    }
}