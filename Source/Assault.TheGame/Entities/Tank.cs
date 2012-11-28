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
    public override void Load(Microsoft.Xna.Framework.Content.ContentManager content)
    {
      Texture = content.Load<Texture2D>("Enemy");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, Color.White);
    } 
  }
}
