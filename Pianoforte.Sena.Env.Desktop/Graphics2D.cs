using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pianoforte.Sena.Env.Desktop
{
  public class Graphics2D : SpriteBatch
  {
    private readonly Texture2D dotTexture;
    public Graphics2D(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
      dotTexture = new Texture2D(graphicsDevice, 1, 1);
      dotTexture.SetData(new[] { Color.White });
    }

    public void DrawRectangle(Rectangle rect, Color color)
      => DrawRectangle(rect, color, 0, 0);
    public void DrawRectangle(Rectangle rect, Color color, float rotation, float layerDepth)
      => DrawRectangle(rect, color, rotation, SpriteEffects.None, layerDepth);
    public void DrawRectangle(Rectangle rect, Color color, float rotation, SpriteEffects effects, float layerDepth)
      => Draw(dotTexture, rect, rect, color, rotation, Vector2.Zero, effects, layerDepth);
  }
}
