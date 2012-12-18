using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Utils
{
  public static class VectorExtensions
  {
    public static Vector2 Truncate(this Vector2 v, float max)
    {
      float s;
      s = max / v.Length();
      s = (s < 1.0f) ? s : 1.0F;
      v.X *= s;
      v.Y *= s;
      return v;
    }

    public static Vector2 Perpendicular(this Vector2 v)
    {
      return new Vector2(-v.Y, v.X);
    }

    public static void Vec2DRotateAroundOrigin(this Vector2 v, double ang)
    {
      //create a transformation matrix
      C2DMatrix mat = new C2DMatrix();

      //rotate
      mat.Rotate(ang);
	
      //now transform the object's vertices
      mat.TransformVector2Ds(v);
    }
  }
}
