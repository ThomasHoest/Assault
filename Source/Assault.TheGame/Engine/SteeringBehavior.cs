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
  class SteeringBehavior
  {
    private Vector2 m_SteeringForce;
    private double m_WanderJitter = 100;
    private Vector2 m_WanderTarget;
    private float m_WanderRadius = 1.2F;
    private float m_WanderDistance = 2.0F;
    private MovingEntity m_Entity;
    public MovingEntity Target{get; set;}

    public SteeringBehavior(MovingEntity entity)
    {
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
      force += Pursuit(Target);
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

    Random m_Random = new Random();
    private double RandomClamped()
    {      
      return m_Random.NextDouble() - m_Random.NextDouble();
    }

    
  }
}
