using Assault.TheGame.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Entities
{
  abstract class AutonomousEntity : MovingEntity
  {
    public AutonomousEntity(GameWorld world) : base(world)
    {

    }
  }
}
