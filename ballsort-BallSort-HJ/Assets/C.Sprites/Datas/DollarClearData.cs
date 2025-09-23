using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DollarClearData : IConfig
{
    public int id;

    //玩家当前美金区间
    public Vector2 range;

    //奖励金额
    public float number;

    //出现概率（如果判定不是美元，则出宝石）
    public int probability;

    //玩家当前宝石区间([1,10]代表1~10刀之间)
    public Vector2 gemRange;

    //钻石奖励金额
    public int gemNumber;

    public int ID => id;
}