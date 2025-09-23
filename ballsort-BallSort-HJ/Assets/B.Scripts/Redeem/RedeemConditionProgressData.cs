using Lei31.Localizetion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Redeem
{
    /// <summary>
    /// 条件完成进度数据
    /// </summary>
    [Serializable]
    public class RedeemConditionProgressData
    {
        public RedeemConditionType type;
        public int ownCount;
        public float needCount;

        public string beginTime;

        public int invitePlayer;
        public int activePlayer;

        public RedeemConditionProgressData(RedeemConditionData conditionData)
        {
            type = conditionData.type;
            ownCount = 0;
            invitePlayer = 0;
            activePlayer = 0;

            needCount = conditionData.value;
            if (type == RedeemConditionType.LoginGame)
            {
                beginTime = OnlineTimeManager.Instance.GetNowRealTime().ToString();
            }
        }

        /// <summary>
        /// 用于修改了条件中目标数量时刷新
        /// </summary>
        /// <param name="conditionData"></param>
        public void RefreshConditionData(RedeemConditionData conditionData)
        {
            if (type == conditionData.type)
            {
                if (needCount != conditionData.value)
                {
                    needCount = conditionData.value;
                }
            }
        }

        public void AddCount(int count = 1)
        {
            ownCount += count;
        }

        public int GetRemainTime(DateTime nowTime)
        {
            return (int)(DateTime.Parse(beginTime).AddHours(needCount * 24) - nowTime).TotalSeconds;
        }

        public int GetPassTime(DateTime nowTime)
        {
            return (int)(nowTime - DateTime.Parse(beginTime)).TotalSeconds;
        }

        public bool CheckFinish()
        {
            if (type == RedeemConditionType.LoginGame)
            {
                return GetRemainTime(OnlineTimeManager.Instance.GetNowRealTime()) <= 0;
            }
            else if (type == RedeemConditionType.InvitePlayer)
            {
                return invitePlayer >= needCount;
            }
            else if (type == RedeemConditionType.ActivePlayer)
            {
                return activePlayer >= needCount;
            }
            else
            {
                return ownCount >= needCount;
            }
        }

        public float GetProgress()
        {
            if (type == RedeemConditionType.LoginGame)
            {
                return (float)GetPassTime(OnlineTimeManager.Instance.GetNowRealTime()) / (needCount * 24 * 3600f);
            }
            else if (type == RedeemConditionType.InvitePlayer)
            {
                return (float)invitePlayer / needCount;
            }
            else if (type == RedeemConditionType.ActivePlayer)
            {
                return (float)activePlayer / needCount;
            }
            else
            {
                return (float)ownCount / needCount;
            }
        }

        public string GetProgressStr()
        {
            if (type == RedeemConditionType.LoginGame)
            {
                string timeStr = "";
                if (needCount >= 1f)
                {
                    timeStr = $"{(GetPassTime(OnlineTimeManager.Instance.GetNowRealTime()) / (24 * 3600f)):0.0}/ {needCount}";
                }
                else
                {
                    timeStr = $"{(GetPassTime(OnlineTimeManager.Instance.GetNowRealTime()) / (24 * 3600f)):0.0}/ {needCount}";
                }
                return $"{timeStr} {LocalizationManager.Instance.GetTextByTag(LocalizationConst.TAG_DAYS)}";
            }
            else if (type == RedeemConditionType.InvitePlayer)
            {
                return $"{invitePlayer}/{needCount}";
            }
            else if (type == RedeemConditionType.ActivePlayer)
            {
                return $"{activePlayer}/{needCount}";
            }
            else
            {
                return $"{ownCount}/{needCount}";
            }
        }
    }
}