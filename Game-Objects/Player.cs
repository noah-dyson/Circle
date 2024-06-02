using System;
using System.Runtime.CompilerServices;
using Circle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Player
{
    public static float Scale = 0.15f;
    private static float _velocity = 0;
    private static float _gravity = 0.1f;
    private static float _terminalVelocity = 4f;

    public Vector2 Position;
    public Color Color;
    public bool Colliding = false;
    
    private Texture2D _textureFront;
    private Texture2D _textureBack;

    public Vector2[] VerticesTop = new Vector2[4];
    public Vector2[] VerticesBottom = new Vector2[4];
    public Vector2[] Axis = new Vector2[2];

    private float _startingY;
    private float _elapsedTime = 0f;

    public Player(Vector2 position, Texture2D textureFront, Texture2D textureBack, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        _startingY = position.Y;
        _textureFront = textureFront;
        _textureBack = textureBack;
        Color = color;

        Axis[0] = new Vector2(1, 0);
        Axis[1] = new Vector2(0, 1);
    }

    public void Render(SpriteBatch spriteBatch)
    {
        // draw the player in two parts, the back and the front
        // as the back part is behind the line and the front part is in front of it
        spriteBatch.Draw(_textureBack, Position, null, Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        spriteBatch.Draw(_textureFront, new Vector2(Position.X + _textureBack.Width * Scale, Position.Y), null, Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.2f);
    }

    public void UpdatePosition(bool running, GameTime gameTime)
    {
        if (!Colliding && running)
        {
            // ensures when the game first begins, the buonding box vertices match the player's current position
            if (VerticesTop[0] == Vector2.Zero)
            {
                generatevertices();
            }

            // apply gravity to the player
            if (_velocity < _terminalVelocity)
            {
                _velocity += _gravity;
            }

            // update the player's position and bounding box vertices
            Position = new Vector2(Position.X, Position.Y + _velocity);
            for (int i = 0; i < 4; i++)
            {
                VerticesTop[i] = new Vector2(VerticesTop[i].X, VerticesTop[i].Y + _velocity);
                VerticesBottom[i] = new Vector2(VerticesBottom[i].X, VerticesBottom[i].Y + _velocity);
            }
        }
        else if (!running)
        {
            // if the game is not running, make the player bounce up and down using a sine wave
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float timePeriod = 1f;
            float amplitude = 10f;
            Position = new Vector2(Position.X, _startingY + (float)Math.Sin(_elapsedTime/timePeriod * 2 * Math.PI) * amplitude);
        }
    }

    public void generatevertices()
    {
        // generate the bounding box vertices for the player
        VerticesTop[0] = new Vector2(Position.X + _textureBack.Width/10*9*Scale, Position.Y);
        VerticesTop[1] = new Vector2(VerticesTop[0].X + _textureBack.Width/10*2*Scale, Position.Y);
        VerticesTop[2] = new Vector2(VerticesTop[1].X, Position.Y + 50*Scale);
        VerticesTop[3] = new Vector2(VerticesTop[0].X, Position.Y + 50*Scale);

        VerticesBottom[0] = new Vector2(Position.X + _textureBack.Width/10*9*Scale, Position.Y + _textureBack.Height*Scale - 50*Scale);
        VerticesBottom[1] = new Vector2(VerticesBottom[0].X + _textureBack.Width/10*2*Scale, VerticesBottom[0].Y);
        VerticesBottom[2] = new Vector2(VerticesBottom[1].X, Position.Y + _textureBack.Height*Scale);
        VerticesBottom[3] = new Vector2(VerticesBottom[0].X, Position.Y + _textureBack.Height*Scale);
    }

    public void jump()
    {
        if (!Colliding)
        {
            _velocity = -2;
        }
    }
}

