using ProjectSpace.BubbleMatch.Scripts.Util;
using UnityEngine;

[System.Serializable]
public class GoodsData : IConfig
{
    public string Name;
    public int IntID;
    public GoodType Type;
    public int subType;
    public string Remark;

    public int ID
    {
        get { return IntID; }
    }

    public Sprite GetGoodTypeIcon()
    {
        return SpriteManager.Instance.GetGoodTypeIconByType(Type, subType);
    }
}

[System.Serializable]
public enum GoodType
{
    Coin = 0,
    SkinBall = 1,
    SkinTube = 2,
    SkinTheme = 3,
    Tool = 4,
    Background = 5,
    Tile = 6,
    Manual_Time = 7,
    GoldBrick = 8,
    MagicStick = 9,
    RemoveAD = 10,
    
    Prop = 12,
    Gem = 13,
    Money = 14,
    Star = 15,
}

[System.Serializable]
public enum GoodSubType
{
    Null = -1,
    Coin = 0,
    RevocationTool = 1,
    AddPipe = 2,
}