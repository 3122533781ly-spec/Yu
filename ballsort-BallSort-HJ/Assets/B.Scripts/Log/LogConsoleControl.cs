using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class LogConsoleControl
{
    public void InjectDependence(LogConsoleModel model, LogConsole console)
    {
        _model = model;
        _console = console;
    }

    public void LogMessageReceived(string condition, string stackTrace, LogType type)
    {
        LogInfo info = new LogInfo()
        {
            Content = condition,
            StackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace(),
            Type = type,
            Id = System.Guid.NewGuid().ToString(),
        };

        _model.AddLogInfo(info);
    }

    public void SelectedOneLog(LogInfo info)
    {
        _currentFrameSelectedLog = info;
    }

    public void FilterLogWithString(string filterString)
    {
        _model.CurrentDisplayPageIndex = 0;
        _model.RefreshDesplayList();
    }

    public void EnterFilterString(string filterString)
    {
        _currentFrameFilterString = filterString;
    }

    public void SwitchConsole()
    {
        _model.IsOpenConsole = !_model.IsOpenConsole;
        if (_model.IsOpenConsole)
        {
            SetToLastPage();
        }
    }

    public void SetToLastPage()
    {
        _model.CurrentDisplayPageIndex = _model.GetLastPageIndex();
        _model.RefreshDesplayList();
    }

    public void NextPage()
    {
        if (_model.IsDesplayLastPage)
        {
            return;
        }
        _model.CurrentDisplayPageIndex++;
        _model.RefreshDesplayList();
    }

    public void PrePage()
    {
        if (_model.CurrentDisplayPageIndex == 0)
        {
            return;
        }

        _model.CurrentDisplayPageIndex--;
        _model.RefreshDesplayList();
    }

    //next frame apply selected , instand in Layout or Repaint event
    public void ApplyUpdate()
    {
        _model.SelectedLogInfo = _currentFrameSelectedLog;
        _model.FilterString = _currentFrameFilterString;
        _model.ConsoleScroll = _currentFrameScrollPosition;

        if (Input.GetMouseButtonDown(0))
        {
            _mousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 moveVector = _mousePosition - Input.mousePosition;
            _model.ConsoleScroll += new Vector2(0f, -moveVector.y) * _console.ScrollSpeed;
            _mousePosition = Input.mousePosition;
        }
    }

    public void ClearAllLog()
    {
        _model.ClearAllLog();
    }

    public void ConsoleScroll(Vector2 value)
    {
        _currentFrameScrollPosition = value;
    }

    public void ExportLog()
    {
        if (!Directory.Exists(LogSetting.Instance.ExportFileSaveDirectory))
        {
            Directory.CreateDirectory(LogSetting.Instance.ExportFileSaveDirectory);
        }

        if (!File.Exists(LogSetting.Instance.FullExportFileSavePath))
        {
            FileStream stream = File.Create(LogSetting.Instance.FullExportFileSavePath);
            stream.Flush();
            stream.Close();
        }
        LDebug.Log("export to :" + LogSetting.Instance.FullExportFileSavePath);

        using (StreamWriter outputFile = new StreamWriter(LogSetting.Instance.FullExportFileSavePath, false))
        {
            List<LogInfo> list = _model.LogInfoList;
            
            foreach (LogInfo item in list)
            {
                //outputFile.Write("\n");
                outputFile.WriteLine(item.Content);
#if !UNITY_ANDROID
                if (!string.IsNullOrEmpty(item.StackTrace))
                {
                    outputFile.Write("\n");
                    outputFile.WriteLine(item.StackTrace);
                }
#endif
            }
            outputFile.Close();
        }
    }

    public LogConsoleControl()
    {
        _currentFrameFilterString = "";
    }

    private LogInfo _currentFrameSelectedLog;
    private string _currentFrameFilterString;
    private Vector2 _currentFrameScrollPosition;
    private LogConsoleModel _model;
    private LogConsole _console;

    private Vector3 _mousePosition;
}
