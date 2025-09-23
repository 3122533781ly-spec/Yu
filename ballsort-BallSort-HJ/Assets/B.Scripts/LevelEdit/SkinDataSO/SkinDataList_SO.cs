using ProjectSpace.BubbleMatch.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinDataList_SO", menuName = "ScriptTableObjects/SkinDataList", order = 1)]
public class SkinDataList_SO : ScriptableObject
{
    [Header("皮肤")] public List<SkinData> Skins;

    public Sprite GetSkin(int Value, SkinType skinType)
    {
        switch (skinType)
        {
            case SkinType.Tube:
                return Skins[Value].tube;

            case SkinType.Bg:
                return Skins[Value].bgIcon;

            default:
                return null;
        }
    }

    //获取球数组
    public List<Sprite> GetBallSkin(int Value)
    {
        return Skins[Value].ballIcon;
    }

    //获取线条数据
    public Color GetLineColor(int Value)
    {
        return Skins[Value].color;
    }

    public Sprite GetMysBallSprite(int Value)
    {
        return Skins[Value].MysteriousBall;
    }
}

[Serializable]
public class SkinData
{
    [Header("管皮肤->")] public Sprite tube;
    [Header("球皮肤")] public List<Sprite> ballIcon;
    [Header("背景皮肤")] public Sprite bgIcon;
    [Header("线条颜色")] public Color color;
    [Header("神秘球皮肤")] public Sprite MysteriousBall;
}