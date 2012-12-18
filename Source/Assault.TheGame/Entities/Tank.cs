using Assault.TheGame.Engine;
using Assault.TheGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Entities
{
  class Tank : AutonomousEntity
  {
    public Tank(GameWorld world) : base(world)
    {
      Mass = 0.05F;
      MaxForce = 200;
      MaxSpeed = 100;
      MaxTurnRate = 100;
      Position = new Vector2(500, 500);
      BoundingRadius = 16;
      Behavior = BehaviorType.Evade | BehaviorType.Wall_avoidance;
      
    }

    public override void Load(Microsoft.Xna.Framework.Content.ContentManager content)
    {
      Texture = content.Load<Texture2D>("Enemy");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, Color.White);
    }

    public override void Update(GameTime time, Engine.GameControls controls)
    {
      Move(time);
    }    
  }

  class Dummy : MovingEntity
  {
    public Dummy(GameWorld world)
      : base(world)
    {
      Mass = 0.05F;
      MaxForce = 250;
      MaxSpeed = 150;
      MaxTurnRate = 100;
      Position = new Vector2(200, 200);
      BoundingRadius = 16;
      Behavior = BehaviorType.Arrive;
    }

    public override void Load(Microsoft.Xna.Framework.Content.ContentManager content)
    {
      Texture = content.Load<Texture2D>("Enemy");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, Color.White);
    }

    public override void Update(GameTime time, Engine.GameControls controls)
    {
      TargetPosition = new Vector2(controls.TouchInput.X, controls.TouchInput.Y);
      Move(time);
    }    
  }
}
