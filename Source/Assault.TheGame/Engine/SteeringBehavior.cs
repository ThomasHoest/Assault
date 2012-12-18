using Assault.TheGame.Entities;
using Assault.TheGame.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Engine
{
  [Flags]
  enum BehaviorType
  {
    None = 0x00000,
    Seek = 0x00002,
    Flee = 0x00004,
    Arrive = 0x00008,
    Wander = 0x00010,
    Cohesion = 0x00020,
    Separation = 0x00040,
    Allignment = 0x00080,
    Obstacle_avoidance = 0x00100,
    Wall_avoidance = 0x00200,
    Follow_path = 0x00400,
    Pursuit = 0x00800,
    Evade = 0x01000,
    Interpose = 0x02000,
    Hide = 0x04000,
    Flock = 0x08000,
    Offset_pursuit = 0x10000,
  }

  enum Deceleration 
  { 
    slow = 3, 
    normal = 2, 
    fast = 1 
  };

  class SteeringBehavior
  {
    private Vector2 m_SteeringForce;
    private double m_WanderJitter = 100;
    private Vector2 m_WanderTarget;
    private float m_WanderRadius = 1.2F;
    private float m_WanderDistance = 2.0F;
    private MovingEntity m_Entity;
    private Vector2 [] m_Feelers = new Vector2[3];

    Random m_Random = new Random();
    private float m_WallDetectionFeelerLength = 100;

    public MovingEntity Target {get; set;}
    public MovingEntity Pursuer { get; set; }
    public List<Wall2D> Walls { get; set; }    

    public SteeringBehavior(MovingEntity entity)
    {
      for (int i = 0; i < m_Feelers.Length; i++)
        m_Feelers[i] = new Vector2();

      Walls = new List<Wall2D>();

      m_Entity = entity;
    }
      
    public Vector2 Calculate(MovingEntity entity)
    {
      m_SteeringForce = Vector2.Zero;
      m_SteeringForce += CalculateWeightedSum(entity);
      return m_SteeringForce;
    }

    public Vector2 CalculateWeightedSum(MovingEntity entity)
    {
      Vector2 force = Vector2.Zero;
      if(entity.Behavior.HasFlag(BehaviorType.Pursuit))
        force += Pursuit(Target);
      if (entity.Behavior.HasFlag(BehaviorType.Seek))
        force += Seek(Target != null ? Target.Position : entity.TargetPosition);
      if (entity.Behavior.HasFlag(BehaviorType.Evade))
        force += Evade(Pursuer);
      if (entity.Behavior.HasFlag(BehaviorType.Arrive))
        force += Arrive(Target != null ? Target.Position : entity.TargetPosition, Deceleration.normal);
      if (entity.Behavior.HasFlag(BehaviorType.Wander))
        force += Wander(Target); //broken random numbers are not random
      if (entity.Behavior.HasFlag(BehaviorType.Wall_avoidance))
        force += WallAvoidance(Walls);
      return force;
    }

    private Vector2 Wander(MovingEntity entity)
    {
      m_WanderTarget = Vector2.Zero;
      //this behavior is dependent on the update rate, so this line must
      //be included when using time independent framerate.
      double JitterThisTimeSlice = m_WanderJitter * entity.TimeElapsed.TotalSeconds;

      //first, add a small random vector to the target's position
      m_WanderTarget += new Vector2((float)(RandomClamped() * JitterThisTimeSlice),
                                  (float)(RandomClamped() * JitterThisTimeSlice));

      //reproject this new vector back on to a unit circle
      m_WanderTarget.Normalize();

      //increase the length of the vector to the same as the radius
      //of the wander circle
      m_WanderTarget *= m_WanderRadius;

      //move the target into a position WanderDist in front of the agent
      Vector2 target = m_WanderTarget + new Vector2(m_WanderDistance, 0);

      //project the target into world space
      target = PointToWorldSpace(target,
                                           entity.Heading,
                                           entity.Side,
                                           entity.Position);

      //and steer towards it
      return target - entity.Position; 
    }

    Vector2 Pursuit(MovingEntity evader)
    {
      //if the evader is ahead and facing the agent then we can just seek
      //for the evader's current position.
      Vector2 toEvader = evader.Position - m_Entity.Position;

      double relativeHeading = Vector2.Dot(m_Entity.Heading, evader.Heading);
      double direction = Vector2.Dot(toEvader, m_Entity.Heading);
      if ( direction > 0 &&  relativeHeading < -0.95)  //acos(0.95)=18 degs
      {
        return Seek(evader.Position);
      }

      //Not considered ahead so we predict where the evader will be.
 
      //the lookahead time is propotional to the distance between the evader
      //and the pursuer; and is inversely proportional to the sum of the
      //agent's velocities
      float LookAheadTime = (float)(toEvader.Length() / 
                            (m_Entity.MaxSpeed + evader.Speed));
  
      //now seek to the predicted future position of the evader
      return Seek(evader.Position + Vector2.Multiply(evader.Velocity, LookAheadTime));
    }

    Vector2 Evade(MovingEntity pursuer)
    {
      /* Not necessary to include the check for facing direction this time */
      Vector2 ToPursuer = pursuer.Position - m_Entity.Position;

      //uncomment the following two lines to have Evade only consider pursuers 
      //within a 'threat range'
      const double ThreatRange = 100.0;
      if (ToPursuer.LengthSquared() > ThreatRange * ThreatRange) 
        return new Vector2();
 
      //the lookahead time is propotional to the distance between the pursuer
      //and the pursuer; and is inversely proportional to the sum of the
      //agents' velocities
      double LookAheadTime = ToPursuer.Length() / (m_Entity.MaxSpeed + pursuer.Speed);
  
      //now flee away from predicted future position of the pursuer
      return Flee(pursuer.Position + pursuer.Velocity * (float)LookAheadTime);
    }

    Vector2 Seek(Vector2 targetPos)
    {
      Vector2 desiredVelocity;
      Vector2 targetVector = (targetPos - m_Entity.Position); 
      Vector2.Normalize(ref targetVector, out desiredVelocity);

      if(float.IsNaN(desiredVelocity.X) || float.IsNaN(desiredVelocity.Y))
        return new Vector2(0,0);

      desiredVelocity *= m_Entity.MaxSpeed;
      return (desiredVelocity - m_Entity.Velocity);
    }

    Vector2 Arrive(Vector2 TargetPos, Deceleration deceleration)
    {
      Vector2 ToTarget = TargetPos - m_Entity.Position;

      //calculate the distance to the target
      double dist = ToTarget.Length();

      if (dist > 0)
      {
        //because Deceleration is enumerated as an int, this value is required
        //to provide fine tweaking of the deceleration..
        const double DecelerationTweaker = 0.3;

        //calculate the speed required to reach the target given the desired
        //deceleration
        double speed =  dist / ((double)deceleration * DecelerationTweaker);     

        //make sure the velocity does not exceed the max
        speed = Math.Min(speed, m_Entity.MaxSpeed);

        //from here proceed just like Seek except we don't need to normalize 
        //the ToTarget vector because we have already gone to the trouble
        //of calculating its length: dist. 
        Vector2 DesiredVelocity =  Vector2.Multiply(ToTarget, (float)(speed / dist));

        return (DesiredVelocity - m_Entity.Velocity);
      }

      return Vector2.Zero;
    }

    Vector2 Flee(Vector2 TargetPos)
    {
      //only flee if the target is within 'panic distance'. Work in distance
      //squared space.
     /* const double PanicDistanceSq = 100.0f * 100.0;
      if (Vec2DDistanceSq(m_Entity.Position, target) > PanicDistanceSq)
      {
        return Vector2D(0,0);
      }
      */

      Vector2 vec = (m_Entity.Position - TargetPos);
      vec.Normalize();
      Vector2 DesiredVelocity = vec * m_Entity.MaxSpeed;

      return (DesiredVelocity - m_Entity.Velocity);
    }

    Vector2 WallAvoidance(List<Wall2D> walls)
    {
      //the feelers are contained in a std::vector, m_Feelers
      CreateFeelers();

      double DistToThisIP = 0.0;
      double DistToClosestIP = double.MaxValue;

      //this will hold an index into the vector of walls
      int ClosestWall = -1;

      Vector2 SteeringForce = Vector2.Zero;
      Vector2 point = Vector2.Zero;
      Vector2 ClosestPoint = Vector2.Zero;                

      //examine each feeler in turn
      for (int flr = 0; flr < m_Feelers.Length; ++flr)
      {
        //run through each wall checking for any intersection points
        for (int w = 0; w < walls.Count; ++w)
        {
          if (Geometry.LineIntersection2D(m_Entity.Position,
                                 m_Feelers[flr],
                                 walls[w].From(),
                                 walls[w].To(),
                                 ref DistToThisIP,
                                 ref point))
          {
            //is this the closest found so far? If so keep a record
            if (DistToThisIP < DistToClosestIP)
            {
              DistToClosestIP = DistToThisIP;

              ClosestWall = w;

              ClosestPoint = point;
            }
          }
        }//next wall


        //if an intersection point has been detected, calculate a force  
        //that will direct the agent away
        if (ClosestWall >= 0)
        {
          //calculate by what distance the projected position of the agent
          //will overshoot the wall
          Vector2 OverShoot = m_Feelers[flr] - ClosestPoint;

          //create a force in the direction of the wall normal, with a 
          //magnitude of the overshoot

          SteeringForce = Vector2.Multiply(walls[ClosestWall].Normal(), OverShoot.Length());
        }

      }//next feeler

      return SteeringForce;
    }

    void CreateFeelers()
    {
      //feeler pointing straight in front
      m_Feelers[0] = m_Entity.Position + Vector2.Multiply(m_Entity.Heading, m_WallDetectionFeelerLength);

      //feeler to left
      Vector2 temp = m_Entity.Heading;
      temp.Vec2DRotateAroundOrigin(Geometry.HalfPi * 3.5f);
      m_Feelers[1] = m_Entity.Position + Vector2.Multiply(temp, m_WallDetectionFeelerLength / 2.0f);

      //feeler to right
      temp = m_Entity.Heading;
      temp.Vec2DRotateAroundOrigin(Geometry.HalfPi * 0.5f);
      m_Feelers[2] = m_Entity.Position + Vector2.Multiply(temp, m_WallDetectionFeelerLength / 2.0f);
    }

    private Vector2 PointToWorldSpace(Vector2 point, Vector2 heading, Vector2 side, Vector2 position)
    {
      //make a copy of the point
      Vector2 TransPoint = point;

      //create a transformation matrix
      C2DMatrix matrix = new C2DMatrix();
      
      //rotate
      matrix.Rotate(heading, side);

      //and translate
      matrix.Translate(position.X, position.Y);

      //now transform the vertices
      matrix.TransformVector2Ds(TransPoint);

      return TransPoint;      
    }
    
    private double RandomClamped()
    {      
      return m_Random.NextDouble() - m_Random.NextDouble();
    }

    
  }
}
