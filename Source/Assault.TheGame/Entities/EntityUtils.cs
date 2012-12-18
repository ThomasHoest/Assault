using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Entities
{
  internal static class EntityUtils
  {
    public static void EnforceNonPenetrationConstraint(GameEntity entity, 
                                        ICollection<GameEntity> entities)
    {
      //iterate through all entities checking for any overlap of bounding radii
      foreach(GameEntity e in entities)
      {
        //make sure we don't check against the individual
        if (e == entity) continue;

        //calculate the distance between the positions of the entities
        Vector2 ToEntity = entity.Position - e.Position;

        float DistFromEachOther = ToEntity.Length();

        //if this distance is smaller than the sum of their radii then this
        //entity must be moved away in the direction parallel to the
        //ToEntity vector   
        float AmountOfOverLap = e.BoundingRadius + entity.BoundingRadius - DistFromEachOther;

        if (AmountOfOverLap >= 0)
        {
          //move the entity a distance away equivalent to the amount of overlap.
          Vector2 unitVecDist = Vector2.Divide(ToEntity, DistFromEachOther);
          entity.Position = entity.Position + unitVecDist * AmountOfOverLap;
        }
      }//next entity
}
  }
}
