using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Circle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public delegate void RestartHandler();
public delegate void JumpHandler();
enum CircleState
{
    Pregame,
    Ingame,
    Postgame
}

public class MainGameState
{
    private static int _lineLength = 10;
    private static float _startLineSpeed = 3f;

    private ContentManager _contentManager;
    private int _screenHeight;
    private int _screenWidth;
    private Texture2D _line;
    private Player _player;
    private Score _score;
    private LinkedList<Line> _lines = new LinkedList<Line>();
    private Random _lineTypeGen = new Random();
    private Random _colorGen = new Random();
    public Color bgColor = Color.CornflowerBlue;

    private float _lineSpeed = 0;
    public event JumpHandler OnJump;
    public event RestartHandler OnRestart;

    private KeyboardState _previousKeyboardState;
    private CircleState _state = CircleState.Pregame;
    
    
    public void Initialize(ContentManager contentManager, int screenHeight, int screenWidth)
    {
        _contentManager = contentManager;
        _screenHeight = screenHeight;
        _screenWidth = screenWidth;
    }

    public void LoadContent()
    {
        _line = _contentManager.Load<Texture2D>("line");
        Texture2D _playerBack = _contentManager.Load<Texture2D>("back-circle");
        Texture2D _playerFront = _contentManager.Load<Texture2D>("front-circle");
        SpriteFont font = _contentManager.Load<SpriteFont>("Score");

        for (int i = 0; i < _lineLength; i++)
        {
            Line line = new Line(new Vector2(Line.length * i, _screenHeight / 2), 0, _line);
            _lines.AddLast(line);
        }

        _player = new Player(new Vector2(_screenWidth / 2 - _playerFront.Width*Player.Scale, _screenHeight / 2 - _playerFront.Height*Player.Scale / 2), _playerFront, _playerBack, Color.White, _screenHeight);
        _score = new Score(font);
        _score.LoadHighScore();

        OnJump += _player.jump;

        _previousKeyboardState = Keyboard.GetState();
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        if (currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
        {
            switch (_state)
            {
                case CircleState.Pregame:
                    _lineSpeed = _startLineSpeed;
                    _state = CircleState.Ingame;
                    OnJump?.Invoke();
                    OnJump += _score.Increment;
                    break;
                case CircleState.Ingame:
                    OnJump?.Invoke();
                    if (_score.Value % 10 == 0)
                    {
                        _lineSpeed += 0.25f;
                        bgColor = new Color(_colorGen.Next(0, 255), _colorGen.Next(0, 255), _colorGen.Next(0, 255));
                    }
                    break;
                case CircleState.Postgame:
                    OnRestart?.Invoke();
                    break;
            }
        }
        _previousKeyboardState = currentKeyboardState;

        switch (_state)
        {
            case CircleState.Pregame:
                StartGameUpdate(gameTime);
                break;
            case CircleState.Ingame:
                InGameUpdate(gameTime);
                break;
            case CircleState.Postgame:
                PostGameUpdate(gameTime);
                break;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Line line in _lines)
        {
            line.Render(spriteBatch);
        }

        _player.Render(spriteBatch);
        _score.Render(spriteBatch, _screenWidth);
    }

    private void UpdatePositions(GameTime gameTime)
    {
        foreach (Line line in _lines)
        {
            line.UpdatePosition(_lineSpeed);
        }
        _player.UpdatePosition(true, gameTime);
    }

    private void AddNextLine()
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

            if (yPos > 380)
            {
                rotation = -(float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
            }
            else if (yPos < 100)
            {
                rotation = (float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
            }

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

            _lines.AddLast(new Line(new Vector2(xPos, yPos), rotation, _line));
    }

    private void StartGameUpdate(GameTime gameTime)
    {
        _player.UpdatePosition(false, gameTime);
    }

    private void InGameUpdate(GameTime gameTime)
    {
        List<Line> possibleCollisions = Collisions.SortAndSweep(_player, _lines);

        _player.colliding = false;
        foreach (Line line in possibleCollisions)
        {
            if (Collisions.SeparatingAxisCollision(line, _player, _player.verticesTop))
            {
                _player.colliding = true;
                break;
            }
            else if (Collisions.SeparatingAxisCollision(line, _player, _player.verticesBottom))
            {
                _player.colliding = true;
                break;
            }
        }

        if (_player.colliding)
        {
            _state = CircleState.Postgame;
            if (_score.Value > _score.HighScore)
            {
                _score.HighScore = _score.Value;
                _score.SaveHighScore();
            }
        }

        UpdatePositions(gameTime);

        if (_lines.First.Value.Position.X + Line.length <= 0)
        {
            AddNextLine();
            _lines.RemoveFirst();
        }
    }

    private void PostGameUpdate(GameTime gameTime)
    {
        _player.Color = Color.Orange;
    }

}