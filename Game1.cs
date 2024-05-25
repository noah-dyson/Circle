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
    private int _screenHeight;
    private int _screenWidth;
    private MainGameState _mainGameState;
    public Game1(MainGameState mainGameState)
    {
        _mainGameState = mainGameState;
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

        _mainGameState.Initialize(Content, _screenHeight, _screenWidth);
        _mainGameState.LoadContent();
        _mainGameState.OnCollision += Restart;
    }

    protected override void Update(GameTime gameTime)
    {
        _mainGameState.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);

        _mainGameState.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void Restart()
    {
        _mainGameState = new MainGameState();
        _mainGameState.Initialize(Content, _screenHeight, _screenWidth);
        _mainGameState.LoadContent();
        _mainGameState.OnCollision += Restart;
    }
}
