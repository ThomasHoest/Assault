using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Entities
{
  public class Wall2D
  {
    Vector2 m_vA;
    Vector2 m_vB;
    Vector2 m_vN;

    public void CalculateNormal()
    {
      Vector2 temp = Vector2.Normalize(m_vB - m_vA);
      m_vN.X = -temp.Y;
      m_vN.Y = temp.X;
    }

    public Wall2D(Vector2 A, Vector2 B)
    {
      m_vA = A;
      m_vB = B;
      CalculateNormal();
    }

    public Wall2D(Vector2 A, Vector2 B, Vector2 N)
    {
      m_vA = A;
      m_vB = B;
    }

    public Vector2 From()
    {
      return m_vA;
    }
    public void SetFrom(Vector2 v)
    {
      m_vA = v;
      CalculateNormal();
    }

    public Vector2 To() 
    { 
      return m_vB; 
    }
    public void SetTo(Vector2 v) 
    { 
      m_vB = v; 
      CalculateNormal(); 
    }

    public Vector2 Normal() 
    { 
      return m_vN; 
    }
    public void SetNormal(Vector2 n) 
    { 
      m_vN = n; 
    }

    public Vector2 Center()
    {
      Vector2 temp = Vector2.Add(m_vA,m_vB);
      return Vector2.Divide(temp, 2);      
    }
  }
}
