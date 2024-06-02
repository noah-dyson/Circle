using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Line
{
    public static int length = 100;
    public static int width = 10;
    private static Color _color = Color.White;

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    private Rectangle _pieceShape = new Rectangle(0, 0, length, width);

    public Vector2[] Vertices = new Vector2[4];
    public Vector2[] Axis = new Vector2[2];
    private Matrix _transform;
    private Texture2D _texture;

    public Line(Vector2 position, float rotation, Texture2D texture)
    {
        Position = new Vector2(position.X, position.Y);
        Rotation = rotation;
        _texture = texture;

        // create a rotation matrix to rotate the line Vertices
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
        // move the line to the left and update the Vertices positions
        Position = new Vector2(Position.X - dx, Position.Y);
        for (int i = 0; i < 4; i++)
        {
            Vertices[i] = new Vector2(Vertices[i].X - dx, Vertices[i].Y);
        }
    }

    public void generateVertices()
    {
        Vertices[0] = new Vector2(Position.X, Position.Y);
        Vertices[1] = new Vector2(Position.X + length, Position.Y);
        Vertices[2] = new Vector2(Position.X + length, Position.Y + width);
        Vertices[3] = new Vector2(Position.X, Position.Y + width);

        // rotate the Vertices
        for (int i = 0; i < 4; i++)
        {
            Vector2 temp = Vertices[i] - Vertices[0];
            temp = Vector2.Transform(temp, _transform);
            Vertices[i] = temp + Vertices[0];
        }
    }

    public void generateAxis()
    {
        // create the Axis of the line used for collision detection
        Vector2 edge = Vertices[1] - Vertices[0];
        Axis[0] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
        edge = Vertices[2] - Vertices[1];
        Axis[1] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
    }


}

