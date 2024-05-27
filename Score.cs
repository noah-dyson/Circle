using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Score
{
    public int Value;
    public int HighScore;
    private SpriteFont _font;
    private static string _fileName = "highscore.txt";

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
        spriteBatch.DrawString(_font, "High Score: " + HighScore.ToString(), new Vector2(10, 10), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
    }

    public void LoadHighScore()
    {
        if(!File.Exists(_fileName))
        {
            // create the file
            using (StreamWriter sw = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
            {
                sw.Write("0");
            }
        }

        using (StreamReader sr = new StreamReader(new FileStream(_fileName, FileMode.Open)))
        {
            HighScore = int.Parse(sr.ReadLine());
        }
    }

    public void SaveHighScore()
    {
        using (StreamWriter sw = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
        {
            sw.Write(HighScore.ToString());
        }
    }
}