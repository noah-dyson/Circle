using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class MainGameState
{
    private static int _lineLength = 10;
    private static float _lineSpeed = 4f;

    private ContentManager _contentManager;
    private Texture2D _line;
    private Player _player;
    private LinkedList<Line> _lines = new LinkedList<Line>();
    private Random _lineTypeGen = new Random();
    
    
    public void Initialize(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    public void LoadContent()
    {
        Texture2D _line = _contentManager.Load<Texture2D>("line");
        Texture2D _playerBack = _contentManager.Load<Texture2D>("back-circle");
        Texture2D _playerFront = _contentManager.Load<Texture2D>("front-circle");

        for (int i = 0; i < _lineLength; i++)
        {
            Line line = new Line(new Microsoft.Xna.Framework.Vector2(Line.length * i, 720 / 2), 0, _line);
            _lines.AddLast(line);
        }

        _player = new Player(new Microsoft.Xna.Framework.Vector2(100, 720 / 2 - 100), _playerFront, _playerBack, Microsoft.Xna.Framework.Color.White);
    }

    public void Update(GameTime gameTime)
    {
        // handle input with events

        _player.UpdatePosition();
        foreach (var line in _lines)
        {
            line.UpdatePosition(_lineSpeed);
        }

        List<Line> possibleCollisions = SortAndSweep();

        foreach (Line line in possibleCollisions)
        {
            if (CheckCollision(line, _player, _player.verticesTop))
            {
                _player.colliding = true;
                break;
            }
            else if (CheckCollision(line, _player, _player.verticesBottom))
            {
                _player.colliding = true;
                break;
            }
            else
            {
                _player.colliding = false;
            }
        }

        UpdatePositions();

        if (_lines.First.Value.Position.X + Line.length <= 0)
        {
            AddNextLine();
            _lines.RemoveFirst();
        }
    }

    private List<Line> SortAndSweep()
    {
        Vector2 axis = new Vector2(1, 0);
        float[] playerProj = { Vector2.Dot(_player.verticesTop[0], axis), Vector2.Dot(_player.verticesTop[1], axis) };
        float playerMin = playerProj.Min();
        float playerMax = playerProj.Max();

        List<Line> possibleCollisions = new List<Line>();

        foreach (Line line in _lines)
        {
            float[] lineProj = { Vector2.Dot(line.vertices[0], axis), Vector2.Dot(line.vertices[1], axis), Vector2.Dot(line.vertices[2], axis), Vector2.Dot(line.vertices[3], axis)};
            float lineMin = lineProj.Min();
            float lineMax = lineProj.Max();

            if (lineMax > playerMin && playerMax > lineMin)
            {
                possibleCollisions.Add(line);
            }
        }

        return possibleCollisions;
    }

    private bool CheckCollision(Line line, Player player, Vector2[] partVertices)
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

    private void UpdatePositions()
    {
        foreach (Line line in _lines)
        {
            line.UpdatePosition(_lineSpeed);
        }
        _player.UpdatePosition();
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

            _lines.AddLast(new Line(new Vector2(xPos, yPos), rotation, _line));
    }

}