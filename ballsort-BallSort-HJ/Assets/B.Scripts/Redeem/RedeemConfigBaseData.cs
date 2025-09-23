using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Redeem
{
    [Serializable]
    public class RedeemConfigBaseData : IConfig
    {
        public int id;
        public int money;
        public string configType;
        public List<RedeemConditionData> conditionDatas;
        public string remark;
        public int ID => id;
    }
    /// <summary>
    /// 条件类型（0看广告，1邀请人数，2激活人数，3玩游戏多少局，4继续玩游戏天数）
    /// </summary>
    public enum RedeemConditionType
    {
        WatchAD,
        InvitePlayer,
        ActivePlayer,
        PlayGame,
        LoginGame,
    }

    [Serializable]
    public class RedeemConditionData
    {
        public RedeemConditionType type;
        public int id;
        public float value;

        public RedeemConditionData(Vector3 vector3)
        {
            type = (RedeemConditionType)((int)vector3.x);
            id = (int)vector3.y;
            value = vector3.z;
        }
    }
}
