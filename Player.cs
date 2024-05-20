using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Player
{
    public Vector2 Position { get; set; }
    private Texture2D _textureFront;
    private Texture2D _textureBack;
    private Color _color;

    public Player(Vector2 position, Texture2D textureFront, Texture2D textureBack, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        _textureFront = textureFront;
        _textureBack = textureBack;
        _color = color;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_textureBack, Position, null, _color, 0, Vector2.Zero, 0.1f, SpriteEffects.None, 0);
        spriteBatch.Draw(_textureFront, new Vector2(Position.X + 50, Position.Y), null, _color, 0, Vector2.Zero, 0.1f, SpriteEffects.None, 0.2f);
    }

    public void UpdatePosition(float dx)
    {
        Position = new Vector2(Position.X - dx, Position.Y);
    }
}

