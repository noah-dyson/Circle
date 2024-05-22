using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Line
{
    public static int length = 100;
    public static int width = 10;
    public Vector2 Position { get; set; }
    private Texture2D _texture;
    public float Rotation { get; set; }
    private Rectangle _pieceShape = new Rectangle(0, 0, length, width);
    private Color _color;

    public Vector2[] vertices = new Vector2[4];
    public Vector2[] axis = new Vector2[2];
    private Matrix _transform;

    public Line(Vector2 position, float rotation, Texture2D texture, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        Rotation = rotation;
        _texture = texture;
        _color = color;

        _transform = Matrix.CreateRotationZ(Rotation);
        generateVertices();
        generateAxis();
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _pieceShape, _color, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
    }

    public void UpdatePosition(float dx)
    {
        Position = new Vector2(Position.X - dx, Position.Y);
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = new Vector2(vertices[i].X - dx, vertices[i].Y);
        }
    }

    public void generateVertices()
    {
        vertices[0] = new Vector2(Position.X, Position.Y);
        vertices[1] = new Vector2(Position.X + length, Position.Y);
        vertices[2] = new Vector2(Position.X + length, Position.Y + width);
        vertices[3] = new Vector2(Position.X, Position.Y + width);

        for (int i = 0; i < 4; i++)
        {
            Vector2 temp = vertices[i] - vertices[0];
            temp = Vector2.Transform(temp, _transform);
            vertices[i] = temp + vertices[0];
        }
    }

    public void generateAxis()
    {
        Vector2 edge = vertices[1] - vertices[0];
        axis[0] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
        edge = vertices[2] - vertices[1];
        axis[1] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
    }


}

