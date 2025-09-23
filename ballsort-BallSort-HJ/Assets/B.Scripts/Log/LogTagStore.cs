#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public partial class LogTagStore
{
    public static LogTagStore Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LogTagStore();
                Fangtang.Log.Init(new CheckTagDelegate());
                _instance.InitEditorData();
            }
            return _instance;
        }
    }

    public const string PrefsKey = "EditorTagIntFlagValue";

    public List<bool> FlagFilterSwitcherList { get; private set; }
    public int RunTimeFlagInt { get; set; }

    public int CurrentFlagInt
    {
        get
        {
            return _currentFlagInt;
        }
        set
        {
            if (_currentFlagInt != value)
            {
                _currentFlagInt = value;
                EditorPrefs.SetInt(PrefsKey, _currentFlagInt);
                Debug.Log(_currentFlagInt);
            }
        }
    }
    
    public bool Check(int tagInt)
    {
        return (_currentFlagInt & (1 << (tagInt - 1))) != 0;
    }
    public void SetFilterSwitcherValueByName(int index, bool value)
    {
        int logFilter = 0;
        for (int i = 0; i < LogSetting.Instance.All.Count; i++)
        {
            if (index == i)
            {
                if (value)
                {
                    logFilter |= 1 << (i);
                }
            }
            else
            {
                if ((CurrentFlagInt & (1 << i)) != 0)
                {
                    logFilter |= 1 << (i);
                }
            }
        }
        CurrentFlagInt = logFilter;
    }

    public int GetSelectedAllFlagInt()
    {
        int logFilter = 0;
        for (int i = 0; i < LogSetting.Instance.All.Count; i++)
        {
            logFilter |= 1 << (i + 1);
        }
        return logFilter;
    }

    private void InitEditorData()
    {
        _currentFlagInt = EditorPrefs.GetInt(PrefsKey, -1);

        RunTimeFlagInt = Fangtang.LogTag.RuntimeFlagInt;
    }

    private bool[] FlagIntToBoolList(int flagInt, int length)
    {
        bool[] result = new bool[length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (flagInt & (1 << i)) != 0;
        }
        return result;
    }

    public int _currentFlagInt;
    private LogTagStore() { }
    private static LogTagStore _instance;
}
#endif