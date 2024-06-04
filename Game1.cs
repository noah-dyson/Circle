using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circle;

public class Game1 : Game
{
    private Rectangle _screenRectangle = new Rectangle(0, 0, 3*800, 3*480);
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private int _screenHeight;
    private int _screenWidth;
    private RenderTarget2D _renderTarget;
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

        _screenHeight = 480;
        _screenWidth = 800;
        
        _graphics.PreferredBackBufferHeight = 3*480;
        _graphics.PreferredBackBufferWidth = 3*800;
        _graphics.ApplyChanges();

        _renderTarget = new RenderTarget2D(_graphicsDevice, _screenWidth, _screenHeight);

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
        Color bgColor = _mainGameState.BackgroundColor;
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(bgColor);

        // create transfrmation matrix
        Matrix viewMatrix = Matrix.CreateTranslation(0, _mainGameState.CameraPos.Y, 0);
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, viewMatrix);
        _mainGameState.DrawObjects(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin();
        _mainGameState.DrawUI(_spriteBatch);
        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTarget, _screenRectangle, Color.White);
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
