using Lei31.Localizetion;
using System;
using UnityEngine;

[Serializable]
public class GuideUIData : IConfig
{
    public int Id;
    public int Type;
    public int Index;
    public string Info;
    public int DialogY;
    public bool HideArrow;
    public int Gesture;
    public string GesturePath;
    public string Path;
    public int Shape;
    public int Event;
    public string EventPath;
    public int Role;
    public Vector2Int RolePos;
    public Vector2Int ArrowOffset;
    public int ID => Id;

    public GuideData Parse()
    {
        GuideData guideData = new GuideData();
        guideData.Type = Type;
        guideData.Index = Index;
        if (Info != null && Info != "")
        {
            //if(Info != "-1") guideData.Info = LocalizationManager.Instance.GetTextByTag(Info);
            //else guideData.Info = Info;
            guideData.Info = Info;
        }
        else guideData.Info = Info;
        guideData.DialogY = DialogY;
        guideData.HideArrow = HideArrow;
        guideData.Gesture = (EGuideGesture)Gesture;
        guideData.GesturePath = GesturePath;
        guideData.Path = Path;
        guideData.Shape = (EGuideShape)Shape;
        guideData.Event = (EEventGuideType)Event;
        guideData.EventPath = EventPath;
        guideData.Role = Role;
        guideData.RolePos = new Vector2(RolePos.x, RolePos.y);
        guideData.ArrowOffset = new Vector2(ArrowOffset.x, ArrowOffset.y);
        return guideData;
    }
}