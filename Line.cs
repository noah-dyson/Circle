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
    public corners Corners;

    public Line(Vector2 position, float rotation, Texture2D texture, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        Rotation = rotation;
        _texture = texture;
        _color = color;
        Corners = GenCorners();
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _pieceShape, _color, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }

    public void UpdatePosition(float dx)
    {
        Position = new Vector2(Position.X - dx, Position.Y);
        Corners.topLeft = new Vector2(Corners.topLeft.X -dx, Corners.topLeft.Y);
        Corners.topRight = new Vector2(Corners.topRight.X -dx, Corners.topRight.Y);
        Corners.bottomLeft = new Vector2(Corners.bottomLeft.X -dx, Corners.bottomLeft.Y);
        Corners.bottomRight = new Vector2(Corners.bottomRight.X -dx, Corners.bottomRight.Y);
    }

    private corners GenCorners()
    {
        corners corner = new corners();
        corner.topLeft = Position;

        if (Rotation < 0) {
            corner.topRight = new Vector2(Position.X + length*(float)Math.Cos(Rotation), Position.Y - length*(float)Math.Sin(Rotation));
            corner.bottomLeft = new Vector2(Position.X + width*(float)Math.Cos(Math.PI/2-Rotation), Position.Y + width*(float)Math.Cos(Math.PI/2-Rotation));
        } else {
            corner.topRight = new Vector2(Position.X + length*(float)Math.Cos(Rotation), Position.Y + length*(float)Math.Sin(Rotation));
            corner.bottomLeft = new Vector2(Position.X - width*(float)Math.Cos(Math.PI/2+Rotation), Position.Y - width*(float)Math.Cos(Math.PI/2+Rotation));
        }
        
        corner.bottomRight = new Vector2(Position.X + length, Position.Y + width);
        return corner;
    }
}

struct corners
{
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
}