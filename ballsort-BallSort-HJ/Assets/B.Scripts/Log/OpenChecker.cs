using UnityEngine;
using System.Collections;

public class OpenChanker : MonoBehaviour
{
    public bool IsOpen
    {
        get { return _isOpen; }
        private set
        {
            _isOpen = value;
            _console.enabled = value;
        }
    }

    public void Update()
    {
        _realDeltaTime = Time.realtimeSinceStartup - _lastFrameTime;

        _intervalTimer += _realDeltaTime;
        if (_intervalTimer > LogSetting.Instance.ClickInterval
            && !Input.GetMouseButton(0))
        {
            _clickCount = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _clickCount++;
            _intervalTimer = 0;
        }

        if (Input.GetMouseButton(0))
        {
            _holdTimer += _realDeltaTime;
        }
        else
        {
            _holdTimer = 0f;
        }

        if (_clickCount >= LogSetting.Instance.OpenEntryClickCount
            && _holdTimer > LogSetting.Instance.OpenEntryHoldPointerTime)
        {
            IsOpen = !IsOpen;
            _clickCount = 0;
        }

        _lastFrameTime = Time.realtimeSinceStartup;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                IsOpen = !IsOpen;
            }
        }
    }

    public void Draw()
    {
        GUILayout.Label(_holdTimer.ToString());
        GUILayout.Label(_clickCount.ToString());
    }

    private void Awake()
    {
        IsOpen = false;
        _clickCount = 0;
    }

    [SerializeField] private LogConsole _console = null;

    private float _holdTimer;

    private float _lastFrameTime;
    private float _realDeltaTime;
    private float _intervalTimer;
    private int _clickCount;

    private bool _isOpen;
}
