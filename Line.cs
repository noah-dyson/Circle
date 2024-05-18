using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Line
{
    public static int length = 200;
    public static int width = 10;
    public Vector2 Position { get; set; }
    private Texture2D _texture;
    public float Rotation { get; set; }
    private Rectangle _pieceShape = new Rectangle(0, 0, length, width);
    private Color _color;
    public Vector2 bottomRight;

    public Line(Vector2 position, float rotation, Texture2D texture, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        Rotation = rotation;
        _texture = texture;
        _color = color;
        bottomRight = GenCorners();
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _pieceShape, _color, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }

    public void UpdatePosition(float dx)
    {
        Position = new Vector2(Position.X - dx, Position.Y);
        bottomRight = new Vector2(bottomRight.X - dx, bottomRight.Y);
    }

    private Vector2 GenCorners()
    {
        if (Rotation <= 0)
        {
            bottomRight = new Vector2(Position.X + width * (float)Math.Cos(Math.PI/2 - Math.Abs(Rotation)) + length * (float)Math.Cos(Rotation), Position.Y + width * (float)Math.Sin(Math.PI/2 - Math.Abs(Rotation)) - length * (float)Math.Sin(Math.Abs(Rotation)));
        }
        else
        {
            bottomRight = new Vector2(Position.X - width * (float)Math.Sin(Math.Abs(Rotation)) + length * (float)Math.Cos(Rotation), Position.Y + width * (float)Math.Cos(Math.Abs(Rotation)) + length * (float)Math.Sin(Rotation));
        }
        return bottomRight;
    }
}

