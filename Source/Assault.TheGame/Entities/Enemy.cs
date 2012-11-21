using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Assault.TheGame.Entities
{
  class Enemy : GameEntity
  {

    public override void Update()
    {
      throw new NotImplementedException();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, new Rectangle(5, 5, 32, 32), Color.White);
    }

    public override void Load(ContentManager content)
    {
      Texture = content.Load<Texture2D>("Enemy");
    }
  }
}
