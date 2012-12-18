using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Utils
{
  public static class Geometry
  {
    public const double Pi = 3.14159;
    public const double TwoPi = Pi * 2;
    public const double HalfPi = Pi / 2;
    public const double QuarterPi = Pi / 4;

    public static bool LineIntersection2D(Vector2 A, Vector2 B, Vector2 C, Vector2 D, ref double dist, ref Vector2 point)
    {
      double rTop = (A.Y - C.Y) * (D.X - C.X) - (A.X - C.X) * (D.Y - C.Y);
      double rBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

      double sTop = (A.Y - C.Y) * (B.X - A.X) - (A.X - C.X) * (B.Y - A.Y);
      double sBot = (B.X - A.X) * (D.Y - C.Y) - (B.Y - A.Y) * (D.X - C.X);

      if ((rBot == 0) || (sBot == 0))
      {
        //lines are parallel
        return false;
      }

      double r = rTop / rBot;
      double s = sTop / sBot;

      if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
      {
        dist = Vector2.Distance(A, B) * r;
        point = A + Vector2.Multiply((B - A), (float)r);
        return true;
      }
      else
      {
        dist = 0;
        return false;
      }
    }

  }
}
