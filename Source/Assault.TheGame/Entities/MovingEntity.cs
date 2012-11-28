using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Assault.TheGame.Engine;
using Assault.TheGame.Utils;

namespace Assault.TheGame.Entities
{
  abstract class MovingEntity : GameEntity
  {    
    public Vector2 Heading { get; set; }
    public Vector2 Side { get; set; }
    public Vector2 Velocity { get; set; }

    public float Mass { get; set; }
    public float MaxSpeed { get; set; }
    public float MaxForce { get; set; }
    public float MaxTurnRate { get; set; }

    public TimeSpan TimeElapsed { get; set; }

    private SteeringBehavior m_Steering = new SteeringBehavior();

    public override void Update(GameTime time)
    {
      TimeElapsed = time.ElapsedGameTime;

      Vector2 OldPos = Position;
      Vector2 SteeringForce;
      SteeringForce = m_Steering.Calculate(this);

      //Acceleration = Force/Mass
      Vector2 acceleration; 
      Vector2.Divide(ref SteeringForce, Mass, out acceleration);

      //update velocity
      Velocity += Vector2.Multiply(acceleration, (float)time.ElapsedGameTime.TotalSeconds);

      //make sure vehicle does not exceed maximum velocity
      Velocity.Truncate(MaxSpeed);

      //update the position
      Position += Vector2.Multiply(Velocity, (float)time.ElapsedGameTime.TotalSeconds);

      //update the heading if the vehicle has a non zero velocity
      if (Velocity.LengthSquared() > 0.00000001)
      {
        Heading = Vector2.Normalize(Velocity);
        Side = Heading.Perpendicular();
      }

      //EnforceNonPenetrationConstraint(this, World()->Agents());

      //treat the screen as a toroid
      //WrapAround(m_vPos, m_pWorld->cxClient(), m_pWorld->cyClient());

      //update the vehicle's current cell if space partitioning is turned on
      //if (Steering()->isSpacePartitioningOn())
      //{
      //  World()->CellSpace()->UpdateEntity(this, OldPos);
      //}

      //if (isSmoothingOn())
      //{
      //  m_vSmoothedHeading = m_pHeadingSmoother->Update(Heading());
      //}
    }


    
  }
}
