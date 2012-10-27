using System;
using System.Linq;
using System.Collections.Generic;
using AiLibrary.GeneticAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace AiTests
{
  [TestClass]
  public class GATest
  {
    struct Schedule
    {
      public List<Slot> Slots { get; set; }
      public double BoxUsage { get; set; }
    }

    class Box
    {
      public int Id { get; set; }
      public int Size { get; set; }
    }

    class Slot
    {
      public Slot()
      {
        Boxes = new List<Box>();
      }
      public int Size { get; set; }
      public List<Box> Boxes { get; set; }
      public double PercentFilled { get { return Boxes.Sum(b => b.Size) / (double)Size; } }
      public int Fill { get { return Boxes.Sum(b => b.Size); } }
    }

    class BestFitFunctions : IGAFunctions
    {
      List<Box> m_Boxes = new List<Box>();
      int m_SlotSize;
      int m_BoxIndexBitSize;
      int m_SlotIndexBitSize;
      int m_NumSlots;
      private List<Slot> m_Slots;
      
      public BestFitFunctions(int maxBoxSize, int slotSize, int numBoxes)
      {
        m_SlotSize = slotSize;
        Random rand = new Random();
        for (int i = 0; i < numBoxes; i++)
        {
          m_Boxes.Add(new Box() { Size = rand.Next(maxBoxSize), Id = i });
        }
        double sumOfBoxes = m_Boxes.Sum(b => b.Size);
        double slots =  sumOfBoxes / slotSize;
        m_NumSlots = (int)Math.Ceiling(slots);
        m_SlotIndexBitSize = Convert.ToString(m_NumSlots-1, 2).Length;
        m_BoxIndexBitSize = Convert.ToString(numBoxes-1, 2).Length;

        m_Slots = new List<Slot>();
        for (int i = 0; i < m_NumSlots; i++)
          m_Slots.Add(new Slot() { Size = m_SlotSize });
      }

      

      public Schedule Decode(BitArray data)
      {        
        foreach (Slot s in m_Slots)
          s.Boxes.Clear();

        bool[] boxesSelected = new bool[m_Boxes.Count];

        bool[] boxIndex = new bool[m_BoxIndexBitSize];
        bool[] slotIndex = new bool[m_SlotIndexBitSize];

        for (int i = 0; i < data.Length; )
        {
          for (int b = 0; b < m_BoxIndexBitSize; b++)
          {
            boxIndex[b] = data[i + b];

          }
          i += m_BoxIndexBitSize;

          for (int s = 0; s < m_SlotIndexBitSize; s++)
          {
            slotIndex[s] = data[i + s];

          }
          i += m_SlotIndexBitSize;

          int bi = BoolArrayToInt(boxIndex);
          int si = BoolArrayToInt(slotIndex);

          if (si >= m_Slots.Count || bi >= m_Boxes.Count)
            continue;
          boxesSelected[bi] = true;
          m_Slots[si].Boxes.Add(m_Boxes[bi]);
        }

        Schedule sc = new Schedule();
        sc.Slots = m_Slots;
        sc.BoxUsage = boxesSelected.Where(b => b).ToArray().Length / (double)m_Boxes.Count;
        return sc;
      }

      public double CalculateFitness(System.Collections.BitArray data)
      {
        Decode(data);

        double fitness = 0;
        bool[] boxesSelected = new bool[m_Boxes.Count];
        foreach (Slot s in m_Slots)
        {
          if (s.Boxes.Count == 0 )
            continue;
          if (s.PercentFilled > 1)
            fitness -= (s.PercentFilled - 1);
          else
            fitness += s.PercentFilled;

          foreach (Box b in s.Boxes)
          {
            if (boxesSelected[b.Id])
            {
              fitness -= 0.05;
            }
            else
              boxesSelected[b.Id] = true;
          }
        }

        fitness += (boxesSelected.Where(b => b).ToArray().Length / (double)m_Boxes.Count)*10;
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

      public int NumberOfChromosones
      {
        get 
        {          
          return m_Boxes.Count*(m_BoxIndexBitSize + m_SlotIndexBitSize); 
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

    [TestMethod]
    public void TestMethod1()
    {
      AutoResetEvent stopEvent = new AutoResetEvent(false); 
      BestFitFunctions bs = new BestFitFunctions(2000, 4000, 8);
      GA ga = new GA(bs);

      ga.OnBestFitChanged += (s, e) =>
      {
        Debug.WriteLine(string.Format("New best fit {0}", ga.BestFit));
        Schedule sc = bs.Decode(ga.Best.Data);
        foreach(Slot sl in sc.Slots)
          Debug.WriteLine(string.Format("Slot percent {0}", sl.PercentFilled));
        Debug.WriteLine(string.Format("Box usage {0}", sc.BoxUsage));

        if (sc.BoxUsage == 1)
          stopEvent.Set();
      };

      DateTime start = DateTime.Now;
      ga.Start();
      stopEvent.WaitOne(TimeSpan.FromMinutes(2));
      Debug.WriteLine(string.Format("Runtime {0}", DateTime.Now - start));
    }
  }
}
