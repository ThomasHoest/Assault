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
    public Texture2D Texture { get; set; }
    public abstract void Update();
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Load(ContentManager content);

  }
}
