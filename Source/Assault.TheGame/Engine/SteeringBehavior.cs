using Assault.TheGame.Entities;
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
    private double m_WanderJitter;
    private Vector2 m_WanderTarget;
    private float m_WanderRadius;
    private float m_WanderDistance;
      
    public Vector2 Calculate(MovingEntity entity)
    {
      m_SteeringForce = Vector2.Zero;
      m_SteeringForce += CalculateWeightedSum(entity);
      return m_SteeringForce;
    }

    public Vector2 CalculateWeightedSum(MovingEntity entity)
    {
      Vector2 force = Vector2.Zero;
      force += Wander(entity);
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

    private Vector2 PointToWorldSpace(Vector2 point, Vector2 heading, Vector2 side, Vector2 position)
    {
      //make a copy of the point
      Vector2 TransPoint = point;

      //create a transformation matrix
      Matrix m = new Matrix();
      
      //rotate
      Matrix.rota

      //and translate
      matTransform.Translate(AgentPosition.x, AgentPosition.y);

      //now transform the vertices
      matTransform.TransformVector2Ds(TransPoint);

      return TransPoint;
      
    }

    private double RandomClamped()
    {
      Random rand = new Random();
      return rand.NextDouble() - rand.NextDouble();
    }
  }
}
