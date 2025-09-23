using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogConsoleModel
{
    public Action OnAddNewLog = delegate { };
    public Action<string> OnFilterStringChanged = delegate { };

    public List<LogInfo> LogInfoList { get; private set; }
    public LogInfo SelectedLogInfo { get; set; }
    public string FilterString
    {
        get
        {
            return _filterString;
        }
        set
        {
            if (_filterString != value)
            {
                _filterString = value;
                OnFilterStringChanged(_filterString);
            }
        }
    }

    public bool IsOpenConsole { get; set; }

    public List<LogInfo> DisplayLogInfoList { get; set; }
    public int CurrentDisplayPageIndex { get; set; }

    public bool IsDesplayLastPage
    {
        get
        {
            if (DisplayLogInfoList.Count < LogSetting.Instance.PageLength)
            {
                return true;
            }

            int nextPageIndex = CurrentDisplayPageIndex + 1;
            int displayNum = 0;
            int pageIndex = 0;

            for (int i = 0; i < LogInfoList.Count; i++)
            {
                if (!BeFiltered(FilterString, LogInfoList[i].Content))
                {
                    displayNum++;
                    pageIndex = displayNum / (LogSetting.Instance.PageLength + 1);

                    if (pageIndex == nextPageIndex)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public bool IsLastPageDisplayFull
    {
        get
        {
            if (DisplayLogInfoList.Count < LogSetting.Instance.PageLength)
            {
                return false;
            }

            int nextPageIndex = CurrentDisplayPageIndex + 1;
            int displayNum = 0;
            int pageIndex = 0;

            for (int i = 0; i < LogInfoList.Count; i++)
            {
                if (!BeFiltered(FilterString, LogInfoList[i].Content))
                {
                    displayNum++;
                    pageIndex = displayNum / (LogSetting.Instance.PageLength + 1);

                    if (pageIndex == nextPageIndex)
                    {
                        return false;
                    }
                }
            }

            if (pageIndex == CurrentDisplayPageIndex &&
                displayNum % LogSetting.Instance.PageLength == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Vector2 ConsoleScroll
    {
        get
        {
            return _consoleScroll;
        }
        set
        {
            if (_consoleScroll != value)
            {
                _consoleScroll = value;
            }
        }
    }

    public int GetLastPageIndex()
    {
        int displayNum = 0;
        int pageIndex = 0;

        for (int i = 0; i < LogInfoList.Count; i++)
        {
            if (!BeFiltered(FilterString, LogInfoList[i].Content))
            {
                displayNum++;
                pageIndex = displayNum / (LogSetting.Instance.PageLength + 1);
            }
        }
        return pageIndex;
    }

    public void AddLogInfo(LogInfo logInfo)
    {
        if (!BeFiltered(FilterString, logInfo.Content))
        {
            if (IsLastPageDisplayFull)
            {
                CurrentDisplayPageIndex++;
                DisplayLogInfoList.Clear();
            }

            if (IsDesplayLastPage)
            {
                DisplayLogInfoList.Add(logInfo);
            }
        }

        LogInfoList.Add(logInfo);

        OnAddNewLog();
    }

    public void ClearAllLog()
    {
        LogInfoList.Clear();
        DisplayLogInfoList.Clear();
        CurrentDisplayPageIndex = 0;
    }

    public LogConsoleModel()
    {
        LogInfoList = new List<LogInfo>();
        DisplayLogInfoList = new List<LogInfo>();
        FilterString = "";
        IsOpenConsole = false;
        CurrentDisplayPageIndex = 0;
    }

    public void RefreshDesplayList()
    {
        DisplayLogInfoList.Clear();

        int displayNum = -1;
        int pageIndex = 0;

        for (int i = 0; i < LogInfoList.Count; i++)
        {
            if (!BeFiltered(FilterString, LogInfoList[i].Content))
            {
                displayNum++;
                pageIndex = displayNum / LogSetting.Instance.PageLength;

                if (pageIndex == CurrentDisplayPageIndex)
                {
                    DisplayLogInfoList.Add(LogInfoList[i]);

                    if (DisplayLogInfoList.Count >= LogSetting.Instance.PageLength)
                    {
                        return;
                    }
                }
            }
        }
    }

    private bool BeFiltered(string filterString, string content)
    {
        if (string.IsNullOrEmpty(filterString))
        {
            return false;
        }

        return !content.ToLower().Contains(filterString.ToLower());
    }

    private string _filterString;
    private Vector2 _consoleScroll;
}
