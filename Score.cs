using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Score
{
    public int Value;
    private SpriteFont _font;

    public Score(SpriteFont font)
    {
        Value = 0;
        _font = font;
    }

    public void Increment()
    {
        Value++;
    }

    public void Render(SpriteBatch spriteBatch, int screenWidth)
    {
        spriteBatch.DrawString(_font, Value.ToString(), new Vector2(screenWidth / 2, 10), Color.White);
    }
}