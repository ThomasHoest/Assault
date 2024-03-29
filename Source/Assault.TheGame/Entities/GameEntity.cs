﻿using Assault.TheGame.Engine;
using Assault.TheGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Entities
{
  abstract class GameEntity
  {
    int m_ID;

    public float BoundingRadius {get; set;}
    public GameWorld World { get; set; } 

    public Vector2 Position { get; set; }
    public Vector2 Scale { get; set; }

    public Texture2D Texture { get; set; }
    
    public abstract void Update(GameTime time, GameControls controls);
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Load(ContentManager content);

    public GameEntity(GameWorld world)
    {
      World = world;
    }

  }
}
