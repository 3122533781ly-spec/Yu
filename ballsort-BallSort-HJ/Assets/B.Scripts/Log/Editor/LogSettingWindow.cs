using UnityEngine;
using UnityEditor;
using Fangtang;

public class LogSettingWindow : EditorWindow
{
    [MenuItem("Tools/Log/LogTagSettingWindow", false, 10)]
    public static void SetOff()
    {
        GetWindow<LogSettingWindow>();
    }

    private void OnEnable()
    {
        Fangtang.Log.Init(new CheckTagDelegate());
        _displayMode = 1;
    }

    private void OnGUI()
    {
        EditorGUILayout.Separator();

        _displayMode = EditorGUILayout.Popup(_displayMode, new string[] { "Scroll", "Pop" });
        if (_displayMode == 0)
        {
            DrawLogTagsFilterGUI();
        }
        else
        {
            DrawLogTagsFilterPopup();
        }

        _isOpenRuntimeEditor = EditorGUILayout.Toggle("Open Runtime Log Tags Fliter", _isOpenRuntimeEditor);
        if (_isOpenRuntimeEditor)
        {
            LogTagStore.Instance.RunTimeFlagInt =
                EditorGUILayout.MaskField(LogTagStore.Instance.RunTimeFlagInt, LogSetting.Instance.All.ToArray());
        }


        if (GUILayout.Button("Renew LogTag.cs"))
        {
            Fangtang.LogTagClassHelper.UpdateVehicleTypeEnum(
                LogSetting.Instance.All,
                LogTagStore.Instance.RunTimeFlagInt);
        }
    }

    private void DrawLogTagsFilterGUI()
    {
        EditorGUILayout.LabelField("Log Tags Filter:");
        _scrollViewPosition = GUILayout.BeginScrollView(_scrollViewPosition);
        GUILayout.BeginVertical();
        for (int i = 0; i < LogSetting.Instance.All.Count; i++)
        {
            bool isEnable = GUILayout.Toggle(
                (
                LogTagStore.Instance.CurrentFlagInt & (1 << i)) != 0,
                LogSetting.Instance.All[i]
                );

            LogTagStore.Instance.SetFilterSwitcherValueByName(i, isEnable);
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select all"))
        {
            for (int i = 0; i < LogSetting.Instance.All.Count; i++)
            {
                LogTagStore.Instance.SetFilterSwitcherValueByName(i, true);
            }
        }
        if (GUILayout.Button("Un Select all"))
        {
            for (int i = 0; i < LogSetting.Instance.All.Count; i++)
            {
                LogTagStore.Instance.SetFilterSwitcherValueByName(i, false);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLogTagsFilterPopup()
    {
        EditorGUILayout.LabelField("Log Tags Filter:");
        LogTagStore.Instance.CurrentFlagInt = EditorGUILayout.MaskField(LogTagStore.Instance.CurrentFlagInt, LogSetting.Instance.All.ToArray());
    }

    private Vector2 _scrollViewPosition;
    private int _displayMode;
    private bool _isOpenRuntimeEditor;
}
