using System;
using System.Linq;
using System.Collections.Generic;
using AiLibrary.GeneticAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using AiLibrary.BinFitting;

namespace AiTests
{
  public struct Schedule
  {
    public List<Slot> Slots { get; set; }
  }

  public class Box : IFitting
  {
    public int Id { get; set; }
    public int Weight { get; set; }
    public int BinNumber { get; set; }
  }

  public class Slot : IBin
  {
    public Slot()
    {
      Boxes = new List<Box>();
    }
    public bool Closed { get; set; }
    public int Size { get; set; }
    public int SpaceRemaining { get { return Size - Fill; } }
    public List<Box> Boxes { get; set; }
    public double PercentFilled { get { return Boxes.Sum(b => b.Weight) / (double)Size; } }
    public int Fill { get { return Boxes.Sum(b => b.Weight); } }
    public bool Add(IFitting item)
    {
      Box b = item as Box;
      if (b != null)
      {
        foreach (Box old in Boxes)
          if (old.Id == b.Id)
            return false;

        Boxes.Add(b);
      }
      return true;
    }
  }

  public class BestFitFunctions : IGAFunctions
  {
    
    int m_SlotSize;
    int m_SlotIndexBitSize;
    int m_NumSlots;

    public List<Box> Boxes { get; set; }
    public List<Slot> Slots {get; set;}
    public double SlotsCountExact { get; set; }
    public int Repetitions{get; set;}

    public BestFitFunctions(int maxBoxSize, int slotSize, int numBoxes)
    {
      Boxes = new List<Box>();
      Slots = new List<Slot>();

      m_SlotSize = slotSize;
      Random rand = new Random();
      for (int i = 0; i < numBoxes; i++)
      {
        Boxes.Add(new Box() { Weight = rand.Next(maxBoxSize), Id = i });
      }
      double sumOfBoxes = Boxes.Sum(b => b.Weight);

      double bestFit = 0;

      for (int i = 1; i <= 10; i++)
      {
        double slots = (sumOfBoxes * i) / slotSize;
        double fit = slots - Math.Floor(slots);
        if (fit > bestFit)
        {
          SlotsCountExact = slots;
          System.Diagnostics.Debug.WriteLine("Optimal fit sum " + slots);
          bestFit = fit;
          Repetitions = i;
          m_NumSlots = (int)Math.Ceiling(slots);
        }
      }

      m_SlotIndexBitSize = Convert.ToString(m_NumSlots - 1, 2).Length;

      Slots = new List<Slot>();
      for (int i = 0; i < m_NumSlots; i++)
        Slots.Add(new Slot() { Size = m_SlotSize });
    }

    public Schedule Decode(BitArray data)
    {
      foreach (Slot s in Slots)
        s.Boxes.Clear();

      bool[] slotIndex = new bool[m_SlotIndexBitSize];

      for (int i = 0; i < data.Length; )
      {
        for (int s = 0; s < m_SlotIndexBitSize; s++)
        {
          slotIndex[s] = data[i + s];
        }
        int bi = (i / m_SlotIndexBitSize) % Boxes.Count;
        i += m_SlotIndexBitSize;
        int si = BoolArrayToInt(slotIndex);

        if (si >= Slots.Count)
          continue;

        Slots[si].Boxes.Add(Boxes[bi]);
      }

      Schedule sc = new Schedule();
      sc.Slots = Slots;
      return sc;
    }

    public double CalculateFitness(System.Collections.BitArray data)
    {
      Decode(data);

      double fitness = 0;
      int[] boxCount = new int[Boxes.Count];
      foreach (Slot s in Slots)
      {
        if (s.Boxes.Count == 0)
          continue;
        if (s.PercentFilled > 1)
          fitness -= (s.PercentFilled - 1);
        else
          fitness += s.PercentFilled;

        foreach (Box b in s.Boxes)
        {
          boxCount[b.Id]++;
          if (boxCount[b.Id] > Repetitions)
            fitness -= boxCount[b.Id] - Repetitions;
        }

      }

      if (fitness < 0)
        return 0.001;
      return fitness;
    }

    private int BoolArrayToInt(bool[] data)
    {
      int res = 0;
      for (int i = 0; i < data.Length; i++)
        res += data[i] ? (int)Math.Pow(2, i) : 0;
      return res;
    }

    public int PopulationSize
    {
      get { return 200; }
    }

    public int ChromosoneLength
    {
      get
      {
        return Boxes.Count * Repetitions * m_SlotIndexBitSize;
      }
    }

    public double MutationRate
    {
      get { return 0.001; }
    }

    public double CrossOverRate
    {
      get { return 0.7; }
    }    
  }

  [TestClass]
  public class GATest
  {
    

    [TestMethod]
    public void TestMethod1()
    {
      AutoResetEvent stopEvent = new AutoResetEvent(false); 
      BestFitFunctions bs = new BestFitFunctions(2000, 4000, 8);
      GA ga = new GA(bs);

      ga.OnBestFitChanged += (s, e) =>
      {
        Debug.WriteLine(string.Format("New best fit {0} generation {1}", ga.BestFit, ga.Generation));
        Schedule sc = bs.Decode(ga.Best.Data);
        double filled = 0;
        foreach (Slot sl in sc.Slots)
        {
          filled += sl.PercentFilled;
          //Debug.WriteLine(string.Format("Slot percent {0}", sl.PercentFilled));
        }

        System.Diagnostics.Debug.WriteLine("Optimal fit sum " + filled);

        double criteria = bs.SlotsCountExact * 0.8;
        if (criteria <= filled)
          stopEvent.Set();

      };

      DateTime start = DateTime.Now;
      ga.Start();
      stopEvent.WaitOne(TimeSpan.FromMinutes(2));
      ga.Stop();
      Schedule bestsc = bs.Decode(ga.Best.Data);
      int count = 0;
      foreach (Slot s in bestsc.Slots)
      {
        Debug.WriteLine(string.Format("Slot {0} fill {1}", count,s.Fill));
        foreach (Box b in s.Boxes)
        {
          Debug.Write(b.Id + ",");
        }
        Debug.WriteLine("");
        count++;
      }

      Debug.WriteLine(string.Format("Runtime {0}", DateTime.Now - start));
    }
  }
}
