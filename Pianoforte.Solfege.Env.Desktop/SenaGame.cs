using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using Pianoforte.Solfege.Lang;

namespace Pianoforte.Solfege.Env.Desktop
{
  public class SenaGame : Game
  {

    private readonly GraphicsDeviceManager graphics;
    private Graphics2D spriteBatch;

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
      spriteBatch = new Graphics2D(GraphicsDevice);

      // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // TODO: Add your update logic here

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      spriteBatch.Begin();
      spriteBatch.DrawRectangle(new Rectangle(0, 0, 10, 10), Color.Red);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
