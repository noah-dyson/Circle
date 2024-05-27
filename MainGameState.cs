using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Circle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public delegate void CollisionHandler();
public delegate void JumpHandler();

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

    private float _lineSpeed = 0;

    private bool started = false;

    public event CollisionHandler OnCollision;
    public event JumpHandler OnJump;

    private KeyboardState _previousKeyboardState;
    
    
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

        OnJump += _player.jump;
        OnJump += _score.Increment;

        _previousKeyboardState = Keyboard.GetState();
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        if(!started)
        {
            _player.UpdatePosition(false, gameTime);
            if (currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
            {
                _lineSpeed = _startLineSpeed;
                started = true;
                OnJump?.Invoke();
            }
            _previousKeyboardState = currentKeyboardState;

            return;
        }

        if (currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
        {
            OnJump?.Invoke();
        }
        _previousKeyboardState = currentKeyboardState;

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
            OnCollision?.Invoke();
        }

        UpdatePositions(gameTime);

        if (_lines.First.Value.Position.X + Line.length <= 0)
        {
            AddNextLine();
            _lines.RemoveFirst();
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

    private void FailedLevel()
    {
        
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

            if (yPos > 440)
            {
                rotation = -(float)_lineTypeGen.NextDouble()*MathHelper.Pi/6;
            }
            else if (yPos < 40)
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

}