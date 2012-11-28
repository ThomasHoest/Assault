using Assault.TheGame.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Assault.TheGame
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class Assault : Game
  {
    GraphicsDeviceManager m_graphics;
    SpriteBatch m_spriteBatch;
    List<GameEntity> m_Entities = new List<GameEntity>();

    public Assault()
    {
      m_graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Assets";
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      m_Entities.Add(new Tank());
      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      m_spriteBatch = new SpriteBatch(GraphicsDevice);
      foreach (GameEntity entity in m_Entities)
        entity.Load(Content);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      foreach (GameEntity entity in m_Entities)
        entity.Update(gameTime);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);
      m_spriteBatch.Begin();
      foreach (GameEntity entity in m_Entities)
        entity.Draw(m_spriteBatch);
      m_spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
