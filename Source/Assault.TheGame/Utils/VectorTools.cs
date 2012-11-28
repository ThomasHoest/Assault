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
    public static void Truncate(this Vector2 v, float max)
    {
      float s;
      s = max / v.Length();
      s = (s < 1.0f) ? 1.0f : s;
      v.X *= s;
      v.Y *= s;
    }

    public static Vector2 Perpendicular(this Vector2 v)
    {
      return new Vector2(-v.Y, v.X);
    }
  }
}
