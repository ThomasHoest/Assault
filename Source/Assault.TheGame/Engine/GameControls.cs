using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assault.TheGame.Engine
{
  public class GameControls
  {
    public GameControls()
    {
      TouchInput = new TouchInput();
    }
    public TouchInput TouchInput { get; set; }
  }

  public class TouchInput
  {
    public float X { get; set; }
    public float Y { get; set; }
    public bool Up { get; set; }
    public bool Down { get; set; }
    public bool Tapped { get; set; }
  }
}
