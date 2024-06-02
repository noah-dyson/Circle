using System;
using System.Runtime.CompilerServices;
using Circle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Player
{
    public Vector2 Position { get; set; }
    private Texture2D _textureFront;
    private Texture2D _textureBack;
    public Color Color;
    public static float Scale = 0.15f;
    public bool colliding = false;
    public Vector2[] verticesTop = new Vector2[4];
    public Vector2[] verticesBottom = new Vector2[4];
    public Vector2[] axis = new Vector2[2];
    public float velocity = 0;
    public float gravity = 0.1f;
    public float terminalVelocity = 4f;
    private float _startingY;
    private int _screenHeight;
    private float _elapsedTime = 0f;

    public Player(Vector2 position, Texture2D textureFront, Texture2D textureBack, Color color, int screenHeight)
    {
        Position = new Vector2(position.X, position.Y);
        _startingY = position.Y;
        _textureFront = textureFront;
        _textureBack = textureBack;
        Color = color;
        _screenHeight = screenHeight;

        axis[0] = new Vector2(1, 0);
        axis[1] = new Vector2(0, 1);
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
        if (!colliding && running)
        {
            // ensures when the game first begins, the buonding box vertices match the player's current position
            if (verticesTop[0] == Vector2.Zero)
            {
                generatevertices();
            }

            // apply gravity to the player
            if (velocity < terminalVelocity)
            {
                velocity += gravity;
            }

            // update the player's position and bounding box vertices
            Position = new Vector2(Position.X, Position.Y + velocity);
            for (int i = 0; i < 4; i++)
            {
                verticesTop[i] = new Vector2(verticesTop[i].X, verticesTop[i].Y + velocity);
                verticesBottom[i] = new Vector2(verticesBottom[i].X, verticesBottom[i].Y + velocity);
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
        verticesTop[0] = new Vector2(Position.X + _textureBack.Width/10*9*Scale, Position.Y);
        verticesTop[1] = new Vector2(verticesTop[0].X + _textureBack.Width/10*2*Scale, Position.Y);
        verticesTop[2] = new Vector2(verticesTop[1].X, Position.Y + 50*Scale);
        verticesTop[3] = new Vector2(verticesTop[0].X, Position.Y + 50*Scale);

        verticesBottom[0] = new Vector2(Position.X + _textureBack.Width/10*9*Scale, Position.Y + _textureBack.Height*Scale - 50*Scale);
        verticesBottom[1] = new Vector2(verticesBottom[0].X + _textureBack.Width/10*2*Scale, verticesBottom[0].Y);
        verticesBottom[2] = new Vector2(verticesBottom[1].X, Position.Y + _textureBack.Height*Scale);
        verticesBottom[3] = new Vector2(verticesBottom[0].X, Position.Y + _textureBack.Height*Scale);
    }

    public void jump()
    {
        if (!colliding)
        {
            velocity = -2;
        }
    }
}

