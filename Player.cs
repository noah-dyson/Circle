using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Player
{
    public Vector2 Position { get; set; }
    private Texture2D _textureFront;
    private Texture2D _textureBack;
    private Color _color;
    private float _scale = 0.1f;
    public bool colliding = false;
    public Vector2[] vertices = new Vector2[4];
    public Vector2[] axis = new Vector2[2];

    public Player(Vector2 position, Texture2D textureFront, Texture2D textureBack, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        _textureFront = textureFront;
        _textureBack = textureBack;
        _color = color;

        generateVertices();
        generateAxis();
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_textureBack, Position, null, _color, 0, Vector2.Zero, 0.1f, SpriteEffects.None, 0);
        spriteBatch.Draw(_textureFront, new Vector2(Position.X + _textureBack.Width * _scale, Position.Y), null, _color, 0, Vector2.Zero, _scale, SpriteEffects.None, 0.2f);
    }

    public void UpdatePosition()
    {
        if (!colliding)
        {
            Position = new Vector2(Position.X, Position.Y + 1f);
            for (int i = 0; i < 4; i++)
            {
                vertices[i] = new Vector2(vertices[i].X, vertices[i].Y + 1f);
            }
        }
    }

    public void generateVertices()
    {
        vertices[0] = new Vector2(Position.X, Position.Y);
        vertices[1] = new Vector2(Position.X + _textureBack.Width*2*_scale, Position.Y);
        vertices[2] = new Vector2(Position.X + _textureBack.Width*2*_scale, Position.Y + 5);
        vertices[3] = new Vector2(Position.X, Position.Y + 5);
    }

    public void generateAxis()
    {
        Vector2 edge = vertices[1] - vertices[0];
        axis[0] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
        edge = vertices[2] - vertices[1];
        axis[1] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
    }
}

