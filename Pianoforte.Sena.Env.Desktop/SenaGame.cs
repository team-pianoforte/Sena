using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using Pianoforte.Sena.Lang;

namespace Pianoforte.Sena.Env.Desktop
{
  public class SenaGame : Game
  {

    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private readonly Engine engine;
    private readonly string filepath;

    public SenaGame(string filepath)
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
      engine = new Engine(Lang.Runtime.DesktopRuntime.Environment);
      this.filepath = filepath;
    }

    protected override void Initialize()
    {
      // TODO: Add your initialization logic here
      engine.Execute(filepath);
      base.Initialize();
    }

    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();
      if (Mouse.GetState().LeftButton == ButtonState.Pressed)
      {
        System.Console.WriteLine("hello");

      }

      // TODO: Add your update logic here

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // TODO: Add your drawing code here

      base.Draw(gameTime);
    }
  }
}
