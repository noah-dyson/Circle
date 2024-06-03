using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circle
{

    // probably not necessary to use handlers in such a small project but I thought they were kinda cool
    public delegate void RestartHandler();
    public delegate void JumpHandler();

    // keeps track of the game state
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
        private SoundManager _soundManager;

        private Random _lineTypeGen = new Random();
        private Random _colorGen = new Random();
        public Color BackgroundColor = Color.CornflowerBlue;

        public event JumpHandler OnJump;
        public event RestartHandler OnRestart;

        private KeyboardState _previousKeyboardState;
        private CircleState _state = CircleState.Pregame;
        private float _lineSpeed = 0;


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

            // adding the first lines to the game
            for (int i = 0; i < _lineLength; i++)
            {
                Line line = new Line(new Vector2(Line.length * i, _screenHeight / 2), 0, _line);
                _lines.AddLast(line);
            }


            // creating the player and the scores
            _player = new Player(new Vector2(_screenWidth / 2 - _playerFront.Width * Player.Scale, _screenHeight / 2 - _playerFront.Height * Player.Scale / 2), _playerFront, _playerBack, Color.White);
            _score = new Score(font);
            _score.LoadHighScore();

            _soundManager = new SoundManager();
            _soundManager.LoadContent(_contentManager);
            OnJump += _player.jump;
            OnJump += _soundManager.PlayJumpSound;

            _previousKeyboardState = Keyboard.GetState();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // what to do when the space key is pressed depending on the game state
            if (currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
            {
                switch (_state)
                {
                    case CircleState.Pregame:

                        // starts the game
                        _lineSpeed = _startLineSpeed;
                        _state = CircleState.Ingame;
                        OnJump?.Invoke();
                        OnJump += _score.Increment;
                        break;

                    case CircleState.Ingame:
                        OnJump?.Invoke();

                        // changes the color and line speed of the game every 10 points
                        if (_score.Value % 10 == 0)
                        {
                            _lineSpeed += 0.25f;
                            BackgroundColor = new Color(_colorGen.Next(0, 255), _colorGen.Next(0, 255), _colorGen.Next(0, 255));
                        }
                        break;

                    case CircleState.Postgame:

                        OnRestart?.Invoke();
                        break;
                }
            }
            _previousKeyboardState = currentKeyboardState;

            // updates the game depending on the game state
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
            // draws the game objects
            foreach (Line line in _lines)
            {
                line.Render(spriteBatch);
            }

            _player.Render(spriteBatch);
            _score.Render(spriteBatch, _screenWidth);
        }

        private void UpdatePositions(GameTime gameTime)
        {
            // updates the position of the lines and the player
            foreach (Line line in _lines)
            {
                line.UpdatePosition(_lineSpeed);
            }
            _player.UpdatePosition(true, gameTime);
        }

        private void StartGameUpdate(GameTime gameTime)
        {
            // moves the player up and down
            _player.UpdatePosition(false, gameTime);
        }

        private void InGameUpdate(GameTime gameTime)
        {
            // checks which lines are close enough to the player to be possible collisions
            List<Line> possibleCollisions = Collisions.SortAndSweep(_player, _lines);

            // checks if either of the players collision boxes are colliding with any of the possible collisions
            _player.Colliding = false;
            foreach (Line line in possibleCollisions)
            {
                if (Collisions.SeparatingAxisCollision(line, _player, _player.VerticesTop))
                {
                    _player.Colliding = true;
                    break;
                }
                else if (Collisions.SeparatingAxisCollision(line, _player, _player.VerticesBottom))
                {
                    _player.Colliding = true;
                    break;
                }
            }

            // if the player is colliding with a line the game is over
            if (_player.Colliding)
            {
                _state = CircleState.Postgame;
                if (_score.Value > _score.HighScore)
                {
                    _score.HighScore = _score.Value;
                    _score.SaveHighScore();
                }
            }

            // updates the positions of the lines and the player
            UpdatePositions(gameTime);

            // removes the first line if it is off the screen and adds a new line
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

        // this method is very long and could probably be condensed if my maths was better but alas this is the abomination I have created
        private void AddNextLine()
        {

            // decides whether the line is flat, going up or going down
            int _lineType = _lineTypeGen.Next(0, 3);
            float rotation = 0;
            switch (_lineType)
            {
                case 0:
                    rotation = 0;
                    break;
                case 1:
                    rotation = -(float)_lineTypeGen.NextDouble() * MathHelper.Pi / 6;
                    break;
                case 2:
                    rotation = (float)_lineTypeGen.NextDouble() * MathHelper.Pi / 6;
                    break;
            }

            Line lastLine = _lines.Last.Value;

            // default position of the next line
            float xPos = _lines.Last.Value.Position.X + Line.length * (float)Math.Cos(_lines.Last.Value.Rotation);
            float yPos = _lines.Last.Value.Position.Y + Line.length * (float)Math.Sin(_lines.Last.Value.Rotation);

            // makes sure the lines don't go off the screen
            if (yPos > 380)
            {
                rotation = -(float)_lineTypeGen.NextDouble() * MathHelper.Pi / 6;
            }
            else if (yPos < 100)
            {
                rotation = (float)_lineTypeGen.NextDouble() * MathHelper.Pi / 6;
            }

            // calculates the bottom right hand corner of the last line segment
            Vector2 bottomCorner = new Vector2(0, 0);
            if (lastLine.Rotation == 0)
            {
                bottomCorner = new Vector2(lastLine.Position.X + Line.length, lastLine.Position.Y + Line.width);
            }
            else if (lastLine.Rotation < 0)
            {
                bottomCorner = new Vector2(lastLine.Position.X + Line.width * (float)Math.Sin(-lastLine.Rotation) + Line.length * (float)Math.Cos(lastLine.Rotation), lastLine.Position.Y + Line.width * (float)Math.Cos(lastLine.Rotation) - Line.length * (float)Math.Sin(-lastLine.Rotation));
            }
            else if (lastLine.Rotation > 0)
            {
                bottomCorner = new Vector2(lastLine.Position.X + Line.length * (float)Math.Cos(lastLine.Rotation) - Line.width * (float)Math.Sin(lastLine.Rotation), lastLine.Position.Y + Line.length * (float)Math.Sin(lastLine.Rotation) + Line.width * (float)Math.Cos(lastLine.Rotation));
            }

            // calculates the position of the next line segment based of the last line segment's bottom right hand corner
            if (lastLine.Rotation <= 0 && rotation < 0 && lastLine.Rotation - rotation > 0)
            {
                xPos = bottomCorner.X - Line.width * (float)Math.Sin(-rotation);
                yPos = bottomCorner.Y - Line.width * (float)Math.Cos(rotation);
            }
            else if (lastLine.Rotation > 0 && rotation >= 0 && lastLine.Rotation - rotation > 0)
            {
                xPos = bottomCorner.X + Line.width * (float)Math.Sin(rotation);
                yPos = bottomCorner.Y - Line.width * (float)Math.Cos(rotation);
            }
            else if (lastLine.Rotation > 0 && rotation < 0)
            {
                xPos = bottomCorner.X - Line.width * (float)Math.Sin(-rotation);
                yPos = bottomCorner.Y - Line.width * (float)Math.Cos(rotation);
            }

            // finally adds the line to the list
            _lines.AddLast(new Line(new Vector2(xPos, yPos), rotation, _line));
        }


    }
}