using UnityEngine;
using System.Collections;

public class LogConsoleView
{
    public LogConsoleView()
    {
        _windowRect = new Rect(0f, 60f, WIDTH, HEIGHT - 60f);
        _titleRect = new Rect(0f, 0f, _windowRect.width, 24f);
    }

    public void InjectDependence(LogConsoleModel model, LogConsoleControl control)
    {
        _model = model;
        _control = control;
    }

    public void DrawLogConsole()
    {
        float ratio = Screen.width / WIDTH < Screen.height / HEIGHT ? Screen.width / WIDTH : Screen.height / HEIGHT;
        if (ratio != _ratio)
        {
            _ratio = ratio;
            _guiMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(ratio, ratio, 1));
        }

        GUI.matrix = _guiMatrix;

        GUILayout.BeginHorizontal();
        GUILayout.Space(50f);

        if (GUI.Button(new Rect(100, 0, 100, 50), "Log"))
        {
            _control.SwitchConsole();
        }

        GUILayout.EndHorizontal();

        if (_model.IsOpenConsole)
        {
            _windowRect = GUI.Window(0, _windowRect, WindowFunction, "Log Console");
        }
    }

    private void WindowFunction(int windowID)
    {
        GUI.DragWindow(_titleRect);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(40)))
        {
            _control.ClearAllLog();
        }

        if (GUILayout.Button("Export", GUILayout.Width(100), GUILayout.Height(40)))
        {
            _control.ExportLog();
        }

        if (GUILayout.Button("Mail Export", GUILayout.Width(100), GUILayout.Height(40)))
        {
            _control.ExportLog();
            LogEmailSender.SendEmail();
        }

        if (GUILayout.Button("Cancel Mail", GUILayout.Width(100), GUILayout.Height(40)))
        {
            LogEmailSender.CancelSend();
        }

        //MVC 此处表达了　model -> view -> control 的通信
        _control.EnterFilterString(GUILayout.TextField(_model.FilterString));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        DrawScrollViewContent();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        if (GUILayout.Button("丄", GUILayout.Width(50), GUILayout.Height(200)))
        {
            _control.PrePage();
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("丅", GUILayout.Width(50), GUILayout.Height(200)))
        {
            _control.NextPage();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void DrawScrollViewContent()
    {
        float oldScrollY = _consoleScroll.y;
        _consoleScroll = GUILayout.BeginScrollView(_model.ConsoleScroll);
        {
            DrawLog();
            GUIUtility.ScaleAroundPivot(Vector2.one, Vector2.zero);
        }

        if (!Mathf.Approximately(oldScrollY, _consoleScroll.y))
        {
            _control.ConsoleScroll(_consoleScroll);
        }

        GUILayout.EndScrollView();
    }

    private void DrawLog()
    {
        for (int i = 0; i < _model.DisplayLogInfoList.Count; i++)
        {
            LogInfo infoItem = _model.DisplayLogInfoList[i];

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(infoItem.Content,
                new GUIStyle()
                {
                    normal = new GUIStyleState() {textColor = GetColorByLogType(infoItem.Type)},
                    fontSize = LogSetting.Instance.FrontSize,
                    wordWrap = true,
                },
                GUILayout.Width(_windowRect.width - 30)))
            {
                if (_model.SelectedLogInfo == infoItem)
                {
                    _control.SelectedOneLog(new LogInfo());
                }
                else
                {
                    _control.SelectedOneLog(infoItem);
                }
            }

            GUILayout.EndHorizontal();

            if (infoItem == _model.SelectedLogInfo)
            {
#if !UNITY_ANDROID
                DrawStackTrace();
#endif
            }
        }
    }

    private void DrawStackTrace()
    {
        GUILayout.BeginVertical();
        GUILayout.Label(FilterStackTraceLine(_model.SelectedLogInfo.StackTrace, 6), new GUIStyle()
        {
            normal = new GUIStyleState() {textColor = GetColorByLogType(_model.SelectedLogInfo.Type)},
            wordWrap = true
        }, GUILayout.Width(Screen.width - 100f));
        GUILayout.EndVertical();
    }

    private Color GetColorByLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                return Color.white;
            case LogType.Error:
            case LogType.Exception:
                return Color.red;
            case LogType.Warning:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    public void MoveScrollToBottom()
    {
//        _consoleScroll = new Vector2(0, int.MaxValue);
    }

    private string FilterStackTraceLine(string content, int lineCount)
    {
        if (!string.IsNullOrEmpty(content))
        {
            string result = content;
            for (int i = 0; i < lineCount; i++)
            {
                result = result.Substring(result.IndexOf("\n") + 1);
            }

            return result;
        }

        return "";
    }

    private Rect _windowRect;
    private Rect _titleRect;
    private Matrix4x4 _guiMatrix;
    private float _ratio;
    private Vector2 _consoleScroll;

    private float WIDTH = Screen.width / 2;
    private float HEIGHT = Screen.height / 2;
    private const float NEW_REF_MILLISECONDS = 5000f;

    private LogConsoleModel _model;
    private LogConsoleControl _control;
}