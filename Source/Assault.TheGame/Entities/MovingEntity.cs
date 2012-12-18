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
using Assault.TheGame.World;

namespace Assault.TheGame.Entities
{
  abstract class MovingEntity : GameEntity
  {
    public Vector2 TargetPosition { get; set; }

    public Vector2 Heading { get; set; }
    public Vector2 Side { get; set; }
    public Vector2 Velocity { get; set; }

    public BehaviorType Behavior { get; set; }

    public double Speed { get { return Velocity.Length(); } }
    public float Mass { get; set; }
    public float MaxSpeed { get; set; }
    public float MaxForce { get; set; }
    public float MaxTurnRate { get; set; }
    public Deceleration Deceleration { get; set; }

    public TimeSpan TimeElapsed { get; set; }

    private SteeringBehavior m_Steering;

    public MovingEntity(GameWorld world) : base(world)
    {
      m_Steering = new SteeringBehavior(this);
      m_Steering.Walls.AddRange(world.Bounds);
    }

    public void SetTarget(MovingEntity target)
    {
      m_Steering.Target = target;
    }

    public void SetPursuer(MovingEntity pursuer)
    {
      m_Steering.Pursuer = pursuer;
    }

    public void Move(GameTime time)
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
      Velocity = Velocity.Truncate(MaxSpeed);

      //update the position
      Position += Vector2.Multiply(Velocity, (float)time.ElapsedGameTime.TotalSeconds);

      //update the heading if the vehicle has a non zero velocity
      if (Velocity.LengthSquared() > 0.00000001)
      {
        Heading = Vector2.Normalize(Velocity);
        Side = Heading.Perpendicular();
      }

      EntityUtils.EnforceNonPenetrationConstraint(this, World.GameEntities);

      World.CheckBounds(this);

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

    void WrapAround(ref Vector2 pos, int MaxX, int MaxY)
    {
      if (pos.X > MaxX) 
        pos.X = 0.0F;

      if (pos.X < 0)    
        pos.X = (float)MaxX;

      if (pos.Y < 0)    
        pos.Y = (float)MaxY;

      if (pos.Y > MaxY) 
        pos.Y = 0.0F;
    }
  }
}
