using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Line
{
    private Texture2D _texture;
    private Vector2 _position;
    private float _rotation;
    private Rectangle _pieceShape = new Rectangle(0, 0, 200, 10);

    public Line(Vector2 position, float rotation, Texture2D texture)
    {
        _position = position;
        _rotation = rotation;
        _texture = texture;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, _pieceShape, Color.Gray, _rotation, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }
}