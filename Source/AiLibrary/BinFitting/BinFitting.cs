using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiLibrary.BinFitting
{
  public interface IFitting
  {
    int Weight { get; }
    int BinNumber { get; set; }
  }

  public interface IBin
  {
    int Size { get; }
    int SpaceRemaining { get; }
    bool Add(IFitting item);
    bool Closed { get; set; }    
  }

  public interface IFittingSet
  {
    IBin CreateNewBin();
    List<IFitting> Items { get; set; }
    List<IBin> Bins { get; set; }
  }  

  public class BinFitting
  {
    public void FirstFit(IFittingSet set)
    {
      set.Bins.Add(set.CreateNewBin());
      foreach (IFitting fit in set.Items)
      {
        fit.BinNumber = -1;
        for(int i=0; i<set.Bins.Count; i++)
        {
          IBin bin = set.Bins[i];
          if (!bin.Closed && bin.SpaceRemaining > fit.Weight)
          {
            fit.BinNumber = i;
            if(bin.Add(fit))
              break;
          }
        }

        if (fit.BinNumber == -1)
        {
          IBin bin = set.CreateNewBin();
          set.Bins.Add(bin);
          fit.BinNumber = set.Bins.Count - 1;
          bin.Add(fit);
        }
      }      
    }    

    public void FirstFitDescending(IFittingSet set)
    {
      set.Items.Sort(CompareByWeight);
      FirstFit(set);
    }

    public void BestFit(IFittingSet set)
    {
      set.Bins.Add(set.CreateNewBin());
      foreach (IFitting fit in set.Items)
      {
        fit.BinNumber = -1;
        IBin bestFit = null;
        
        for (int i = 0; i < set.Bins.Count; i++)
        {
          IBin bin = set.Bins[i];
          if (!bin.Closed && bin.SpaceRemaining > fit.Weight)
          {
            if (bestFit == null || bestFit.SpaceRemaining - fit.Weight < bin.SpaceRemaining - fit.Weight)
              bestFit = bin;
          }
        }

        if (bestFit != null)
        {
          fit.BinNumber = set.Bins.IndexOf(bestFit);
          bestFit.Add(fit);
        }

        if (fit.BinNumber == -1)
        {
          IBin bin = set.CreateNewBin();
          set.Bins.Add(bin);
          fit.BinNumber = set.Bins.Count - 1;
          bin.Add(fit);
        }
      }
    }

    public void BestFitDescending(IFittingSet set)
    {
      set.Items.Sort(CompareByWeight);
      BestFit(set);
    }

    public void WorstFit(IFittingSet set)
    {
      set.Bins.Add(set.CreateNewBin());
      foreach (IFitting fit in set.Items)
      {
        fit.BinNumber = -1;
        IBin worstFit = null;

        for (int i = 0; i < set.Bins.Count; i++)
        {
          IBin bin = set.Bins[i];
          if (!bin.Closed && bin.SpaceRemaining > fit.Weight)
          {
            if (worstFit == null || worstFit.SpaceRemaining - fit.Weight > bin.SpaceRemaining - fit.Weight)
              worstFit = bin;
          }
        }

        if (worstFit != null)
        {
          fit.BinNumber = set.Bins.IndexOf(worstFit);
          worstFit.Add(fit);
        }

        if (fit.BinNumber == -1)
        {
          IBin bin = set.CreateNewBin();
          set.Bins.Add(bin);
          fit.BinNumber = set.Bins.Count - 1;
          bin.Add(fit);
        }
      }
    }

    public void WorstFitDescending(IFittingSet set)
    {
      set.Items.Sort(CompareByWeight);
      WorstFit(set);
    }


    int CompareByWeight(IFitting f1, IFitting f2)
    {
      if (f1.Weight < f2.Weight)
        return 1;

      if (f1.Weight > f2.Weight)
        return -1;

      return 0;
    }

  }
}
