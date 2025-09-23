using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotUI : MonoBehaviour
{
    [SerializeField] private Button clickBtn;
    [SerializeField] private List<PlotItem> plots;

    public int Idnex { get; private set; }

    Action _onEnd;

    private void OnEnable()
    {
        clickBtn.onClick.AddListener(OnBtnClick);
    }

    private void OnDisable()
    {
        clickBtn.onClick.RemoveListener(OnBtnClick);
    }

    public void Rest()
    {
        for (int i = 0; i < plots.Count; i++)
        {
            plots[i].TestRest();
            plots[i].SetActive(false);
        }
    }

    public void StartPlay(Action onEnd)
    {
        _onEnd = onEnd;
        Idnex = 0;
        plots[Idnex].SetActive(true);
        plots[Idnex].StartPlay(onEndPlot);
    }

    void onEndPlot()
    {
        Idnex++;
        if (Idnex < plots.Count)
        {
            for (int i = 0; i < plots.Count; i++)
            {
                plots[i].SetActive(i == Idnex);
            }
            plots[Idnex].StartPlay(onEndPlot);
        }
        else _onEnd.Invoke();
    }

    void OnBtnClick()
    {
        plots[Idnex].NextPlay();
    }
}