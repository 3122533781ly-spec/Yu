using UnityEngine;

public class LogConsole : MonoBehaviour
{
    [SerializeField]
    public GUISkin Skin = null;
    [SerializeField]
    public float ScrollSpeed = 5f;
#if LOG_INFO || LOG_DEBUG || LOG_WARNING || LOG_ERROR
    private void Awake()
    {
        HandleMVC();
#if UNITY_EDITOR
        Application.logMessageReceivedThreaded += _control.LogMessageReceived;
#elif UNITY_ANDROID
        Application.logMessageReceivedThreaded += _control.LogMessageReceived;
#else
        Application.logMessageReceivedThreaded += _control.LogMessageReceived;
#endif
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        Application.logMessageReceivedThreaded -= _control.LogMessageReceived;
#elif UNITY_ANDROID
        Application.logMessageReceivedThreaded -= _control.LogMessageReceived;
#else
        Application.logMessageReceivedThreaded -= _control.LogMessageReceived;
#endif
    }

    private void OnGUI()
    {
        GUI.skin = Skin;
        _view.DrawLogConsole();
    }

    private void OnEnable()
    {
        _model.OnAddNewLog += _view.MoveScrollToBottom;
        _model.OnFilterStringChanged += _control.FilterLogWithString;
        _control.SetToLastPage();
    }

    private void OnDisable()
    {
        _model.OnAddNewLog -= _view.MoveScrollToBottom;
        _model.OnFilterStringChanged -= _control.FilterLogWithString;
    }

    private void Update()
    {
        _control.ApplyUpdate();
    }

    private void HandleMVC()
    {
        _model = new LogConsoleModel();
        _view = new LogConsoleView();
        _control = new LogConsoleControl();

        _view.InjectDependence(_model, _control);
        _control.InjectDependence(_model, this);
    }

    private LogConsoleModel _model;
    private LogConsoleControl _control;
    private LogConsoleView _view;
#endif
}