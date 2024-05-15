using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Line
{
    public static int length = 200;
    public static int width = 10;
    public Vector2 Position { get; set; }
    private Texture2D _texture;
    private float _rotation;
    private Rectangle _pieceShape = new Rectangle(0, 0, length, width);
    private Color _color;

    public Line(Vector2 position, float rotation, Texture2D texture, Color color)
    {
        Position = new Vector2(position.X - length / 2, position.Y - width / 2);
        _rotation = rotation;
        _texture = texture;
        _color = color;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _pieceShape, _color, _rotation, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }

    public void UpdatePosition(float dx)
    {
        Position = new Vector2(Position.X - dx, Position.Y);
    }
}