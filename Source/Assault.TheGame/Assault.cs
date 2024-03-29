﻿using Assault.TheGame.Engine;
using Assault.TheGame.Entities;
using Assault.TheGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
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
    GameControls m_Controls;
    GameWorld m_World; 

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
      TouchPanel.EnabledGestures = GestureType.Tap;
      m_World = new GameWorld();
      m_Controls = new GameControls();

      Tank t = new Tank(m_World);
      Dummy d = new Dummy(m_World); 
      t.SetPursuer(d);
      m_World.GameEntities.Add(t);
      m_World.GameEntities.Add(d);

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
      foreach (GameEntity entity in m_World.GameEntities)
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
      if (TouchPanel.IsGestureAvailable)
      {
        GestureSample sample = TouchPanel.ReadGesture();
        m_Controls.TouchInput.X = sample.Position.X;
        m_Controls.TouchInput.Y = sample.Position.Y;
      }

      foreach (GameEntity entity in m_World.GameEntities)
      {
        entity.Update(gameTime, m_Controls);
      }

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      //foreach (GameEntity entity in m_Entities)
      //  entity.Update(gameTime);

      GraphicsDevice.Clear(Color.CornflowerBlue);
      m_spriteBatch.Begin();
      foreach (GameEntity entity in m_World.GameEntities)
        entity.Draw(m_spriteBatch);
      m_spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
