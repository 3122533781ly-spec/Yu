using System.Collections.Generic;
using Fangtang;
using UnityEngine;

public class InGameMatchModel : ElementModel
{
    public List<DotPoint> AllPoint { get; }

    public List<DotPoint> LinkedList { get; }

    public DotPoint LastPoint => LinkedList[LinkedList.Count - 1];

    public DotPoint LastTwoPoint => LinkedList[LinkedList.Count - 2];

    public void AddToLast(DotPoint target)
    {
        LinkedList.Add(target);
    }

    public void RemoveLast(DotPoint target)
    {
        LinkedList.Remove(target);
    }

    public bool IsAllPointDog()
    {
        for (int i = 0; i < AllPoint.Count; i++)
        {
            if (AllPoint[i].Data.Value != 2)
                continue;

            if (!AllPoint[i].Model.HasFlip)
            {
                return false;
            }
        }

        return true;
    }

    public void InitStartData(List<DotPoint> all)
    {
        AllPoint.AddRange(all);
    }

    public InGameMatchModel()
    {
        AllPoint = new List<DotPoint>();
        LinkedList = new List<DotPoint>();
    }
}