using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AiLibrary.BinFitting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiTests
{
  public class FittingSet : IFittingSet
  {
    public FittingSet()
    {
      Items = new List<IFitting>();
      Bins = new List<IBin>();
    }

    public List<IFitting> Items { get; set; }
    public List<IBin> Bins { get; set; }
    public CreateBin Create { get; set; }
    public delegate IBin CreateBin();


    public IBin CreateNewBin()
    {
      return Create();
    }
  }

  [TestClass]
  public class FittingTest
  {
    [TestMethod]
    public void FirstFit()
    {
      List<Box> boxes = new List<Box>();      
      Random rand = new Random();
      for (int i = 0; i < 8; i++)
      {
        boxes.Add(new Box() { Weight = rand.Next(2000), Id = i });
      }
      double sumOfBoxes = boxes.Sum(b => b.Weight);
      BinFitting fitting = new BinFitting();

      Debug.WriteLine("FF");
      TestFitAlgorithm(boxes, s=>fitting.FirstFit(s));      
      Debug.WriteLine("FFD");
      TestFitAlgorithm(boxes, s=>fitting.FirstFitDescending(s));      
      Debug.WriteLine("BF");
      TestFitAlgorithm(boxes, s=>fitting.BestFit(s));      
      Debug.WriteLine("BFD");
      TestFitAlgorithm(boxes, s=>fitting.BestFitDescending(s));      
      Debug.WriteLine("WF");
      TestFitAlgorithm(boxes, s=>fitting.WorstFit(s));      
      Debug.WriteLine("WFD");
      TestFitAlgorithm(boxes, s=>fitting.WorstFitDescending(s));      
      
    }

    delegate void FitIt(FittingSet set);
    private void TestFitAlgorithm(List<Box> boxes, FitIt fit)
    {
      double best = double.MaxValue;
      double res;
      int bestBinCount = 0;
      FittingSet set = new FittingSet();
      set.Create = () => { return new Slot() { Size = 4000 }; };
      int sets;
      FittingSet bestSet = null;
      for (sets = 1; sets <= 10; sets++)
      {
        set.Items.Clear();
        foreach (Box b in boxes)
          set.Items.Add(b);
        BinFitting fitting = new BinFitting();
        fit(set);
        //PrintResult(set, sets);
        res = set.Bins.Count / (double)sets;
        if (best > res)
        {
          bestBinCount = set.Bins.Count;
          best = res;
          //bestSet = Helper.CloneObject(set);
          double percentRemaining = set.Bins.Sum(b => b.SpaceRemaining) / (double)set.Bins.Sum(b => b.Size);
          Debug.WriteLine(string.Format("Percent remaining {0}", percentRemaining*100));
        }
      }
      System.Diagnostics.Debug.WriteLine(string.Format("Best usage {0} slots {1}", best, bestBinCount));
      //return bestSet;
    }

    private void PrintResult(FittingSet set, int sets)
    {
      int count = 0;
      double filled = 0;
      foreach (IBin bin in set.Bins)
      {
        Slot s = (Slot)bin;
        Debug.WriteLine(string.Format("Slot {0} fill {1}", count, s.Fill));
        foreach (Box b in s.Boxes)
        {
          Debug.Write(b.Id + ",");
        }
        Debug.WriteLine("");
        count++;
        filled += s.PercentFilled;
      }

      System.Diagnostics.Debug.WriteLine("Result usage " + set.Bins.Count/(double)sets);
    }
  }

  static class Helper
  {
    public static TObject CloneObject<TObject>(TObject original) where TObject : class
    {
      if (original == null)
        return null;

      XmlSerializer serializer = null;
      serializer = new XmlSerializer(original.GetType());

      using (MemoryStream ms = new MemoryStream())
      {
        serializer.Serialize(ms, original);
        ms.Position = 0;
        return (TObject)serializer.Deserialize(ms);
      }
    }
  }

}
