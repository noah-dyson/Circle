using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Circle;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private Random _lineTypeGen = new Random();
    private Texture2D _line;
    private Player _player;
    private int _screenHeight;
    private int _screenWidth;
    private static int _lineLength = 10;
    private LinkedList<Line> _lines = new LinkedList<Line>();
    private float _lineSpeed = 4f;
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
        Texture2D _playerBack = Content.Load<Texture2D>("back-circle");
        Texture2D _playerFront = Content.Load<Texture2D>("front-circle");
        
        for (int i = 0; i < _lineLength; i++)
        {
            Line line = new Line(new Vector2(Line.length * i, _screenHeight / 2), 0, _line, Color.Gray);
            _lines.AddLast(line);
        }
        
        _player = new Player(new Vector2(100, _screenHeight / 2 - 100), _playerFront, _playerBack, Color.White);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            _player.jump();
        }

        Vector2 axis = new Vector2(1, 0);
        float[] playerProj = { Vector2.Dot(_player.verticesTop[0], axis), Vector2.Dot(_player.verticesTop[1], axis) };
        float playerMin = playerProj.Min();
        float playerMax = playerProj.Max();

        List<Line> collisions = new List<Line>();

        foreach (Line line in _lines)
        {
            float[] lineProj = { Vector2.Dot(line.vertices[0], axis), Vector2.Dot(line.vertices[1], axis), Vector2.Dot(line.vertices[2], axis), Vector2.Dot(line.vertices[3], axis)};
            float lineMin = lineProj.Min();
            float lineMax = lineProj.Max();

            if (lineMax > playerMin && playerMax > lineMin)
            {
                collisions.Add(line);
            }
        }

        _player.colliding = false;
        foreach (Line line in collisions)
        {
            if (Collision(line, _player, _player.verticesTop))
            {
                _player.colliding = true;
                break;
            }
            else if (Collision(line, _player, _player.verticesBottom))
            {
                _player.colliding = true;
                break;
            }
        }

        foreach (Line line in _lines)
        {
            line.UpdatePosition(_lineSpeed);
        }
        _player.UpdatePosition();

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

            Line lastLine = _lines.Last.Value;

            float xPos = _lines.Last.Value.Position.X + Line.length * (float)Math.Cos(_lines.Last.Value.Rotation);
            float yPos = _lines.Last.Value.Position.Y + Line.length * (float)Math.Sin(_lines.Last.Value.Rotation);

            if (yPos > 440)
            {
                rotation = -(float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
            }
            else if (yPos < 40)
            {
                rotation = (float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
            }

            _lines.RemoveFirst();

            Vector2 bottomCorner = new Vector2(0,0);
            if (lastLine.Rotation == 0)
            {
                bottomCorner = new Vector2(lastLine.Position.X + Line.length, lastLine.Position.Y + Line.width);
            }
            else if (lastLine.Rotation < 0)
            {
                bottomCorner = new Vector2(lastLine.Position.X + Line.width*(float)Math.Sin(-lastLine.Rotation) + Line.length*(float)Math.Cos(lastLine.Rotation), lastLine.Position.Y + Line.width*(float)Math.Cos(lastLine.Rotation) - Line.length*(float)Math.Sin(-lastLine.Rotation));
            }
            else if (lastLine.Rotation > 0)
            {
                bottomCorner = new Vector2(lastLine.Position.X + Line.length*(float)Math.Cos(lastLine.Rotation) - Line.width*(float)Math.Sin(lastLine.Rotation), lastLine.Position.Y + Line.length*(float)Math.Sin(lastLine.Rotation) + Line.width*(float)Math.Cos(lastLine.Rotation));
            }

            if (lastLine.Rotation <= 0 && rotation < 0 && lastLine.Rotation - rotation > 0)
            {
                xPos = bottomCorner.X - Line.width*(float)Math.Sin(-rotation);
                yPos = bottomCorner.Y - Line.width*(float)Math.Cos(rotation);
            }
            else if (lastLine.Rotation > 0 && rotation >= 0 && lastLine.Rotation - rotation > 0)
            {
                xPos = bottomCorner.X + Line.width*(float)Math.Sin(rotation);
                yPos = bottomCorner.Y - Line.width*(float)Math.Cos(rotation);
            }
            else if (lastLine.Rotation > 0 && rotation < 0)
            {
                xPos = bottomCorner.X - Line.width*(float)Math.Sin(-rotation);
                yPos = bottomCorner.Y - Line.width*(float)Math.Cos(rotation);
            }

            _lines.AddLast(new Line(new Vector2(xPos, yPos), rotation, _line, Color.Red));
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);

        foreach (Line line in _lines)
        {
            line.Render(_spriteBatch);
        }

        _player.Render(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public static bool Collision(Line line, Player player, Vector2[] partVertices)
    {
        Vector2[] vertices = line.vertices;
        Vector2[] axis = line.axis;
        Vector2[] playerVertices = partVertices;
        Vector2[] playerAxis = player.axis;

        for (int i = 0; i < 2; i++)
        {
            float[] lineProjection = new float[4];
            float[] playerProjection = new float[4];

            for (int j = 0; j < 4; j++)
            {
                lineProjection[j] = Vector2.Dot(vertices[j], axis[i]);
                playerProjection[j] = Vector2.Dot(playerVertices[j], axis[i]);
            }

            float lineMin = lineProjection.Min();
            float lineMax = lineProjection.Max();
            float playerMin = playerProjection.Min();
            float playerMax = playerProjection.Max();

            if (lineMax < playerMin || playerMax < lineMin)
            {
                return false;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            float[] lineProjection = new float[4];
            float[] playerProjection = new float[4];

            for (int j = 0; j < 4; j++)
            {
                lineProjection[j] = Vector2.Dot(vertices[j], playerAxis[i]);
                playerProjection[j] = Vector2.Dot(playerVertices[j], playerAxis[i]);
            }

            float lineMin = lineProjection.Min();
            float lineMax = lineProjection.Max();
            float playerMin = playerProjection.Min();
            float playerMax = playerProjection.Max();

            if (lineMax < playerMin || playerMax < lineMin)
            {
                return false;
            }
        }

        return true;
    }
}
