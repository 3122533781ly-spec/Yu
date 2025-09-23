using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotItem : MonoBehaviour
{
    [SerializeField] private List<PlotSubItem> plots;
    public int Idnex { get; private set; }

    Action _onEnd;

    public void StartPlay(Action onEnd)
    {
        _onEnd = onEnd;
        Idnex = 0;
        plots[Idnex].Play();
    }

    public void NextPlay()
    {
        Idnex++;
        if (Idnex < plots.Count) plots[Idnex].Play();
        else _onEnd.Invoke();
    }

    [Button]
    public void TestRest()
    {
        for (int i = 0; i < plots.Count; i++)
        {
            plots[i].Rest();
        }
    }
    [Button]
    public void TestPlay()
    {
        for (int i = 0; i < plots.Count; i++)
        {
            plots[i].TestPlay();
        }
    }
}