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

    public Line(Vector2 position, float rotation, Texture2D texture, Color color)
    {
        Position = new Vector2(position.X, position.Y);
        Rotation = rotation;
        _texture = texture;
        _color = color;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, _pieceShape, _color, Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }

    public void UpdatePosition(float dx)
    {
        Position = new Vector2(Position.X - dx, Position.Y);
    }
}