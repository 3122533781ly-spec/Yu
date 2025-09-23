using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Redeem
{
    public enum RedeemEntryType
    {
        PayPal,
        GiftCard,
    }
    [Serializable]
    public class RedeemEntryBaseData
    {
        public RedeemEntryType type;
        public RedeemEntryState state;
        public int redeemDataID;
        public string inviteLink;
        //public int serverID;
        //public string serverNote;
        public int nowConditionIndex;
        //条件进度 修改时 需要判断是否与当前进行中的条件类型一致
        public RedeemConditionProgressData nowConditionProgressData;

        public Action onStateChanged;
        public Action onDataChanged;

        public RedeemEntryBaseData(int redeemDataID, RedeemEntryState state)
        {
            RefreshRedeemDataID(redeemDataID);
            ChangeState(state);
            ResetCondition();
            Debug.Log("RedeemEntryBaseData.....");
        }

        public void ResetCondition()
        {
            nowConditionIndex = 0;
            RedeemConfigBaseData redeemData = GetRedeemData();
            RedeemConditionData conditionData = redeemData.conditionDatas[nowConditionIndex];
            nowConditionProgressData = new RedeemConditionProgressData(conditionData);
        }

        public virtual RedeemConfigBaseData GetRedeemData()
        {
            return null;
        }
        /// <summary>
        /// 当修改了表格中配置ID时刷新
        /// </summary>
        /// <param name="redeemDataID"></param>
        public void RefreshRedeemDataID(int redeemDataID)
        {
            Debug.Log($"RefreshRedeemDataID this redeemDataID:{this.redeemDataID},redeemDataID:{redeemDataID}");
            if (this.redeemDataID != redeemDataID)
            {
                this.redeemDataID = redeemDataID;
            }
        }

        public void ChangeState(RedeemEntryState state)
        {
            this.state = state;
            onStateChanged?.Invoke();
            if (type == RedeemEntryType.PayPal)
            {
                RedeemConfigBaseData redeemData = GetRedeemData();
                if (redeemData != null)
                {
                    StaticModule.RedeemState(redeemData.money, state);
                }
            }

        }

        public void CheckNowConditionFinish()
        {
            if (nowConditionProgressData != null && state == RedeemEntryState.NeedFinishCondition)
            {
                if (nowConditionProgressData.CheckFinish())
                {
                    EnterNextCondition();
                }
            }
        }

        public void EnterNextCondition()
        {
            RedeemConfigBaseData redeemData = GetRedeemData();
            StaticModule.RedeemTaskFinish(redeemData.money, nowConditionIndex);

            nowConditionIndex++;

            if (nowConditionIndex < redeemData.conditionDatas.Count)
            {
                RedeemConditionData conditionData = redeemData.conditionDatas[nowConditionIndex];
                nowConditionProgressData = new RedeemConditionProgressData(conditionData);
            }
            else
            {
                //完成所有条件
                ChangeState(RedeemEntryState.CanConfirm);
            }
        }
    }
}
