using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circle;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private Random _lineTypeGen = new Random();
    private Texture2D _line;
    private int _screenHeight;
    private int _screenWidth;
    private static int _lineLength = 6;
    private LinkedList<Line> _lines = new LinkedList<Line>();
    private float _lineSpeed = 2f;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _graphicsDevice = _graphics.GraphicsDevice;
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _screenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;
        _screenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;

        _line = Content.Load<Texture2D>("line");
        
        for (int i = 0; i < _lineLength; i++)
        {
            Line line = new Line(new Vector2(Line.length * i, _screenHeight / 2), 0, _line, Color.Gray);
            _lines.AddLast(line);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        foreach (Line line in _lines)
        {
            line.UpdatePosition(_lineSpeed);
        }
        if (_lines.First.Value.Position.X + Line.length <= 0)
        {
            int _lineType = _lineTypeGen.Next(0, 3);
            float rotation = 0;
            switch(_lineType)
            {
                case 0:
                    rotation = 0;
                    break;
                case 1:
                    rotation = -(float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
                    break;
                case 2:
                    rotation = (float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
                    break;
            }

            _lines.RemoveFirst();
            float xPos = 0;
            float yPos = 0;
            if (_lines.Last.Value.Rotation >= 0)
            {
                xPos = _lines.Last.Value.Position.X + Line.length * (float)Math.Cos(_lines.Last.Value.Rotation);
                yPos = _lines.Last.Value.Position.Y + Line.length * (float)Math.Sin(_lines.Last.Value.Rotation);
            }
            else if (_lines.Last.Value.Rotation < 0)
            {
                xPos = _lines.Last.Value.Position.X + Line.length * (float)Math.Cos(_lines.Last.Value.Rotation) - Line.width * (float)Math.Cos(_lines.Last.Value.Rotation);
                yPos = _lines.Last.Value.Position.Y + Line.length * (float)Math.Sin(_lines.Last.Value.Rotation);
            }

            _lines.AddLast(new Line(new Vector2(xPos, yPos), rotation, _line, Color.Red));
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (Line line in _lines)
        {
            line.Render(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
