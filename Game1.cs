using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        // start the game with an instantce of MainGameState
        _mainGameState.Initialize(Content, _screenHeight, _screenWidth);
        _mainGameState.LoadContent();
        _mainGameState.OnRestart += Restart;
    }

    protected override void Update(GameTime gameTime)
    {
        _mainGameState.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // changing the background color randomly
        Color bgColor = _mainGameState.bgColor;
        GraphicsDevice.Clear(bgColor);

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
        _mainGameState.OnRestart += Restart;
    }
}
