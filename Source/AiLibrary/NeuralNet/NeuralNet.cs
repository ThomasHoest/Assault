using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiLibrary.NeuralNet
{
  public class NeuralNet
  {


  }

  class Neuron
  {
    public List<Connections> Connections { get; set; }

    public Neuron()
    {
      Connections = new List<Connections>();
    }

    public void AddConnection(Neuron n, double w)
    {
      Connections.Add(new Connections(){Target = n, Weigth = w});
    }



  }

  class Connections
  {
    public double Weigth { get; set; }
    public Neuron Target { get; set; }
  }
}

