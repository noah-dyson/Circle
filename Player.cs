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
    public Vector2[] verticesTop = new Vector2[4];
    public Vector2[] verticesBottom = new Vector2[4];
    public Vector2[] axis = new Vector2[2];
    public float velocity = 0;
    public float gravity = 0.3f;
    public float terminalVelocity = 2f;

    public Player(Vector2 position, Texture2D textureFront, Texture2D textureBack, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        _textureFront = textureFront;
        _textureBack = textureBack;
        _color = color;

        generatevertices();
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
            if (velocity < terminalVelocity)
            {
                velocity += gravity;
            }
            Position = new Vector2(Position.X, Position.Y + velocity);
            for (int i = 0; i < 4; i++)
            {
                verticesTop[i] = new Vector2(verticesTop[i].X, verticesTop[i].Y + velocity);
            }
        }
    }

    public void generatevertices()
    {
        verticesTop[0] = new Vector2(Position.X, Position.Y);
        verticesTop[1] = new Vector2(Position.X + _textureBack.Width*2*_scale, Position.Y);
        verticesTop[2] = new Vector2(Position.X + _textureBack.Width*2*_scale, Position.Y + 5);
        verticesTop[3] = new Vector2(Position.X, Position.Y + 5);

        verticesBottom[0] = new Vector2(Position.X, Position.Y + _textureBack.Height*_scale - 5);
        verticesBottom[1] = new Vector2(Position.X + _textureBack.Width*2*_scale, Position.Y + _textureBack.Height*_scale - 5);
        verticesBottom[2] = new Vector2(Position.X + _textureBack.Width*2*_scale, Position.Y + _textureBack.Height*_scale);
        verticesBottom[3] = new Vector2(Position.X, Position.Y + _textureBack.Height*_scale);
    }

    public void generateAxis()
    {
        Vector2 edge = verticesTop[1] - verticesTop[0];
        axis[0] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
        edge = verticesTop[2] - verticesTop[1];
        axis[1] = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
    }

    public void jump()
    {
        if (!colliding)
        {
            velocity = -2;
        }
    }
}

