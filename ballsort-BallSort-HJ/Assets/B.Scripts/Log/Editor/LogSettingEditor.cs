using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LogSetting))]
public class LogSettingEditor : Editor
{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        _logTagListControl.Draw(_logTagListAdaptor);
//    }
//
//    private void OnEnable()
//    {
//        LogSetting setting = target as LogSetting;
//
//        _logTagListAdaptor = new GenericListAdaptor<string>(setting.All, ItemDrawer, 15f);
//        _logTagListControl = new ReorderableListControl(ReorderableListFlags.DisableDuplicateCommand | ReorderableListFlags.DisableContextMenu);
//    }
//
//    private string ItemDrawer(Rect position, string item)
//    {
//        item = EditorGUI.TextField(position, item);
//        return item;
//    }
//
//    private ReorderableListControl _logTagListControl;
//    private GenericListAdaptor<string> _logTagListAdaptor;
}