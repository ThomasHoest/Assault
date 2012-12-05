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
    public Tank()
    {
      Mass = 1F;
      MaxForce = 100;
      MaxSpeed = 50;
      MaxTurnRate = 100;
      Position = new Vector2(500, 500);
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
    public Dummy()
    {
      Mass = 1F;
      MaxForce = 100;
      MaxSpeed = 50;
      MaxTurnRate = 100;
      Position = new Vector2(500, 500);
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
      Position = new Vector2(controls.TouchInput.X, controls.TouchInput.Y);
    }    
  }
}
