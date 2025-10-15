using Godot;
using System;

public partial class Player : Area2D
{
    [Export] public float Speed = 300.0f;
    
    private Sprite2D sprite;
    private CollisionShape2D collisionShape;
    private Vector2 screenSize;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size;

        sprite = new Sprite2D();
        
        sprite.Texture = CreatePlayerTexture();

        sprite.Modulate = new Color(0.3f, 1.0f, 0.3f);

        AddChild(sprite);

        collisionShape = new CollisionShape2D();

        var shape = new CircleShape2D();

        shape.Radius = 12.0f;

        collisionShape.Shape = shape;

        AddChild(collisionShape);

        AddToGroup("player");

        Position = screenSize / 2;
    }

    private ImageTexture CreatePlayerTexture()
    {
        var image = Image.Create(24, 24, false, Image.Format.Rgba8);
        
        image.Fill(new Color(0, 0, 0, 0));
        
        for (int y = 0; y < 24; y++)
        {
            for (int x = 0; x < 24; x++)
            {
                float dx = x - 12;

                float dy = y - 12;
                
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (distance <= 10.0f)
                {
                    image.SetPixel(x, y, Colors.White);
                }
            }
        }
        
        return ImageTexture.CreateFromImage(image);
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero;

        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed(Key.S))
        {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed(Key.W))
        {
            velocity.Y -= 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
        }

        Position += velocity * (float)delta;

        Position = new Vector2(
            Mathf.Clamp(Position.X, 0, screenSize.X),
            Mathf.Clamp(Position.Y, 0, screenSize.Y));
    }
}