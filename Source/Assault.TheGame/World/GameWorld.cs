using Assault.TheGame.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.World
{
  class GameWorld
  {
    public GameWorld()
    {
      GameEntities = new List<GameEntity>();
      Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
      Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

      Bounds = new List<Wall2D>();
      Bounds.Add(new Wall2D(new Vector2(0,0),new Vector2(Width,0)));
      Bounds.Add(new Wall2D(new Vector2(Width,0),new Vector2(Width,Height)));
      Bounds.Add(new Wall2D(new Vector2(Width,Height),new Vector2(0,Height)));
      Bounds.Add(new Wall2D(new Vector2(0,Height),new Vector2(0,0)));
    }

    public List<Wall2D> Bounds { get; set; }
    public int Width {get; set;}
    public int Height { get; set; }
    public List<GameEntity> GameEntities { get; set; }

    internal void CheckBounds(MovingEntity movingEntity)
    {
      if (movingEntity.Position.X > Width)
        movingEntity.Position = new Vector2(Width, movingEntity.Position.Y);
      if (movingEntity.Position.Y > Height)
        movingEntity.Position = new Vector2(movingEntity.Position.X, Height);
      if (movingEntity.Position.X < 0)
        movingEntity.Position = new Vector2(0, movingEntity.Position.Y);
      if (movingEntity.Position.Y < 0)
        movingEntity.Position = new Vector2(movingEntity.Position.X,0);
    }
  }
}
