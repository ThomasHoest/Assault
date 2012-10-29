using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AiLibrary.GeneticAlgorithm
{
  public interface IGAFunctions
  {
    double CalculateFitness(BitArray data);
    int PopulationSize { get; }
    int ChromosoneLength { get; }
    double MutationRate { get; }
    double CrossOverRate { get; }
  }

  public class Chromosome
  {
    public Chromosome()
    {
    }

    public Chromosome(int length)
    {
      Data = new BitArray(length);
      Random rand = new Random();
      for(int i = 0; i<length; i++)
        Data.Set(i,rand.Next(2) == 1);        
    }

    public BitArray Data { get; set; } 
    public double Fitness { get; set; }

  }

  public class GA
  {

    public Chromosome Best { get; set; }
    public double BestFit { get; set; }
    public int Generation { get; set; }    
    public List<Chromosome> Chromosomes { get; set; }
    public event EventHandler OnNewGeneration;
    public event EventHandler OnBestFitChanged;

    private Random m_Rand = new Random();
    IGAFunctions m_Functions;
    private bool m_DoRun;
    private AutoResetEvent m_Stopped = new AutoResetEvent(false);

    public GA(IGAFunctions functions)
    {
      m_Functions = functions;
      Chromosomes = new List<Chromosome>();

      for (int i = 0; i < functions.PopulationSize; i++)
      {
        Chromosome chromosome = new Chromosome(functions.ChromosoneLength);
        chromosome.Fitness = m_Functions.CalculateFitness(chromosome.Data);
        Chromosomes.Add(chromosome);
      }
    }


    private void MakeNewGeneration()
    {
      List<Chromosome> children = new List<Chromosome>();

      while (children.Count < m_Functions.PopulationSize)
      {
        Chromosome mum = SelectChromosome();
        Chromosome dad = SelectChromosome();

        bool doCrossOver = TossTheDice(m_Functions.CrossOverRate);

        Chromosome child1 = doCrossOver ? CrossOver(mum, dad) : mum;
        Chromosome child2 = doCrossOver ? CrossOver(dad, mum) : dad;

        Mutate(child1);
        Mutate(child2);

        child1.Fitness = m_Functions.CalculateFitness(child1.Data);
        child2.Fitness = m_Functions.CalculateFitness(child2.Data);

        children.Add(child1);
        children.Add(child2);

        UpdateBestFit(child1);
        UpdateBestFit(child2);

      }

      Chromosomes = children;
    }

    private void UpdateBestFit(Chromosome c)
    {
      if (c.Fitness > BestFit)
      {
        BestFit = c.Fitness;
        Best = c;
        if (OnBestFitChanged != null)
          OnBestFitChanged(this, EventArgs.Empty);
      }
    }

    private void Mutate(Chromosome c)
    {
      for (int i = 0; i < c.Data.Length; i++)
      {
        if (TossTheDice(m_Functions.MutationRate))
          c.Data.Set(i, !c.Data[i]);
      }
    }

    private bool TossTheDice(double p)
    {
      double val = m_Rand.Next(10000)/(double)10000;
      return p > val;
    }

    private Chromosome CrossOver(Chromosome p1, Chromosome p2)
    {
      Chromosome c = new Chromosome();
      c.Data = p1.Data.Clone() as BitArray;
      int index = m_Rand.Next(c.Data.Length);
      for (int i = 0; i < index; i++)
        c.Data.Set(i, p2.Data[i]);
      return c;
    }

    private Chromosome SelectChromosome()
    {
      double totalFit = Chromosomes.Sum(g => g.Fitness);
      double fitnessIndex = 0;
      Random rand = new Random();
      double randomFitness = rand.Next((int)totalFit*100) / 100;

      for (int i = 0; i < Chromosomes.Count; i++)
      {
        Chromosome c = Chromosomes[i];
        fitnessIndex += c.Fitness;
        if (fitnessIndex > randomFitness)
          return c;
      }

      throw new InvalidOperationException("Should have found a genome");
    }    

    public void Start()
    {
      m_DoRun = true;
      ThreadPool.QueueUserWorkItem(s=>
        {
          while (m_DoRun)
          {
            MakeNewGeneration();
            Generation++;
            if(OnNewGeneration != null)
              OnNewGeneration(this, EventArgs.Empty);
          }
          m_Stopped.Set();
        });
    }

    public void Stop()
    {
      m_DoRun = false;
      m_Stopped.WaitOne();
    }
  }
}
